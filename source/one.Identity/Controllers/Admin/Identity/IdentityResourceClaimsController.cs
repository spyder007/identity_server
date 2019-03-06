using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using one.Identity.Controllers.Admin.Identity;
using one.Identity.Models.IdentityResourceViewModels;

namespace one.Identity.Controllers.Admin.Identity.IdentityResourceViewModels
{
    public class IdentityResourceClaimsController : BaseIdentityResourceCollectionController<IdentityResourceClaimViewModel, IdentityResourceClaimsViewModel, IdentityClaim>
    {
        public IdentityResourceClaimsController(ConfigurationDbContext context) : base(context)
        {
        }

        protected override IEnumerable<IdentityResourceClaimViewModel> PopulateItemList(IdentityResource identityResource)
        {
            return identityResource.UserClaims.AsQueryable().ProjectTo<IdentityResourceClaimViewModel>();
        }

        protected override IIncludableQueryable<IdentityResource, List<IdentityClaim>> AddIncludes(DbSet<IdentityResource> query)
        {
            return query.Include(ir => ir.UserClaims);
        }

        protected override void RemoveObject(IdentityResource identityResource, int id)
        {
            var claim = identityResource.UserClaims.FirstOrDefault(uc => uc.Id == id);
            identityResource.UserClaims.Remove(claim);
        }

        protected override void AddObject(IdentityResource identityResource, int identityResourceId, IdentityResourceClaimViewModel newItem)
        {
            identityResource.UserClaims.Add(new IdentityClaim()
            {
                IdentityResourceId = identityResourceId,
                Type = newItem.Type
            });
        }
    }
}
