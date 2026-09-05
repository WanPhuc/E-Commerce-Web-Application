using Microsoft.AspNetCore.SignalR;

namespace AuraMart.Notification.Infrastructure.Realtime;

public class GlobalHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }
}
