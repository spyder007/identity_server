using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using one.Identity.Models.IdentityResourceViewModels;

namespace one.Identity.Controllers.Admin.Identity
{
    public class IdentityResourcePropertiesController : BaseIdentityResourceCollectionController<IdentityResourcePropertyViewModel, IdentityResourcePropertiesViewModel, IdentityResourceProperty>
    {
        public IdentityResourcePropertiesController(ConfigurationDbContext context) : base(context)
        {
        }

        protected override IEnumerable<IdentityResourcePropertyViewModel> PopulateItemList(IdentityResource identityResource)
        {
            return identityResource.Properties.AsQueryable().ProjectTo<IdentityResourcePropertyViewModel>();
        }

        protected override IIncludableQueryable<IdentityResource, List<IdentityResourceProperty>> AddIncludes(DbSet<IdentityResource> query)
        {
            return query.Include(ir => ir.Properties);
        }

        protected override void RemoveObject(IdentityResource identityResource, int id)
        {
            var prop = identityResource.Properties.FirstOrDefault(p => p.Id == id);
            identityResource.Properties.Remove(prop);
        }

        protected override void AddObject(IdentityResource identityResource, int identityResourceId, IdentityResourcePropertyViewModel newItem)
        {
            identityResource.Properties.Add(new IdentityResourceProperty()
            {
                IdentityResourceId = identityResourceId,
                Key = newItem.Key,
                Value = newItem.Value
            });
        }
    }
}
