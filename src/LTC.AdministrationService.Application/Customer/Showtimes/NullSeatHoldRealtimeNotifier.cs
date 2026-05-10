using System.Threading;
using System.Threading.Tasks;

namespace LTC.AdministrationService.Customer.Showtimes;

public class NullSeatHoldRealtimeNotifier : ISeatHoldRealtimeNotifier
{
    public Task NotifyAsync(SeatHoldRealtimeNotification notification, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
