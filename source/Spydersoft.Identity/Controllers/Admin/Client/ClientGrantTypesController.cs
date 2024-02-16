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
    /// Class ClientGrantTypesController.
    /// Implements the <see cref="Client.BaseClientCollectionController{Models.Admin.ClientViewModels.ClientGrantTypeViewModel, Models.Admin.ClientViewModels.ClientGrantTypesViewModel, Duende.IdentityServer.EntityFramework.Entities.ClientGrantType}" />
    /// </summary>
    /// <seealso cref="Client.BaseClientCollectionController{Models.Admin.ClientViewModels.ClientGrantTypeViewModel, Models.Admin.ClientViewModels.ClientGrantTypesViewModel, Duende.IdentityServer.EntityFramework.Entities.ClientGrantType}" />
    public class ClientGrantTypesController(ConfigurationDbContext context, IMapper mapper) : BaseClientCollectionController<ClientGrantTypeViewModel, ClientGrantTypesViewModel, ClientGrantType>(context, mapper)
    {

        #region BaseClientCollectionController Implementation

        /// <summary>
        /// Populates the item list.
        /// </summary>
        /// <param name="mainEntity">The main entity.</param>
        /// <returns>IEnumerable&lt;TSingleViewModel&gt;.</returns>
        protected override IEnumerable<ClientGrantTypeViewModel> PopulateItemList(Duende.IdentityServer.EntityFramework.Entities.Client mainEntity)
        {
            return Mapper.ProjectTo<ClientGrantTypeViewModel>(mainEntity.AllowedGrantTypes.AsQueryable());
        }

        /// <summary>
        /// Adds the includes.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>IQueryable&lt;IS4Entities.Client&gt;.</returns>
        protected override IQueryable<Duende.IdentityServer.EntityFramework.Entities.Client> AddIncludes(
            DbSet<Duende.IdentityServer.EntityFramework.Entities.Client> query)
        {
            return query.Include(c => c.AllowedGrantTypes);
        }

        /// <summary>
        /// Finds the item in collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>ClientGrantType.</returns>
        protected override ClientGrantType FindItemInCollection(List<ClientGrantType> collection, int id)
        {
            return collection.Find(g => g.Id == id);
        }

        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <param name="mainEntity">The main entity.</param>
        /// <returns>List&lt;ClientGrantType&gt;.</returns>
        protected override List<ClientGrantType> GetCollection(Duende.IdentityServer.EntityFramework.Entities.Client mainEntity)
        {
            return mainEntity.AllowedGrantTypes;
        }

        #endregion BaseClientCollectionController Implementation
    }
}