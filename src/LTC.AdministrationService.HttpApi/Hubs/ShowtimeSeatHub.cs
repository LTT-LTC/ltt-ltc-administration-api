using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace LTC.AdministrationService.Hubs;

[AllowAnonymous]
public class ShowtimeSeatHub : Hub
{
    public static string GroupName(Guid showtimeId) => $"showtime-seat-{showtimeId:N}";

    public Task JoinShowtime(string showtimeId)
    {
        if (!Guid.TryParse(showtimeId, out var id))
        {
            return Task.CompletedTask;
        }

        return Groups.AddToGroupAsync(Context.ConnectionId, GroupName(id));
    }

    public Task LeaveShowtime(string showtimeId)
    {
        if (!Guid.TryParse(showtimeId, out var id))
        {
            return Task.CompletedTask;
        }

        return Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(id));
    }
}
