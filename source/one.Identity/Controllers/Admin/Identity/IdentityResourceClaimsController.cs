using AutoMapper.QueryableExtensions;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Collections.Generic;
using System.Linq;
using one.Identity.Models.Admin.IdentityResourceViewModels;

namespace one.Identity.Controllers.Admin.Identity
{
    public class IdentityResourceClaimsController : BaseIdentityResourceCollectionController<IdentityResourceClaimViewModel, IdentityResourceClaimsViewModel, IdentityClaim>
    {
        public IdentityResourceClaimsController(ConfigurationDbContext context) : base(context)
        {
        }

        #region BaseIdentityResourceCollectionController Implementation

        protected override IEnumerable<IdentityResourceClaimViewModel> PopulateItemList(IdentityResource mainEntity)
        {
            return mainEntity.UserClaims.AsQueryable().ProjectTo<IdentityResourceClaimViewModel>();
        }

        protected override IIncludableQueryable<IdentityResource, List<IdentityClaim>> AddIncludes(DbSet<IdentityResource> query)
        {
            return query.Include(ir => ir.UserClaims);
        }

        protected override List<IdentityClaim> GetCollection(IdentityResource mainEntity)
        {
            return mainEntity.UserClaims;
        }

        protected override IdentityClaim FindItemInCollection(List<IdentityClaim> collection, int id)
        {
            return collection.FirstOrDefault(ic => ic.Id == id);
        }

        #endregion BaseIdentityResourceCollectionController Implementation
    }
}