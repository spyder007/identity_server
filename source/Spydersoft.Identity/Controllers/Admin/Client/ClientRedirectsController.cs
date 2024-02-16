using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Duende.IdentityServer.EntityFramework.DbContexts;

using Microsoft.EntityFrameworkCore;

using Spydersoft.Identity.Models.Admin.ClientViewModels;

using IS4Entities = Duende.IdentityServer.EntityFramework.Entities;

namespace Spydersoft.Identity.Controllers.Admin.Client
{
    /// <summary>
    /// Class ClientRedirectsController.
    /// Implements the <see cref="BaseClientCollectionController{TSingleViewModel, TCollectionViewModel, TChildEntity}" />
    /// </summary>
    /// <seealso cref="BaseClientCollectionController{TSingleViewModel, TCollectionViewModel, TChildEntity}" />
    public class ClientRedirectsController(ConfigurationDbContext context, IMapper mapper) : BaseClientCollectionController<ClientRedirectViewModel, ClientRedirectsViewModel, IS4Entities.ClientRedirectUri>(context, mapper)
    {

        #region BaseClientCollectinoController Implementation

        /// <summary>
        /// Populates the item list.
        /// </summary>
        /// <param name="mainEntity">The main entity.</param>
        /// <returns>IEnumerable&lt;TSingleViewModel&gt;.</returns>
        protected override IEnumerable<ClientRedirectViewModel> PopulateItemList(IS4Entities.Client mainEntity)
        {
            return Mapper.ProjectTo<ClientRedirectViewModel>(mainEntity.RedirectUris.AsQueryable());
        }

        /// <summary>
        /// Adds the includes.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>IQueryable&lt;IS4Entities.Client&gt;.</returns>
        protected override IQueryable<IS4Entities.Client> AddIncludes(DbSet<IS4Entities.Client> query)
        {
            return query.Include(c => c.RedirectUris);
        }

        /// <summary>
        /// Finds the item in collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>IS4Entities.ClientRedirectUri.</returns>
        protected override IS4Entities.ClientRedirectUri FindItemInCollection(List<IS4Entities.ClientRedirectUri> collection, int id)
        {
            return collection.Find(r => r.Id == id);
        }

        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <param name="mainEntity">The main entity.</param>
        /// <returns>List&lt;IS4Entities.ClientRedirectUri&gt;.</returns>
        protected override List<IS4Entities.ClientRedirectUri> GetCollection(IS4Entities.Client mainEntity)
        {
            return mainEntity.RedirectUris;
        }

        #endregion BaseClientCollectinoController Implementation
    }
}