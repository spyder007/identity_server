using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;

using Microsoft.EntityFrameworkCore;

using spydersoft.Identity.Models.Admin.IdentityResourceViewModels;

namespace spydersoft.Identity.Controllers.Admin.Identity
{
    public class IdentityResourceClaimsController : BaseIdentityResourceCollectionController<IdentityResourceClaimViewModel, IdentityResourceClaimsViewModel, IdentityResourceClaim>
    {
        public IdentityResourceClaimsController(ConfigurationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        #region BaseIdentityResourceCollectionController Implementation

        protected override IEnumerable<IdentityResourceClaimViewModel> PopulateItemList(IdentityResource mainEntity)
        {
            return Mapper.ProjectTo<IdentityResourceClaimViewModel>(mainEntity.UserClaims.AsQueryable());
        }

        protected override IQueryable<IdentityResource> AddIncludes(DbSet<IdentityResource> query)
        {
            return query.Include(ir => ir.UserClaims);
        }

        protected override List<IdentityResourceClaim> GetCollection(IdentityResource mainEntity)
        {
            return mainEntity.UserClaims;
        }

        protected override IdentityResourceClaim FindItemInCollection(List<IdentityResourceClaim> collection, int id)
        {
            return collection.FirstOrDefault(ic => ic.Id == id);
        }

        #endregion BaseIdentityResourceCollectionController Implementation
    }
}