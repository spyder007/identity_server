using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;

using Microsoft.EntityFrameworkCore;

using Spydersoft.Identity.Models.Admin.ApiResourceViewModels;

namespace Spydersoft.Identity.Controllers.Admin.Api
{
    public class ApiResourceScopesController(ConfigurationDbContext context, IMapper mapper) : BaseApiResourceCollectionController<ApiResourceScopeViewModel, ApiResourceScopesViewModel, ApiResourceScope>(context, mapper)
    {

        #region BaseApiCollectionController Implementation

        protected override IEnumerable<ApiResourceScopeViewModel> PopulateItemList(ApiResource mainEntity)
        {
            return Mapper.ProjectTo<ApiResourceScopeViewModel>(mainEntity.Scopes.AsQueryable());
        }

        protected override IQueryable<ApiResource> AddIncludes(DbSet<ApiResource> query)
        {
            return query.Include(c => c.Scopes);
        }

        protected override ApiResourceScope FindItemInCollection(List<ApiResourceScope> collection, int id)
        {
            return collection.Find(s => s.Id == id);
        }

        protected override List<ApiResourceScope> GetCollection(ApiResource mainEntity)
        {
            return mainEntity.Scopes;
        }

        #endregion BaseApiCollectionController Implementation
    }
}