using AutoMapper.QueryableExtensions;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using one.Identity.Models.IdentityResourceViewModels;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

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

        protected override List<IdentityResourceProperty> GetCollection(IdentityResource mainEntity)
        {
            return mainEntity.Properties;
        }

        protected override IdentityResourceProperty FindItemInCollection(List<IdentityResourceProperty> collection, int id)
        {
            return collection.FirstOrDefault(prop => prop.Id == id);
        }

        #endregion BaseIdentityResourceCollectionController Implementation
    }
}