using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using Duende.IdentityServer;
using Duende.IdentityServer.EntityFramework.DbContexts;

using Microsoft.EntityFrameworkCore;

namespace Spydersoft.Identity.Core.Models.Admin.ClientViewModels
{
    /// <summary>
    /// Class ClientScopesViewModel.
    /// Implements the <see cref="BaseClientCollectionViewModel{TChildViewModel}" />
    /// </summary>
    /// <seealso cref="BaseClientCollectionViewModel{TChildViewModel}" />
    public class ClientScopesViewModel : BaseClientCollectionViewModel<ClientScopeViewModel>
    {
        /// <summary>
        /// Gets the child.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="configDbContext">The configuration database context.</param>
        /// <returns>Spydersoft.Identity.Models.Admin.ClientViewModels.ClientScopeViewModel.</returns>
        public override ClientScopeViewModel GetChild(ClientViewModel parent, ConfigurationDbContext configDbContext)
        {
            ClientScopeViewModel child = base.GetChild(parent, configDbContext);
            child.Scopes = [.. configDbContext.ApiScopes.Select(scope => scope.Name)];
            child.Scopes.AddRange(configDbContext.IdentityResources.Where(ir => ir.Enabled).Select(ir => ir.Name));

            // Standard OIDC scopes are kept as a fallback in case the IdentityResources
            // table hasn't been seeded yet (fresh install). Distinct below de-duplicates
            // when they're also present in the database.
            child.Scopes.Add(IdentityServerConstants.StandardScopes.Address);
            child.Scopes.Add(IdentityServerConstants.StandardScopes.Email);
            child.Scopes.Add(IdentityServerConstants.StandardScopes.OfflineAccess);
            child.Scopes.Add(IdentityServerConstants.StandardScopes.OpenId);
            child.Scopes.Add(IdentityServerConstants.StandardScopes.Phone);
            child.Scopes.Add(IdentityServerConstants.StandardScopes.Profile);

            child.Scopes = [.. child.Scopes.Distinct()];

            Duende.IdentityServer.EntityFramework.Entities.Client? client = configDbContext.Clients.Include(c => c.AllowedScopes).FirstOrDefault(c => c.Id == parent.Id);
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

    /// <summary>
    /// Class ClientScopeViewModel.
    /// Implements the <see cref="BaseAdminChildItemViewModel" />
    /// </summary>
    /// <seealso cref="BaseAdminChildItemViewModel" />
    public class ClientScopeViewModel : BaseAdminChildItemViewModel
    {
        /// <summary>
        /// Gets or sets the scope.
        /// </summary>
        /// <value>The scope.</value>
        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 2)]
        [Display(Name = "Scope")]
        public string Scope { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the scopes.
        /// </summary>
        /// <value>The scopes.</value>
        public List<string> Scopes { get; set; } = [];
    }
}