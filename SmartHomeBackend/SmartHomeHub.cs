using Microsoft.AspNetCore.SignalR;

namespace SmartHomeBackend;

public class SmartHomeHub :Hub  
{
    public async Task SendMessage(string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", message);
    }
}