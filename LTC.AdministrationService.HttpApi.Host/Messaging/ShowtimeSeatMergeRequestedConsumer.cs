using System;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LTC.AdministrationService.Customer.Showtimes;
using LTC.AdministrationService.Showtimes;
using LTC.AdministrationService.Showtimes.Dtos;
using LTC.Shared.Hosting.Microservices.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.Uow;

namespace LTC.AdministrationService.Messaging;

public class ShowtimeSeatMergeRequestedConsumer : BackgroundService
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);
    private static readonly ConcurrentDictionary<Guid, byte> ProcessedBookingIds = new();

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RabbitMqOptions _options;
    private readonly ILogger<ShowtimeSeatMergeRequestedConsumer> _logger;

    public ShowtimeSeatMergeRequestedConsumer(
        IServiceScopeFactory scopeFactory,
        IOptions<RabbitMqOptions> options,
        ILogger<ShowtimeSeatMergeRequestedConsumer> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.Enabled || string.IsNullOrWhiteSpace(_options.HostName))
        {
            _logger.LogWarning(
                "ShowtimeSeatMergeRequested consumer skipped (RabbitMq:Enabled=false or HostName empty).");
            return;
        }

        _logger.LogInformation(
            "ShowtimeSeatMergeRequested consumer starting with HostName={HostName} Queue={Queue}",
            _options.HostName,
            _options.Consumer.ShowtimeSeatMergeRequestedQueue);

        var factory = new global::RabbitMQ.Client.ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port > 0 ? _options.Port : 5672,
            VirtualHost = string.IsNullOrEmpty(_options.VirtualHost) ? "/" : _options.VirtualHost,
            UserName = string.IsNullOrEmpty(_options.UserName) ? "guest" : _options.UserName,
            Password = _options.Password ?? "guest",
        };

        global::RabbitMQ.Client.IConnection connection;
        try
        {
            connection = factory.CreateConnection();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "ShowtimeSeatMergeRequested consumer cannot connect to RabbitMQ at {HostName}:{Port}; administration API continues without consuming merge messages.",
                _options.HostName,
                _options.Port > 0 ? _options.Port : 5672);
            return;
        }

        using (connection)
        {
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(
                exchange: _options.Exchange,
                type: global::RabbitMQ.Client.ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                arguments: null);
            channel.QueueDeclare(
                queue: _options.Consumer.ShowtimeSeatMergeRequestedQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            channel.QueueBind(
                queue: _options.Consumer.ShowtimeSeatMergeRequestedQueue,
                exchange: _options.Exchange,
                routingKey: _options.RoutingKeys.ShowtimeSeatMergeRequested,
                arguments: null);
            channel.BasicQos(0, 1, false);

            var consumer = new global::RabbitMQ.Client.Events.AsyncEventingBasicConsumer(channel);
            consumer.Received += async (_, ea) =>
            {
                ShowtimeSeatMergeRequestedEvent? payload = null;
                try
                {
                    var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                    payload = JsonSerializer.Deserialize<ShowtimeSeatMergeRequestedEvent>(json, JsonSerializerOptions);
                    if (payload is null)
                    {
                        _logger.LogWarning("Skip invalid ShowtimeSeatMergeRequested payload");
                        channel.BasicAck(ea.DeliveryTag, false);
                        return;
                    }

                    if (!ProcessedBookingIds.TryAdd(payload.BookingId, 1))
                    {
                        _logger.LogInformation(
                            "Skip duplicate ShowtimeSeatMergeRequested for booking {BookingId}",
                            payload.BookingId);
                        channel.BasicAck(ea.DeliveryTag, false);
                        return;
                    }

                    try
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var uowManager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();
                        using (var uow = uowManager.Begin())
                        {
                            var merge = scope.ServiceProvider.GetRequiredService<IShowtimeSeatLayoutMergeAppService>();
                            await merge.MergePaidSeatsAsync(
                                payload.ShowtimeId,
                                new MergePaidShowtimeSeatsInputDto { SeatCodes = payload.SeatCodes ?? [] });
                        }

                        // Release Redis seat holds if session key is provided
                        if (!string.IsNullOrWhiteSpace(payload.SessionKey))
                        {
                            try
                            {
                                var seatHoldStore = scope.ServiceProvider.GetRequiredService<ShowtimeSeatHoldStore>();
                                var released = await seatHoldStore.ReleaseAsync(payload.ShowtimeId, payload.SessionKey);
                                if (released.Count > 0)
                                {
                                    _logger.LogInformation(
                                        "Released {Count} Redis seat holds for booking {BookingId}: {Seats}",
                                        released.Count,
                                        payload.BookingId,
                                        string.Join(",", released));
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(
                                    ex,
                                    "Failed to release Redis seat holds for booking {BookingId}; seats will expire naturally",
                                    payload.BookingId);
                            }
                        }

                        _logger.LogInformation(
                            "ShowtimeSeatMergeRequested applied. BookingId={BookingId}, ShowtimeId={ShowtimeId}",
                            payload.BookingId,
                            payload.ShowtimeId);

                        channel.BasicAck(ea.DeliveryTag, false);
                    }
                    catch
                    {
                        ProcessedBookingIds.TryRemove(payload.BookingId, out byte _);
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "RabbitMQ consume failure for ShowtimeSeatMergeRequested queue.");
                    try
                    {
                        channel.BasicNack(ea.DeliveryTag, false, requeue: true);
                    }
                    catch (Exception nackEx)
                    {
                        _logger.LogError(nackEx, "Failed to nack message.");
                    }
                }
            };

            channel.BasicConsume(
                queue: _options.Consumer.ShowtimeSeatMergeRequestedQueue,
                autoAck: false,
                consumerTag: string.Empty,
                noLocal: false,
                exclusive: false,
                arguments: null,
                consumer: consumer);

            try
            {
                await Task.Delay(Timeout.Infinite, stoppingToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // shutdown
            }
        }
    }
}
