using AutoMapper.QueryableExtensions;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using spydersoft.Identity.Models.Admin.ScopeViewModels;

namespace spydersoft.Identity.Controllers.Admin.Scope
{
    public class ScopeClaimsController : BaseScopeCollectionController<ScopeClaimViewModel, ScopeClaimsViewModel, ApiScopeClaim>
    {
        public ScopeClaimsController(ConfigurationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        #region BaseApiCollectionController Implementation

        protected override IEnumerable<ScopeClaimViewModel> PopulateItemList(ApiScope mainEntity)
        {
            return Mapper.ProjectTo<ScopeClaimViewModel>(mainEntity.UserClaims.ToList().AsQueryable());
        }

        protected override IQueryable<ApiScope> AddIncludes(DbSet<ApiScope> query)
        {
            return query.Include(c => c.UserClaims);
        }

        protected override ApiScopeClaim FindItemInCollection(List<ApiScopeClaim> collection, int id)
        {
            return collection.FirstOrDefault(c => c.Id == id);
        }

        protected override List<ApiScopeClaim> GetCollection(ApiScope mainEntity)
        {
            return mainEntity.UserClaims;
        }

        #endregion BaseApiCollectionController Implementation
    }
}