using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
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

        private static Queue<PublicMessage> _lastMessages = new Queue<PublicMessage>();

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
                User = adminUser, Content = $"{username} disconnected.", Time = now, IsOwn = false
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

            await Task.WhenAll(ReceiveLastPublicMessages(), ReceiveUserList());

            var mymsg = new PublicMessage
            {
                User = adminUser, Content = $"{user.Username} connected.", Time = now, IsOwn = true
            };
            var theirmsg = new PublicMessage(mymsg) { IsOwn = false };

            await Task.WhenAll(ReceivePublicMessage(mymsg, ToWhom.CLIENT), ReceivePublicMessage(theirmsg, ToWhom.OTHER));
        }

        public async Task SendPublicMessage(PublicMessage message)
        {
            var now = utcNow;
            _logger.LogInformation($"[{now}] [PUBLIC] {message.User.Username}: {message.Content}");

            var mymsg = new PublicMessage
            {
                User = _connectedUsers[Context.ConnectionId], Content = message.Content, Time = now, IsOwn = true
            };
            var theirmsg = new PublicMessage(mymsg) { IsOwn = false };

            await Task.WhenAll(ReceivePublicMessage(mymsg, ToWhom.CLIENT), ReceivePublicMessage(theirmsg, ToWhom.OTHER));

            lock (_lastMessages)
            {
                while (_lastMessages.Count > 20)
                    _lastMessages.TryDequeue(out _);
                _lastMessages.Enqueue(theirmsg);
            }
        }

        public enum ToWhom { CLIENT, OTHER, ALL }

        public Task ReceivePublicMessage(PublicMessage message, ToWhom who = ToWhom.ALL)
        {
            const string func = "ReceivePublicMessage";
            switch (who)
            {
                case ToWhom.CLIENT: return Clients.Caller.SendAsync(func, message);
                case ToWhom.OTHER: return Clients.Others.SendAsync(func, message);
                case ToWhom.ALL: default: return Clients.All.SendAsync(func, message);
            }
        }

        public Task ReceiveUserList()
            => Clients.All.SendAsync("ReceiveUserList", _connectedUsers.Values.ToList());

        public Task ReceiveLastPublicMessages()
        {
            List<PublicMessage>? messages;
            lock (_lastMessages)
                messages = _lastMessages.ToList();
            return Clients.Caller.SendAsync("ReceiveLastPublicMessages", messages);
        }
    }
}
