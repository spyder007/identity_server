using System.Collections.Generic;
using System.Linq;

using Duende.IdentityServer.EntityFramework.DbContexts;

using Microsoft.EntityFrameworkCore;

namespace Spydersoft.Identity.Models.Admin.ApiResourceViewModels
{
    public class ApiResourceScopesViewModel : BaseApiResourceCollectionViewModel<ApiResourceScopeViewModel>
    {
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

    public class ApiResourceScopeViewModel : BaseAdminChildItemViewModel
    {
        public ApiResourceScopeViewModel()
        {
        }

        public string Scope { get; set; }

        public List<string> Scopes { get; set; }
    }


}