using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Duende.IdentityServer.Events;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Spydersoft.Identity.Models.Grants;

namespace Spydersoft.Identity.Pages.Grants
{
    /// <summary>Lets a user review and revoke grants given to clients.</summary>
    public class IndexModel(
        IIdentityServerInteractionService interaction,
        IClientStore clients,
        IResourceStore resources,
        IEventService events) : PageModel
    {
        /// <summary>The grants display model.</summary>
        public GrantsViewModel View { get; set; }

        /// <summary>Lists the user's grants.</summary>
        public async Task<IActionResult> OnGetAsync()
        {
            View = await BuildViewModelAsync();
            return Page();
        }

        /// <summary>Revokes the user's consent for a client.</summary>
        public async Task<IActionResult> OnPostRevokeAsync(string clientId)
        {
            await interaction.RevokeUserConsentAsync(clientId);
            await events.RaiseAsync(new GrantsRevokedEvent(User.GetSubjectId(), clientId));
            return RedirectToPage();
        }

        private async Task<GrantsViewModel> BuildViewModelAsync()
        {
            IEnumerable<Duende.IdentityServer.Models.Grant> grants = await interaction.GetAllUserGrantsAsync();

            var list = new List<GrantViewModel>();
            foreach (Duende.IdentityServer.Models.Grant grant in grants)
            {
                Duende.IdentityServer.Models.Client client = await clients.FindClientByIdAsync(grant.ClientId);
                if (client != null)
                {
                    Duende.IdentityServer.Models.Resources resourcesByScope = await resources.FindResourcesByScopeAsync(grant.Scopes);

                    list.Add(new GrantViewModel
                    {
                        ClientId = client.ClientId,
                        ClientName = client.ClientName ?? client.ClientId,
                        ClientLogoUrl = client.LogoUri,
                        ClientUrl = client.ClientUri,
                        Description = grant.Description,
                        Created = grant.CreationTime,
                        Expires = grant.Expiration,
                        IdentityGrantNames = [.. resourcesByScope.IdentityResources.Select(x => x.DisplayName ?? x.Name)],
                        ApiGrantNames = [.. resourcesByScope.ApiScopes.Select(x => x.DisplayName ?? x.Name)]
                    });
                }
            }

            return new GrantsViewModel { Grants = list };
        }
    }
}