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
    /// Class ClientCorsOriginsController.
    /// Implements the <see cref="Client.BaseClientCollectionController{Models.Admin.ClientViewModels.ClientCorsOriginViewModel, Models.Admin.ClientViewModels.ClientCorsOriginsViewModel, Duende.IdentityServer.EntityFramework.Entities.ClientCorsOrigin}" />
    /// </summary>
    /// <seealso cref="Client.BaseClientCollectionController{Models.Admin.ClientViewModels.ClientCorsOriginViewModel, Models.Admin.ClientViewModels.ClientCorsOriginsViewModel, Duende.IdentityServer.EntityFramework.Entities.ClientCorsOrigin}" />
    public class ClientCorsOriginsController(ConfigurationDbContext context, IMapper mapper) : BaseClientCollectionController<ClientCorsOriginViewModel, ClientCorsOriginsViewModel, ClientCorsOrigin>(context, mapper)
    {

        #region BaseClientCollectionController Implementation

        /// <summary>
        /// Populates the item list.
        /// </summary>
        /// <param name="mainEntity">The main entity.</param>
        /// <returns>IEnumerable&lt;TSingleViewModel&gt;.</returns>
        protected override IEnumerable<ClientCorsOriginViewModel> PopulateItemList(Duende.IdentityServer.EntityFramework.Entities.Client mainEntity)
        {
            return Mapper.ProjectTo<ClientCorsOriginViewModel>(mainEntity.AllowedCorsOrigins.AsQueryable());
        }

        /// <summary>
        /// Adds the includes.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>IQueryable&lt;IS4Entities.Client&gt;.</returns>
        protected override IQueryable<Duende.IdentityServer.EntityFramework.Entities.Client> AddIncludes(
            DbSet<Duende.IdentityServer.EntityFramework.Entities.Client> query)
        {
            return query.Include(c => c.AllowedCorsOrigins);
        }

        /// <summary>
        /// Finds the item in collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>ClientCorsOrigin.</returns>
        protected override ClientCorsOrigin FindItemInCollection(List<ClientCorsOrigin> collection, int id)
        {
            return collection.Find(cors => cors.Id == id);
        }

        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <param name="mainEntity">The main entity.</param>
        /// <returns>List&lt;ClientCorsOrigin&gt;.</returns>
        protected override List<ClientCorsOrigin> GetCollection(Duende.IdentityServer.EntityFramework.Entities.Client mainEntity)
        {
            return mainEntity.AllowedCorsOrigins;
        }

        #endregion BaseClientCollectionController Implementation
    }
}