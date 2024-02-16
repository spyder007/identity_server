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
    /// Class ClientScopesController.
    /// Implements the <see cref="BaseClientCollectionController{TSingleViewModel, TCollectionViewModel, TChildEntity}" />
    /// </summary>
    /// <seealso cref="BaseClientCollectionController{TSingleViewModel, TCollectionViewModel, TChildEntity}" />
    public class ClientScopesController(ConfigurationDbContext dbContext, IMapper mapper) : BaseClientCollectionController<ClientScopeViewModel, ClientScopesViewModel, ClientScope>(dbContext, mapper)
    {

        #region Constructor

        #endregion Constructor

        #region BaseClientCollectionController Implementation

        /// <summary>
        /// Populates the item list.
        /// </summary>
        /// <param name="mainEntity">The main entity.</param>
        /// <returns>IEnumerable&lt;TSingleViewModel&gt;.</returns>
        protected override IEnumerable<ClientScopeViewModel> PopulateItemList(Duende.IdentityServer.EntityFramework.Entities.Client mainEntity)
        {
            return Mapper.ProjectTo<ClientScopeViewModel>(mainEntity.AllowedScopes.AsQueryable());
        }

        /// <summary>
        /// Adds the includes.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>IQueryable&lt;IS4Entities.Client&gt;.</returns>
        protected override IQueryable<Duende.IdentityServer.EntityFramework.Entities.Client> AddIncludes(
            DbSet<Duende.IdentityServer.EntityFramework.Entities.Client> query)
        {
            return query.Include(c => c.AllowedScopes);
        }

        /// <summary>
        /// Finds the item in collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>ClientScope.</returns>
        protected override ClientScope FindItemInCollection(List<ClientScope> collection, int id)
        {
            return collection.Find(s => s.Id == id);
        }

        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <param name="mainEntity">The main entity.</param>
        /// <returns>List&lt;ClientScope&gt;.</returns>
        protected override List<ClientScope> GetCollection(Duende.IdentityServer.EntityFramework.Entities.Client mainEntity)
        {
            return mainEntity.AllowedScopes;
        }

        #endregion BaseClientCollectionController Implementation
    }
}