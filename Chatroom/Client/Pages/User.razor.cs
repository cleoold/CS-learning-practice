using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using Chatroom.Shared;
using SUser = Chatroom.Shared.User;

namespace Chatroom.Pages
{
    public partial class User : ComponentBase, IDisposable
    {
        [Inject]
        private NavigationManager navigationManager { get; set; }

        [Inject]
        private IJSRuntime js { get; set; }

        [Parameter]
        public string username { get; set; } = null!;

        private SUser user { get => new SUser { Username = username }; }

        private HubConnection hubConnection;

        private List<PublicMessage> history = new List<PublicMessage>();

        private List<SUser> allusers = new List<SUser>();

        private string messageInput = null!;

        protected override async Task OnInitializedAsync()
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl(navigationManager.ToAbsoluteUri("/chathub"))
                .Build();

            RegisterHubEvents();

            await hubConnection.StartAsync();

            await hubConnection.SendAsync("NotifyAboutLogin", user);

            await js.InvokeVoidAsync("chatroom.setTitle", $"@{username} - chatroom");
        }

        private void RegisterHubEvents()
        {
            hubConnection.On<PublicMessage>("ReceivePublicMessage", async message =>
            {
                history.Add(message);
                StateHasChanged();

                if (message.IsOwn || await IsLastMessageVisible())
                    await ScrollToBottom();
            });

            hubConnection.On<List<SUser>>("ReceiveUserList", users =>
            {
                allusers = users;
                StateHasChanged();
            });
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

        public ValueTask ScrollToBottom()
            => js.InvokeVoidAsync("chatroom.scrollIntoView", ".message-list li:last-child");

        public ValueTask<bool> IsLastMessageVisible()
            => js.InvokeAsync<bool>("chatroom.isScrolledIntoView", ".message-list", ".message-list li:nth-last-child(2)");

        public bool IsConnected => hubConnection.State == HubConnectionState.Connected;

        public void Dispose() => hubConnection.DisposeAsync();
    }
}
