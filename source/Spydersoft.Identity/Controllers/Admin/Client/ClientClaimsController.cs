using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;

using Microsoft.EntityFrameworkCore;

using Spydersoft.Identity.Models.Admin.ClientViewModels;

namespace Spydersoft.Identity.Controllers.Admin.Client
{
    /// <summary>
    /// Class ClientClaimsController.
    /// Implements the <see cref="Client.BaseClientCollectionController{Models.Admin.ClientViewModels.ClientClaimViewModel, Models.Admin.ClientViewModels.ClientClaimsViewModel, Duende.IdentityServer.EntityFramework.Entities.ClientClaim}" />
    /// </summary>
    /// <seealso cref="Client.BaseClientCollectionController{Models.Admin.ClientViewModels.ClientClaimViewModel, Models.Admin.ClientViewModels.ClientClaimsViewModel, Duende.IdentityServer.EntityFramework.Entities.ClientClaim}" />
    public class ClientClaimsController(ConfigurationDbContext context, IMapper mapper) : BaseClientCollectionController<ClientClaimViewModel, ClientClaimsViewModel, ClientClaim>(context, mapper)
    {

        #region BaseClientCollectionController Implementation

        /// <summary>
        /// Populates the item list.
        /// </summary>
        /// <param name="mainEntity">The main entity.</param>
        /// <returns>IEnumerable&lt;TSingleViewModel&gt;.</returns>
        protected override IEnumerable<ClientClaimViewModel> PopulateItemList(Duende.IdentityServer.EntityFramework.Entities.Client mainEntity)
        {
            return Mapper.ProjectTo<ClientClaimViewModel>(mainEntity.Claims.AsQueryable());
        }

        /// <summary>
        /// Adds the includes.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>IQueryable&lt;IS4Entities.Client&gt;.</returns>
        protected override IQueryable<Duende.IdentityServer.EntityFramework.Entities.Client> AddIncludes(
            DbSet<Duende.IdentityServer.EntityFramework.Entities.Client> query)
        {
            return query.Include(c => c.Claims);
        }

        /// <summary>
        /// Finds the item in collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>ClientClaim.</returns>
        protected override ClientClaim FindItemInCollection(List<ClientClaim> collection, int id)
        {
            return collection.Find(c => c.Id == id);
        }

        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <param name="mainEntity">The main entity.</param>
        /// <returns>List&lt;ClientClaim&gt;.</returns>
        protected override List<ClientClaim> GetCollection(Duende.IdentityServer.EntityFramework.Entities.Client mainEntity)
        {
            return mainEntity.Claims;
        }

        #endregion BaseClientCollectionController Implementation
    }
}