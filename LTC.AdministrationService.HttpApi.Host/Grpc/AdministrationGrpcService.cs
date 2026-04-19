using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using LTC.AdministrationService.Entities;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories;

namespace LTC.AdministrationService.Grpc;

public class AdministrationGrpcService : AdministrationGrpc.AdministrationGrpcBase
{
    private readonly IRepository<Entities.Cinema, Guid> _cinemaRepository;
    private readonly IRepository<Entities.Showtime, Guid> _showtimeRepository;
    private readonly IRepository<Entities.GiftCode, Guid> _giftCodeRepository;

    public AdministrationGrpcService(
        IRepository<Entities.Cinema, Guid> cinemaRepository,
        IRepository<Entities.Showtime, Guid> showtimeRepository,
        IRepository<Entities.GiftCode, Guid> giftCodeRepository)
    {
        _cinemaRepository = cinemaRepository;
        _showtimeRepository = showtimeRepository;
        _giftCodeRepository = giftCodeRepository;
    }

    public override async Task<CinemasResponse> GetCinemas(Empty request, ServerCallContext context)
    {
        var cinemas = await _cinemaRepository.GetListAsync();
        var response = new CinemasResponse();
        foreach (var cinema in cinemas)
        {
            response.Items.Add(new CinemaMessage
            {
                Id = cinema.Id.ToString(),
                Name = cinema.Name,
                City = cinema.City ?? string.Empty,
                Address = cinema.Address ?? string.Empty,
                PhoneNumber = cinema.ServiceNumber ?? string.Empty,
                Status = cinema.Status ?? string.Empty
            });
        }
        return response;
    }

    public override async Task<CinemaMessage> GetCinemaById(GetByIdRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var id))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid Cinema ID"));
        }

        var cinema = await _cinemaRepository.GetAsync(id);
        return new CinemaMessage
        {
            Id = cinema.Id.ToString(),
            Name = cinema.Name,
            City = cinema.City ?? string.Empty,
            Address = cinema.Address ?? string.Empty,
            PhoneNumber = cinema.ServiceNumber ?? string.Empty,
            Status = cinema.Status ?? string.Empty
        };
    }

    public override async Task<ShowtimesResponse> GetShowtimes(GetShowtimesRequest request, ServerCallContext context)
    {
        var queryable = await _showtimeRepository.GetQueryableAsync();
        
        if (!string.IsNullOrWhiteSpace(request.MovieId) && Guid.TryParse(request.MovieId, out var distributionId))
        {
            queryable = queryable.Where(x => x.DistributionId == distributionId);
        }

        if (!string.IsNullOrWhiteSpace(request.CinemaId) && Guid.TryParse(request.CinemaId, out var cinemaId))
        {
            queryable = queryable.Where(x => x.CinemaId == cinemaId);
        }

        if (!string.IsNullOrWhiteSpace(request.Date) && DateTime.TryParse(request.Date, out var date))
        {
            var targetDate = date.Date;
            queryable = queryable.Where(x => x.ShowDate == targetDate);
        }

        var showtimes = await queryable.OrderBy(x => x.StartTime).ToListAsync();
        var response = new ShowtimesResponse();
        foreach (var showtime in showtimes)
        {
            response.Items.Add(new ShowtimeMessage
            {
                Id = showtime.Id.ToString(),
                CinemaId = showtime.CinemaId.ToString(),
                MovieId = showtime.DistributionId.ToString(),
                ScreenId = showtime.ScreenId.ToString(),
                StartTime = showtime.ShowDate.Date.Add(showtime.StartTime).ToString("o"),
                EndTime = showtime.ShowDate.Date.Add(showtime.EndTime).ToString("o"),
                TicketPrice = (double)showtime.BasePrice,
                FormatId = Guid.Empty.ToString(),
                Status = showtime.Status ?? string.Empty
            });
        }
        return response;
    }

    public override async Task<ShowtimeMessage> GetShowtimeById(GetByIdRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var id))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid Showtime ID"));
        }

        var showtime = await _showtimeRepository.GetAsync(id);
        return new ShowtimeMessage
        {
            Id = showtime.Id.ToString(),
            CinemaId = showtime.CinemaId.ToString(),
            MovieId = showtime.DistributionId.ToString(),
            ScreenId = showtime.ScreenId.ToString(),
            StartTime = showtime.ShowDate.Date.Add(showtime.StartTime).ToString("o"),
            EndTime = showtime.ShowDate.Date.Add(showtime.EndTime).ToString("o"),
            TicketPrice = (double)showtime.BasePrice,
            FormatId = Guid.Empty.ToString(),
            Status = showtime.Status ?? string.Empty
        };
    }

    public override async Task<GiftCodeMessage> ValidateGiftCode(ValidateGiftCodeRequest request, ServerCallContext context)
    {
        var giftCode = await _giftCodeRepository.FirstOrDefaultAsync(x => x.Code == request.Code);
        if (giftCode == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Gift code not found"));
        }

        return new GiftCodeMessage
        {
            Id = giftCode.Id.ToString(),
            Code = giftCode.Code,
            Amount = (double)giftCode.DiscountValue,
            ExpiryDate = (giftCode.EndDate ?? DateTime.MinValue).ToString("o"),
            IsUsed = giftCode.UsageLimit != null && giftCode.UsageCount >= giftCode.UsageLimit,
            Status = giftCode.Status ?? string.Empty
        };
    }
}
