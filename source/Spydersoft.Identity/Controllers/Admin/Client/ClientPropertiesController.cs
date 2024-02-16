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
    /// Class ClientPropertiesController.
    /// Implements the <see cref="Client.BaseClientCollectionController{Models.Admin.ClientViewModels.ClientPropertyViewModel, Models.Admin.ClientViewModels.ClientPropertiesViewModel, Duende.IdentityServer.EntityFramework.Entities.ClientProperty}" />
    /// </summary>
    /// <seealso cref="Client.BaseClientCollectionController{Models.Admin.ClientViewModels.ClientPropertyViewModel, Models.Admin.ClientViewModels.ClientPropertiesViewModel, Duende.IdentityServer.EntityFramework.Entities.ClientProperty}" />
    public class ClientPropertiesController(ConfigurationDbContext context, IMapper mapper) : BaseClientCollectionController<ClientPropertyViewModel, ClientPropertiesViewModel, ClientProperty>(context, mapper)
    {

        #region BaseClientCollectionController Implementation

        /// <summary>
        /// Populates the item list.
        /// </summary>
        /// <param name="mainEntity">The main entity.</param>
        /// <returns>IEnumerable&lt;TSingleViewModel&gt;.</returns>
        protected override IEnumerable<ClientPropertyViewModel> PopulateItemList(Duende.IdentityServer.EntityFramework.Entities.Client mainEntity)
        {
            return Mapper.ProjectTo<ClientPropertyViewModel>(mainEntity.Properties.AsQueryable());
        }

        /// <summary>
        /// Adds the includes.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>IQueryable&lt;IS4Entities.Client&gt;.</returns>
        protected override IQueryable<Duende.IdentityServer.EntityFramework.Entities.Client> AddIncludes(
            DbSet<Duende.IdentityServer.EntityFramework.Entities.Client> query)
        {
            return query.Include(c => c.Properties);
        }

        /// <summary>
        /// Finds the item in collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>ClientProperty.</returns>
        protected override ClientProperty FindItemInCollection(List<ClientProperty> collection, int id)
        {
            return collection.Find(p => p.Id == id);
        }

        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <param name="mainEntity">The main entity.</param>
        /// <returns>List&lt;ClientProperty&gt;.</returns>
        protected override List<ClientProperty> GetCollection(Duende.IdentityServer.EntityFramework.Entities.Client mainEntity)
        {
            return mainEntity.Properties;
        }

        #endregion BaseClientCollectionController Implementation
    }
}