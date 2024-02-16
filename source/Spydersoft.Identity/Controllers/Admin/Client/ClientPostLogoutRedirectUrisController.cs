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
    /// Class ClientPostLogoutRedirectUrisController.
    /// Implements the <see cref="BaseClientCollectionController{TSingleViewModel, TCollectionViewModel, TChildEntity}" />
    /// </summary>
    /// <seealso cref="BaseClientCollectionController{TSingleViewModel, TCollectionViewModel, TChildEntity}" />
    public class ClientPostLogoutRedirectUrisController(ConfigurationDbContext context, IMapper mapper) : BaseClientCollectionController<ClientPostLogoutRedirectUriViewModel, ClientPostLogoutRedirectUrisViewModel, ClientPostLogoutRedirectUri>(context, mapper)
    {

        #region BaseClientCollectionController Implementation

        /// <summary>
        /// Populates the item list.
        /// </summary>
        /// <param name="mainEntity">The main entity.</param>
        /// <returns>IEnumerable&lt;TSingleViewModel&gt;.</returns>
        protected override IEnumerable<ClientPostLogoutRedirectUriViewModel> PopulateItemList(Duende.IdentityServer.EntityFramework.Entities.Client mainEntity)
        {
            return Mapper.ProjectTo<ClientPostLogoutRedirectUriViewModel>(mainEntity.PostLogoutRedirectUris.AsQueryable());
        }

        /// <summary>
        /// Adds the includes.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>IQueryable&lt;IS4Entities.Client&gt;.</returns>
        protected override IQueryable<Duende.IdentityServer.EntityFramework.Entities.Client> AddIncludes(
            DbSet<Duende.IdentityServer.EntityFramework.Entities.Client> query)
        {
            return query.Include(c => c.PostLogoutRedirectUris);
        }

        /// <summary>
        /// Finds the item in collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>ClientPostLogoutRedirectUri.</returns>
        protected override ClientPostLogoutRedirectUri FindItemInCollection(List<ClientPostLogoutRedirectUri> collection, int id)
        {
            return collection.Find(p => p.Id == id);
        }

        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <param name="mainEntity">The main entity.</param>
        /// <returns>List&lt;ClientPostLogoutRedirectUri&gt;.</returns>
        protected override List<ClientPostLogoutRedirectUri> GetCollection(Duende.IdentityServer.EntityFramework.Entities.Client mainEntity)
        {
            return mainEntity.PostLogoutRedirectUris;
        }

        #endregion BaseClientCollectionController Implementation
    }
}