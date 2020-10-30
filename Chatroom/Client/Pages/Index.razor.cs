using Microsoft.AspNetCore.Components;

namespace Chatroom.Pages
{
    public partial class Index : ComponentBase
    {
        [Inject]
        private NavigationManager navigationManager { get; set; }

        private string usernameInput = null!;

        private bool showUsernameInvalidMessage = false;

        private void TryGotoChatroom()
        {
            if (string.IsNullOrWhiteSpace(usernameInput))
            {
                if (showUsernameInvalidMessage) // CS0414
                    return;
                showUsernameInvalidMessage = true;
                StateHasChanged();
            }
            else
            {
                navigationManager.NavigateTo($"/User/{usernameInput}");
            }
        }
    }
}
