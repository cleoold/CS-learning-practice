using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using Chatroom.Shared;

namespace Chatroom.Server.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(ILogger<ChatHub> logger)
        {
            _logger = logger;
        }

        public async Task SendPublicMessage(PublicMessage message)
        {
            _logger.LogInformation($"[{message.Time}] [PUBLIC] {message.User}: {message.Content}");
            await Clients.All.SendAsync("ReceivePublicMessage", message);
        }
    }
}
