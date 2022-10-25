using Duende.IdentityServer;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace spydersoft.Identity.Models.Admin.ClientViewModels
{
    public class ClientScopesViewModel : BaseClientCollectionViewModel<ClientScopeViewModel>
    {
        public override ClientScopeViewModel GetChild(ClientViewModel parent, ConfigurationDbContext configDbContext)
        {
            var child = base.GetChild(parent, configDbContext);
            child.Scopes = configDbContext.ApiScopes.Select(scope => scope.Name).ToList();
            
            child.Scopes.Add(IdentityServerConstants.StandardScopes.Address);
            child.Scopes.Add(IdentityServerConstants.StandardScopes.Email);
            child.Scopes.Add(IdentityServerConstants.StandardScopes.OfflineAccess);
            child.Scopes.Add(IdentityServerConstants.StandardScopes.OpenId);
            child.Scopes.Add(IdentityServerConstants.StandardScopes.Phone);
            child.Scopes.Add(IdentityServerConstants.StandardScopes.Profile);

            var client = configDbContext.Clients.Include(c => c.AllowedScopes).FirstOrDefault(c => c.Id == parent.Id);
            if (client != null)
            {
                foreach (var scope in client.AllowedScopes)
                {
                    child.Scopes.Remove(scope.Scope);
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

        public List<string> Scopes { get; set;  }
    }
}
