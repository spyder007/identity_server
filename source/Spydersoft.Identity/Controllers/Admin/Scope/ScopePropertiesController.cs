using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;

using Microsoft.EntityFrameworkCore;

using Spydersoft.Identity.Models.Admin.ScopeViewModels;

namespace Spydersoft.Identity.Controllers.Admin.Scope
{
    public class ScopePropertiesController(ConfigurationDbContext context, IMapper mapper) : BaseScopeCollectionController<ScopePropertyViewModel, ScopePropertiesViewModel, ApiScopeProperty>(context, mapper)
    {

        #region BaseApiCollectionController Implementation

        protected override IEnumerable<ScopePropertyViewModel> PopulateItemList(ApiScope mainEntity)
        {
            return Mapper.ProjectTo<ScopePropertyViewModel>(mainEntity.Properties.AsQueryable());
        }

        protected override IQueryable<ApiScope> AddIncludes(DbSet<ApiScope> query)
        {
            return query.Include(api => api.Properties);
        }

        protected override ApiScopeProperty FindItemInCollection(List<ApiScopeProperty> collection, int id)
        {
            return collection.Find(c => c.Id == id);
        }

        protected override List<ApiScopeProperty> GetCollection(ApiScope mainEntity)
        {
            return mainEntity.Properties;
        }

        #endregion BaseApiCollectionController Implementation
    }
}