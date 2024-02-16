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
    /// Class ClientIdpRestrictionsController.
    /// Implements the <see cref="Client.BaseClientCollectionController{Models.Admin.ClientViewModels.ClientIdpRestrictionViewModel, Models.Admin.ClientViewModels.ClientIdpRestrictionsViewModel, Duende.IdentityServer.EntityFramework.Entities.ClientIdPRestriction}" />
    /// </summary>
    /// <seealso cref="Client.BaseClientCollectionController{Models.Admin.ClientViewModels.ClientIdpRestrictionViewModel, Models.Admin.ClientViewModels.ClientIdpRestrictionsViewModel, Duende.IdentityServer.EntityFramework.Entities.ClientIdPRestriction}" />
    public class ClientIdpRestrictionsController(ConfigurationDbContext context, IMapper mapper) : BaseClientCollectionController<ClientIdpRestrictionViewModel, ClientIdpRestrictionsViewModel, ClientIdPRestriction>(context, mapper)
    {

        #region BaseClientCollectionController Implementation

        /// <summary>
        /// Populates the item list.
        /// </summary>
        /// <param name="mainEntity">The main entity.</param>
        /// <returns>IEnumerable&lt;TSingleViewModel&gt;.</returns>
        protected override IEnumerable<ClientIdpRestrictionViewModel> PopulateItemList(Duende.IdentityServer.EntityFramework.Entities.Client mainEntity)
        {
            return Mapper.ProjectTo<ClientIdpRestrictionViewModel>(mainEntity.IdentityProviderRestrictions.AsQueryable());
        }

        /// <summary>
        /// Adds the includes.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>IQueryable&lt;IS4Entities.Client&gt;.</returns>
        protected override IQueryable<Duende.IdentityServer.EntityFramework.Entities.Client> AddIncludes(
            DbSet<Duende.IdentityServer.EntityFramework.Entities.Client> query)
        {
            return query.Include(c => c.IdentityProviderRestrictions);
        }

        /// <summary>
        /// Finds the item in collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>ClientIdPRestriction.</returns>
        protected override ClientIdPRestriction FindItemInCollection(List<ClientIdPRestriction> collection, int id)
        {
            return collection.Find(idp => idp.Id == id);
        }

        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <param name="mainEntity">The main entity.</param>
        /// <returns>List&lt;ClientIdPRestriction&gt;.</returns>
        protected override List<ClientIdPRestriction> GetCollection(Duende.IdentityServer.EntityFramework.Entities.Client mainEntity)
        {
            return mainEntity.IdentityProviderRestrictions;
        }

        #endregion BaseClientCollectionController Implementation
    }
}