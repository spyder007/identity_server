using System.Collections.Generic;
using System.Linq;

using Duende.IdentityServer.EntityFramework.DbContexts;

using Microsoft.EntityFrameworkCore;

namespace Spydersoft.Identity.Models.Admin.ApiResourceViewModels
{
    /// <summary>
    /// Class ApiResourceScopesViewModel.
    /// Implements the <see cref="BaseApiResourceCollectionViewModel{T}" />
    /// </summary>
    /// <seealso cref="BaseApiResourceCollectionViewModel{T}" />
    public class ApiResourceScopesViewModel : BaseApiResourceCollectionViewModel<ApiResourceScopeViewModel>
    {
        /// <summary>
        /// Gets the child.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="configDbContext">The configuration database context.</param>
        /// <returns>Spydersoft.Identity.Models.Admin.ApiResourceViewModels.ApiResourceScopeViewModel.</returns>
        public override ApiResourceScopeViewModel GetChild(ApiResourceViewModel parent, ConfigurationDbContext configDbContext)
        {
            ApiResourceScopeViewModel child = base.GetChild(parent, configDbContext);
            child.Scopes = [.. configDbContext.ApiScopes.Select(scope => scope.Name)];

            Duende.IdentityServer.EntityFramework.Entities.ApiResource api = configDbContext.ApiResources.Include(api => api.Scopes).FirstOrDefault(api => api.Id == parent.Id);
            if (api != null)
            {
                foreach (Duende.IdentityServer.EntityFramework.Entities.ApiResourceScope scope in api.Scopes)
                {
                    _ = child.Scopes.Remove(scope.Scope);
                }
            }

            return child;
        }
    }

    /// <summary>
    /// Class ApiResourceScopeViewModel.
    /// Implements the <see cref="BaseAdminChildItemViewModel" />
    /// </summary>
    /// <seealso cref="BaseAdminChildItemViewModel" />
    public class ApiResourceScopeViewModel : BaseAdminChildItemViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResourceScopeViewModel"/> class.
        /// </summary>
        public ApiResourceScopeViewModel()
        {
        }

        /// <summary>
        /// Gets or sets the scope.
        /// </summary>
        /// <value>The scope.</value>
        public string Scope { get; set; }

        /// <summary>
        /// Gets or sets the scopes.
        /// </summary>
        /// <value>The scopes.</value>
        public List<string> Scopes { get; set; }
    }


}