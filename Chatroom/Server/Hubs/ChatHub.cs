using System;
using System.Linq;
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

        private static ConcurrentDictionary<string, User> _connectedUsers = new ConcurrentDictionary<string, User>();

        private static User adminUser = new User { Username = "admin", Color = "white" };

        private readonly ILogger<ChatHub> _logger;

        private DateTime utcNow { get => DateTime.Now.ToUniversalTime(); }

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
            var now = utcNow;
            var username = _connectedUsers[Context.ConnectionId].Username;
            _logger.LogInformation($"[{now}] [ADMIN] {username} disconnected.");
            _connectedUsers.TryRemove(Context.ConnectionId, out _);
            await base.OnDisconnectedAsync(exception);

            await ReceivePublicMessage(new PublicMessage
            {
                User = adminUser, Content = $"{username} disconnected.", Time = now
            });

            await ReceiveUserList();
        }

        public async Task NotifyAboutLogin(User user)
        {
            var now = utcNow;
            _logger.LogInformation($"[{now}] [ADMIN] {user.Username} connected.");
            // bright color only
            user.Color = $"hsla({_random.Next(1,360)},100%,50%,1)";
            _connectedUsers[Context.ConnectionId] = user;

            await ReceivePublicMessage(new PublicMessage
            {
                User = adminUser, Content = $"{user.Username} connected.", Time = now
            });

            await ReceiveUserList();
        }

        public async Task SendPublicMessage(PublicMessage message)
        {
            var now = utcNow;
            _logger.LogInformation($"[{now}] [PUBLIC] {message.User.Username}: {message.Content}");
            await ReceivePublicMessage(new PublicMessage
            {
                User = _connectedUsers[Context.ConnectionId], Content = message.Content, Time = now
            });
        }

        public Task ReceivePublicMessage(PublicMessage message)
            => Clients.All.SendAsync("ReceivePublicMessage", message);

        public Task ReceiveUserList()
            => Clients.All.SendAsync("ReceiveUserList", _connectedUsers.Values.ToList());
    }
}
