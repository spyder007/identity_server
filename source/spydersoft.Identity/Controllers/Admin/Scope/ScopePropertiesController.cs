using AutoMapper.QueryableExtensions;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using spydersoft.Identity.Models.Admin.ApiViewModels;
using spydersoft.Identity.Models.Admin.ScopeViewModels;

namespace spydersoft.Identity.Controllers.Admin.Scope
{
    public class ScopePropertiesController : BaseScopeCollectionController<ScopePropertyViewModel, ScopePropertiesViewModel, ApiScopeProperty>
    {
        public ScopePropertiesController(ConfigurationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        #region BaseApiCollectionController Implementation

        protected override IEnumerable<ScopePropertyViewModel> PopulateItemList(ApiScope mainEntity)
        { 
            return Mapper.ProjectTo<ScopePropertyViewModel>(mainEntity.Properties.ToList().AsQueryable());
        }

        protected override IQueryable<ApiScope> AddIncludes(DbSet<ApiScope> query)
        {
            return query.Include(api => api.Properties);
        }

        protected override ApiScopeProperty FindItemInCollection(List<ApiScopeProperty> collection, int id)
        {
            return collection.FirstOrDefault(c => c.Id == id);
        }

        protected override List<ApiScopeProperty> GetCollection(ApiScope mainEntity)
        {
            return mainEntity.Properties;
        }

        #endregion BaseApiCollectionController Implementation
    }
}