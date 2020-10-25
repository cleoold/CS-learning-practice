using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Chatroom.Shared;
using SUser = Chatroom.Shared.User;

namespace Chatroom.Pages
{
    public partial class User : ComponentBase, IDisposable
    {
        [Inject]
        private NavigationManager navigationManager { get; set; }

        [Parameter]
        public string username { get; set; } = null!;

        private SUser user { get => new SUser { Username = username }; }

        private HubConnection hubConnection;

        private List<PublicMessage> history = new List<PublicMessage>();

        private string messageInput = null!;

        protected override async Task OnInitializedAsync()
        {

            hubConnection = new HubConnectionBuilder()
                .WithUrl(navigationManager.ToAbsoluteUri("/chathub"))
                .Build();

            hubConnection.On<PublicMessage>("ReceivePublicMessage", message =>
            {
                history.Add(message);
                StateHasChanged();
            });

            await hubConnection.StartAsync();

            await hubConnection.SendAsync("NotifyAboutLogin", user);
        }

        public async Task Send()
        {
            if (String.IsNullOrWhiteSpace(messageInput))
                return;

            await hubConnection.SendAsync("SendPublicMessage", new PublicMessage
            {
                User = user, Content = messageInput
            });

            messageInput = "";
        }

        public bool IsConnected => hubConnection.State == HubConnectionState.Connected;

        public void Dispose() => hubConnection.DisposeAsync();
    }
}
