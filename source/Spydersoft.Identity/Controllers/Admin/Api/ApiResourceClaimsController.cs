using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;

using Microsoft.EntityFrameworkCore;

using Spydersoft.Identity.Models.Admin.ApiResourceViewModels;

namespace Spydersoft.Identity.Controllers.Admin.Api
{
    /// <summary>
    /// Class ApiResourceClaimsController.
    /// Implements the <see cref="Api.BaseApiResourceCollectionController{Models.Admin.ApiResourceViewModels.ApiResourceClaimViewModel, Models.Admin.ApiResourceViewModels.ApiResourceClaimsViewModel, Duende.IdentityServer.EntityFramework.Entities.ApiResourceClaim}" />
    /// </summary>
    /// <seealso cref="Api.BaseApiResourceCollectionController{Models.Admin.ApiResourceViewModels.ApiResourceClaimViewModel, Models.Admin.ApiResourceViewModels.ApiResourceClaimsViewModel, Duende.IdentityServer.EntityFramework.Entities.ApiResourceClaim}" />
    public class ApiResourceClaimsController(ConfigurationDbContext context, IMapper mapper) : BaseApiResourceCollectionController<ApiResourceClaimViewModel, ApiResourceClaimsViewModel, ApiResourceClaim>(context, mapper)
    {

        #region BaseApiCollectionController Implementation

        /// <summary>
        /// Populates the item list.
        /// </summary>
        /// <param name="mainEntity">The main entity.</param>
        /// <returns>IEnumerable&lt;ApiResourceClaimViewModel&gt;.</returns>
        protected override IEnumerable<ApiResourceClaimViewModel> PopulateItemList(ApiResource mainEntity)
        {
            return Mapper.ProjectTo<ApiResourceClaimViewModel>(mainEntity.UserClaims.AsQueryable());
        }

        /// <summary>
        /// Adds the includes.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>IQueryable&lt;ApiResource&gt;.</returns>
        protected override IQueryable<ApiResource> AddIncludes(DbSet<ApiResource> query)
        {
            return query.Include(c => c.UserClaims);
        }

        /// <summary>
        /// Finds the item in collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>ApiResourceClaim.</returns>
        protected override ApiResourceClaim FindItemInCollection(List<ApiResourceClaim> collection, int id)
        {
            return collection.Find(c => c.Id == id);
        }

        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <param name="mainEntity">The main entity.</param>
        /// <returns>List&lt;ApiResourceClaim&gt;.</returns>
        protected override List<ApiResourceClaim> GetCollection(ApiResource mainEntity)
        {
            return mainEntity.UserClaims;
        }

        #endregion BaseApiCollectionController Implementation
    }
}