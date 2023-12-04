using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using Duende.IdentityServer;
using Duende.IdentityServer.EntityFramework.DbContexts;

using Microsoft.EntityFrameworkCore;

namespace Spydersoft.Identity.Models.Admin.ClientViewModels
{
    public class ClientScopesViewModel : BaseClientCollectionViewModel<ClientScopeViewModel>
    {
        public override ClientScopeViewModel GetChild(ClientViewModel parent, ConfigurationDbContext configDbContext)
        {
            ClientScopeViewModel child = base.GetChild(parent, configDbContext);
            child.Scopes = configDbContext.ApiScopes.Select(scope => scope.Name).ToList();

            child.Scopes.Add(IdentityServerConstants.StandardScopes.Address);
            child.Scopes.Add(IdentityServerConstants.StandardScopes.Email);
            child.Scopes.Add(IdentityServerConstants.StandardScopes.OfflineAccess);
            child.Scopes.Add(IdentityServerConstants.StandardScopes.OpenId);
            child.Scopes.Add(IdentityServerConstants.StandardScopes.Phone);
            child.Scopes.Add(IdentityServerConstants.StandardScopes.Profile);

            Duende.IdentityServer.EntityFramework.Entities.Client client = configDbContext.Clients.Include(c => c.AllowedScopes).FirstOrDefault(c => c.Id == parent.Id);
            if (client != null)
            {
                foreach (Duende.IdentityServer.EntityFramework.Entities.ClientScope scope in client.AllowedScopes)
                {
                    _ = child.Scopes.Remove(scope.Scope);
                }
            }

            return child;
        }
    }

    public class ClientScopeViewModel : BaseAdminChildItemViewModel
    {
        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 2)]
        [Display(Name = "Scope")]
        public string Scope { get; set; }

        public List<string> Scopes { get; set; }
    }
}