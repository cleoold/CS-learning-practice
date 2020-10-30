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
    internal class RetryPolicy : IRetryPolicy
    {
        private int count = 0;
        private static TimeSpan span = TimeSpan.FromSeconds(5);
        public TimeSpan? NextRetryDelay(RetryContext ctx)
        {
            if (count++ < 10)
                return span;
            return null;
        }
    }

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

        private bool HaveUnreadNewMessages = false;

        private bool MobileOnlyUserListSwitchedIn = false;

        protected override async Task OnInitializedAsync()
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl(navigationManager.ToAbsoluteUri("/chathub"))
                .WithAutomaticReconnect(new RetryPolicy())
                .Build();

            RegisterHubEvents();

            await hubConnection.StartAsync();

            await hubConnection.SendAsync("NotifyAboutLogin", user);

            await js.InvokeVoidAsync("chatroom.setTitle", $"@{username} - chatroom");
        }

        private void RegisterHubEvents()
        {
            hubConnection.Reconnected += _ => InvokeAsync(StateHasChanged);
            hubConnection.Reconnecting += _ => InvokeAsync(StateHasChanged);
            hubConnection.Closed += _ => InvokeAsync(StateHasChanged);

            hubConnection.On<List<PublicMessage>>("ReceiveLastPublicMessages", messages =>
            {
                history.InsertRange(history.Count, messages);
                StateHasChanged();
            });

            hubConnection.On<PublicMessage>("ReceivePublicMessage", async message =>
            {
                history.Add(message);
                StateHasChanged();

                if (message.IsOwn || await IsLastMessageVisible)
                {
                    await ScrollToBottom();
                }
                else
                {
                    HaveUnreadNewMessages = true;
                    StateHasChanged();
                }
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

        public ValueTask<bool> IsLastMessageVisible
            => js.InvokeAsync<bool>("chatroom.isScrolledIntoView", ".message-list", ".message-list li:nth-last-child(2)");

        public bool IsConnected => hubConnection.State == HubConnectionState.Connected;

        public async Task ClickUnreadNewMessageFlyNotice()
        {
            if (!HaveUnreadNewMessages) // CS0414
                return;
            await ScrollToBottom();
            HaveUnreadNewMessages = false;
            StateHasChanged();
        }

        public ValueTask<bool> IsMobile => js.InvokeAsync<bool>("chatroom.isMobile");

        public void MobileOnlySwitchUserList()
        {
            MobileOnlyUserListSwitchedIn = !MobileOnlyUserListSwitchedIn;
            StateHasChanged();
        }

        public void Dispose() => hubConnection.DisposeAsync();
    }
}
