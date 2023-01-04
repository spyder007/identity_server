using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;

using Microsoft.EntityFrameworkCore;

using spydersoft.Identity.Models.Admin.ApiResourceViewModels;

namespace spydersoft.Identity.Controllers.Admin.Api
{
    public class ApiResourcePropertiesController : BaseApiResourceCollectionController<ApiResourcePropertyViewModel, ApiResourcePropertiesViewModel, ApiResourceProperty>
    {
        public ApiResourcePropertiesController(ConfigurationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        #region BaseApiCollectionController Implementation

        protected override IEnumerable<ApiResourcePropertyViewModel> PopulateItemList(ApiResource mainEntity)
        {
            return Mapper.ProjectTo<ApiResourcePropertyViewModel>(mainEntity.Properties.AsQueryable());
        }

        protected override IQueryable<ApiResource> AddIncludes(DbSet<ApiResource> query)
        {
            return query.Include(api => api.Properties);
        }

        protected override ApiResourceProperty FindItemInCollection(List<ApiResourceProperty> collection, int id)
        {
            return collection.FirstOrDefault(c => c.Id == id);
        }

        protected override List<ApiResourceProperty> GetCollection(ApiResource mainEntity)
        {
            return mainEntity.Properties;
        }

        #endregion BaseApiCollectionController Implementation
    }
}