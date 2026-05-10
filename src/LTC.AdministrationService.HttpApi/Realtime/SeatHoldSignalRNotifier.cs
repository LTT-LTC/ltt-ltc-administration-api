using System.Threading;
using System.Threading.Tasks;
using LTC.AdministrationService.Customer.Showtimes;
using LTC.AdministrationService.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace LTC.AdministrationService.Realtime;

public class SeatHoldSignalRNotifier : ISeatHoldRealtimeNotifier
{
    private readonly IHubContext<ShowtimeSeatHub> _hubContext;

    public SeatHoldSignalRNotifier(IHubContext<ShowtimeSeatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyAsync(SeatHoldRealtimeNotification notification, CancellationToken cancellationToken = default)
    {
        var group = ShowtimeSeatHub.GroupName(notification.ShowtimeId);
        await _hubContext.Clients.Group(group).SendAsync(
            "SeatMapDelta",
            new
            {
                kind = notification.Kind,
                seatCodes = notification.SeatCodes,
                sessionKey = notification.SessionKey,
                expiresAtUtc = notification.ExpiresAtUtc,
            },
            cancellationToken);
    }
}
