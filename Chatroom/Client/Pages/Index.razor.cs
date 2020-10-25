using Microsoft.AspNetCore.Components;

namespace Chatroom.Pages
{
    public partial class Index : ComponentBase
    {
        [Inject]
        private NavigationManager navigationManager { get; set; }

        private string usernameInput = null!;

        private void GotoChatroom()
        {
            if (!string.IsNullOrWhiteSpace(usernameInput))
                navigationManager.NavigateTo($"/User/{usernameInput}");
        }
    }
}
