using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;

using Microsoft.EntityFrameworkCore;

using Spydersoft.Identity.Models.Admin.ScopeViewModels;

namespace Spydersoft.Identity.Controllers.Admin.Scope
{
    /// <summary>
    /// Class ScopeClaimsController.
    /// Implements the <see cref="Scope.BaseScopeCollectionController{Models.Admin.ScopeViewModels.ScopeClaimViewModel, Models.Admin.ScopeViewModels.ScopeClaimsViewModel, Duende.IdentityServer.EntityFramework.Entities.ApiScopeClaim}" />
    /// </summary>
    /// <seealso cref="Scope.BaseScopeCollectionController{Models.Admin.ScopeViewModels.ScopeClaimViewModel, Models.Admin.ScopeViewModels.ScopeClaimsViewModel, Duende.IdentityServer.EntityFramework.Entities.ApiScopeClaim}" />
    public class ScopeClaimsController(ConfigurationDbContext context, IMapper mapper) : BaseScopeCollectionController<ScopeClaimViewModel, ScopeClaimsViewModel, ApiScopeClaim>(context, mapper)
    {

        #region BaseApiCollectionController Implementation

        /// <summary>
        /// Populates the item list.
        /// </summary>
        /// <param name="mainEntity">The main entity.</param>
        /// <returns>IEnumerable&lt;TSingleViewModel&gt;.</returns>
        protected override IEnumerable<ScopeClaimViewModel> PopulateItemList(ApiScope mainEntity)
        {
            return Mapper.ProjectTo<ScopeClaimViewModel>(mainEntity.UserClaims.AsQueryable());
        }

        /// <summary>
        /// Adds the includes.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>IQueryable&lt;IS4Entities.ApiScope&gt;.</returns>
        protected override IQueryable<ApiScope> AddIncludes(DbSet<ApiScope> query)
        {
            return query.Include(c => c.UserClaims);
        }

        /// <summary>
        /// Finds the item in collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>ApiScopeClaim.</returns>
        protected override ApiScopeClaim FindItemInCollection(List<ApiScopeClaim> collection, int id)
        {
            return collection.Find(c => c.Id == id);
        }

        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <param name="mainEntity">The main entity.</param>
        /// <returns>List&lt;ApiScopeClaim&gt;.</returns>
        protected override List<ApiScopeClaim> GetCollection(ApiScope mainEntity)
        {
            return mainEntity.UserClaims;
        }

        #endregion BaseApiCollectionController Implementation
    }
}