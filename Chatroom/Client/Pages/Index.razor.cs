using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.SignalR.Client;
using Chatroom.Shared;

namespace Chatroom.Pages
{
    public partial class Index : ComponentBase, IDisposable
    {
        [Inject]
        private NavigationManager navigationManager { get; set; }

        private HubConnection hubConnection;

        private List<PublicMessage> history = new List<PublicMessage>();
        private string userInput = null!;
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
        }

        public Task Send()
        {
            messageInput = "";
            return hubConnection.SendAsync("SendPublicMessage", new PublicMessage
            {
                User = userInput, Content = messageInput, Time = DateTime.Now
            });
        }

        public bool IsConnected => hubConnection.State == HubConnectionState.Connected;

        public void Dispose() => hubConnection.DisposeAsync();
    }
}
