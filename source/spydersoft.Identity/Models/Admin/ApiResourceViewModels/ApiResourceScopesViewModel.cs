using Duende.IdentityServer.EntityFramework.DbContexts;
using Google.Protobuf.WellKnownTypes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace spydersoft.Identity.Models.Admin.ApiResourceViewModels
{
    public class ApiResourceScopesViewModel : BaseApiResourceCollectionViewModel<ApiResourceScopeViewModel>
    {
        public override ApiResourceScopeViewModel GetChild(ApiResourceViewModel parent, ConfigurationDbContext configDbContext)
        {
            var child = base.GetChild(parent, configDbContext);
            child.Scopes = configDbContext.ApiScopes.Select(scope => scope.Name).ToList();

            var api = configDbContext.ApiResources.Include(api => api.Scopes).FirstOrDefault(api => api.Id == parent.Id);
            foreach (var scope in api.Scopes)
            {
                child.Scopes.Remove(scope.Scope);
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

