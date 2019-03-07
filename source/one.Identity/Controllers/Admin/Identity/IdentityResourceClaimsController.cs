using AutoMapper;
using AutoMapper.QueryableExtensions;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using one.Identity.Models.IdentityResourceViewModels;
using System.Collections.Generic;
using System.Linq;

namespace one.Identity.Controllers.Admin.Identity.IdentityResourceViewModels
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

        protected override void RemoveObject(IdentityResource mainEntity, int id)
        {
            var claim = mainEntity.UserClaims.FirstOrDefault(uc => uc.Id == id);
            mainEntity.UserClaims.Remove(claim);
        }

        protected override void AddObject(IdentityResource mainEntity, int parentId, IdentityResourceClaimViewModel newItem)
        {
            IdentityClaim ic = Mapper.Map<IdentityClaim>(newItem);
            ic.IdentityResourceId = parentId;
            mainEntity.UserClaims.Add(ic);
        }

        #endregion BaseIdentityResourceCollectionController Implementation
    }
}