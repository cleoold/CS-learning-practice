using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using Chatroom.Shared;

namespace Chatroom.Server.Hubs
{
    public class ChatHub : Hub
    {
        private static Random _random = new Random();

        private readonly ILogger<ChatHub> _logger;

        private static ConcurrentDictionary<string, User> _connectedUsers = new ConcurrentDictionary<string, User>();

        private static User adminUser = new User { Username = "admin", Color = "white" };

        public ChatHub(ILogger<ChatHub> logger)
        {
            _logger = logger;
        }

        public override Task OnConnectedAsync()
        {
            _connectedUsers[Context.ConnectionId] = new User();
            return base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var now = DateTime.Now;
            var username = _connectedUsers[Context.ConnectionId].Username;
            _logger.LogInformation($"[{now}] [ADMIN] {username} disconnected.");
            _connectedUsers.TryRemove(Context.ConnectionId, out _);
            await base.OnDisconnectedAsync(exception);

            await PushPublicMessageToAll(new PublicMessage
            {
                User = adminUser, Content = $"{username} disconnected.", Time = now
            });
        }

        public async Task NotifyAboutLogin(User user)
        {
            var now = DateTime.Now;
            _logger.LogInformation($"[{now}] [ADMIN] {user.Username} connected.");
            // bright color only
            user.Color = $"hsla({_random.Next(1,360)},100%,50%,1)";
            _connectedUsers[Context.ConnectionId] = user;

            await PushPublicMessageToAll(new PublicMessage
            {
                User = adminUser, Content = $"{user.Username} connected.", Time = now
            });
        }

        public async Task SendPublicMessage(PublicMessage message)
        {
            var now = DateTime.Now;
            _logger.LogInformation($"[{now}] [PUBLIC] {message.User.Username}: {message.Content}");
            await PushPublicMessageToAll(new PublicMessage
            {
                User = _connectedUsers[Context.ConnectionId], Content = message.Content, Time = now
            });
        }

        private Task PushPublicMessageToAll(PublicMessage message)
            => Clients.All.SendAsync("ReceivePublicMessage", message);
    }
}
