using System.Threading;
using System.Threading.Tasks;

namespace LTC.AdministrationService.Customer.Showtimes;

/// <summary>Broadcasts seat hold/release updates (SignalR or similar).</summary>
public interface ISeatHoldRealtimeNotifier
{
    Task NotifyAsync(SeatHoldRealtimeNotification notification, CancellationToken cancellationToken = default);
}
