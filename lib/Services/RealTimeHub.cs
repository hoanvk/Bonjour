using Bonjour.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Bonjour.Lib.Services;

public class RealTimeHub : Hub
{
    private readonly ILogger<RealTimeHub> logger;

    public RealTimeHub(ILogger<RealTimeHub> logger)
    {
        this.logger = logger;
    }

    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    public async Task SendNotification(Message message)
    {
        logger.LogInformation(JsonConvert.SerializeObject(message));
        await Clients.All.SendAsync("ReceiveNotification", message);
    }
}