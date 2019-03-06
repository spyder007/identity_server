using AutoMapper.QueryableExtensions;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using one.Identity.Models.IdentityResourceViewModels;
using System.Collections.Generic;
using System.Linq;

namespace one.Identity.Controllers.Admin.Identity
{
    public class IdentityResourcePropertiesController : BaseIdentityResourceCollectionController<IdentityResourcePropertyViewModel, IdentityResourcePropertiesViewModel, IdentityResourceProperty>
    {
        public IdentityResourcePropertiesController(ConfigurationDbContext context) : base(context)
        {
        }

        #region BaseIdentityResourceCollectionController Implementation

        protected override IEnumerable<IdentityResourcePropertyViewModel> PopulateItemList(IdentityResource mainEntity)
        {
            return mainEntity.Properties.AsQueryable().ProjectTo<IdentityResourcePropertyViewModel>();
        }

        protected override IIncludableQueryable<IdentityResource, List<IdentityResourceProperty>> AddIncludes(DbSet<IdentityResource> query)
        {
            return query.Include(ir => ir.Properties);
        }

        protected override void RemoveObject(IdentityResource mainEntity, int id)
        {
            var prop = mainEntity.Properties.FirstOrDefault(p => p.Id == id);
            mainEntity.Properties.Remove(prop);
        }

        protected override void AddObject(IdentityResource mainEntity, int parentId, IdentityResourcePropertyViewModel newItem)
        {
            mainEntity.Properties.Add(new IdentityResourceProperty()
            {
                IdentityResourceId = parentId,
                Key = newItem.Key,
                Value = newItem.Value
            });
        }

        #endregion BaseIdentityResourceCollectionController Implementation
    }
}