using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LTC.Shared.Hosting.Microservices.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace LTC.AdministrationService.Messaging;

/// <summary>
/// Hangfire job that re-publishes a failed ShowtimeSeatMergeRequested message back to the main queue
/// with an incremented retry count header. Scheduled by <see cref="ShowtimeSeatMergeRequestedConsumer"/>
/// after a transient failure; after 3 attempts the consumer lets the message route to DLQ.
/// </summary>
public class ShowtimeSeatMergeDlqRetryJob
{
    private readonly RabbitMqOptions _options;
    private readonly ILogger<ShowtimeSeatMergeDlqRetryJob> _logger;

    public ShowtimeSeatMergeDlqRetryJob(
        IOptions<RabbitMqOptions> options,
        ILogger<ShowtimeSeatMergeDlqRetryJob> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public Task ExecuteAsync(string messageJson, string messageId, int retryCount)
    {
        if (!_options.Enabled || string.IsNullOrWhiteSpace(_options.HostName))
        {
            _logger.LogWarning("ShowtimeSeatMergeDlqRetryJob skipped: RabbitMQ not configured.");
            return Task.CompletedTask;
        }

        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port > 0 ? _options.Port : 5672,
            VirtualHost = string.IsNullOrEmpty(_options.VirtualHost) ? "/" : _options.VirtualHost,
            UserName = string.IsNullOrEmpty(_options.UserName) ? "guest" : _options.UserName,
            Password = _options.Password ?? "guest",
        };

        try
        {
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(
                exchange: _options.Exchange,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                arguments: null);

            var body = Encoding.UTF8.GetBytes(messageJson);
            var props = channel.CreateBasicProperties();
            props.Persistent = true;
            props.MessageId = messageId;
            props.Headers = new Dictionary<string, object>
            {
                ["x-retry-count"] = retryCount
            };

            channel.BasicPublish(
                exchange: _options.Exchange,
                routingKey: _options.RoutingKeys.ShowtimeSeatMergeRequested,
                mandatory: false,
                basicProperties: props,
                body: body);

            _logger.LogInformation(
                "ShowtimeSeatMergeDlqRetryJob re-published MessageId={MessageId} RetryCount={RetryCount}",
                messageId,
                retryCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "ShowtimeSeatMergeDlqRetryJob failed to re-publish MessageId={MessageId}",
                messageId);
            throw;
        }

        return Task.CompletedTask;
    }
}
