using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;

using Microsoft.EntityFrameworkCore;

using Spydersoft.Identity.Models.Admin.IdentityResourceViewModels;

namespace Spydersoft.Identity.Controllers.Admin.Identity
{
    /// <summary>
    /// Class IdentityResourceClaimsController.
    /// Implements the <see cref="BaseIdentityResourceCollectionController{TSingleViewModel, TCollectionViewModel, TChildEntity}" />
    /// </summary>
    /// <seealso cref="BaseIdentityResourceCollectionController{TSingleViewModel, TCollectionViewModel, TChildEntity}" />
    public class IdentityResourceClaimsController(ConfigurationDbContext context, IMapper mapper) : BaseIdentityResourceCollectionController<IdentityResourceClaimViewModel, IdentityResourceClaimsViewModel, IdentityResourceClaim>(context, mapper)
    {

        #region BaseIdentityResourceCollectionController Implementation

        /// <summary>
        /// Populates the item list.
        /// </summary>
        /// <param name="mainEntity">The main entity.</param>
        /// <returns>IEnumerable&lt;TSingleViewModel&gt;.</returns>
        protected override IEnumerable<IdentityResourceClaimViewModel> PopulateItemList(IdentityResource mainEntity)
        {
            return Mapper.ProjectTo<IdentityResourceClaimViewModel>(mainEntity.UserClaims.AsQueryable());
        }

        /// <summary>
        /// Adds the includes.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>IQueryable&lt;IS4Entities.IdentityResource&gt;.</returns>
        protected override IQueryable<IdentityResource> AddIncludes(DbSet<IdentityResource> query)
        {
            return query.Include(ir => ir.UserClaims);
        }

        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <param name="mainEntity">The main entity.</param>
        /// <returns>List&lt;IdentityResourceClaim&gt;.</returns>
        protected override List<IdentityResourceClaim> GetCollection(IdentityResource mainEntity)
        {
            return mainEntity.UserClaims;
        }

        /// <summary>
        /// Finds the item in collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>IdentityResourceClaim.</returns>
        protected override IdentityResourceClaim FindItemInCollection(List<IdentityResourceClaim> collection, int id)
        {
            return collection.Find(ic => ic.Id == id);
        }

        #endregion BaseIdentityResourceCollectionController Implementation
    }
}