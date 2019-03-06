using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using one.Identity.Models.ApiViewModels;

namespace one.Identity.Controllers.Admin.Api
{
    public class ApiPropertiesController : BaseApiCollectionController<ApiPropertyViewModel, ApiPropertiesViewModel, ApiResourceProperty>
    {
        public ApiPropertiesController(ConfigurationDbContext context) : base(context)
        {
        }

        protected override IEnumerable<ApiPropertyViewModel> PopulateItemList(ApiResource mainEntity)
        {
            return mainEntity.Properties.AsQueryable().ProjectTo<ApiPropertyViewModel>();
        }

        protected override IIncludableQueryable<ApiResource, List<ApiResourceProperty>> AddIncludes(DbSet<ApiResource> query)
        {
            return query.Include(api => api.Properties);
        }

        protected override void RemoveObject(ApiResource mainEntity, int id)
        {
            var prop = mainEntity.Properties.FirstOrDefault(p => p.Id == id);
            mainEntity.Properties.Remove(prop);
        }

        protected override void AddObject(ApiResource mainEntity, int parentId, ApiPropertyViewModel newItem)
        {
            ApiResourceProperty newProp = Mapper.Map<ApiResourceProperty>(newItem);
            newProp.ApiResourceId = parentId;
            mainEntity.Properties.Add(newProp);
        }
    }
}
