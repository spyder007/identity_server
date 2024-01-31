using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;

using Microsoft.EntityFrameworkCore;

using Spydersoft.Identity.Models.Admin.ScopeViewModels;

namespace Spydersoft.Identity.Controllers.Admin.Scope
{
    public class ScopeClaimsController(ConfigurationDbContext context, IMapper mapper) : BaseScopeCollectionController<ScopeClaimViewModel, ScopeClaimsViewModel, ApiScopeClaim>(context, mapper)
    {

        #region BaseApiCollectionController Implementation

        protected override IEnumerable<ScopeClaimViewModel> PopulateItemList(ApiScope mainEntity)
        {
            return Mapper.ProjectTo<ScopeClaimViewModel>(mainEntity.UserClaims.AsQueryable());
        }

        protected override IQueryable<ApiScope> AddIncludes(DbSet<ApiScope> query)
        {
            return query.Include(c => c.UserClaims);
        }

        protected override ApiScopeClaim FindItemInCollection(List<ApiScopeClaim> collection, int id)
        {
            return collection.Find(c => c.Id == id);
        }

        protected override List<ApiScopeClaim> GetCollection(ApiScope mainEntity)
        {
            return mainEntity.UserClaims;
        }

        #endregion BaseApiCollectionController Implementation
    }
}