using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Duende.IdentityServer.EntityFramework.DbContexts;

using Microsoft.EntityFrameworkCore;

using Spydersoft.Identity.Models.Admin;
using Spydersoft.Identity.Models.Admin.IdentityResourceViewModels;

using IS4Entities = Duende.IdentityServer.EntityFramework.Entities;

namespace Spydersoft.Identity.Controllers.Admin.Identity
{
    /// <summary>
    /// Class BaseIdentityResourceCollectionController.
    /// Implements the <see cref="Admin.BaseAdminCollectionController{TSingleViewModel, TCollectionViewModel, Models.Admin.IdentityResourceViewModels.IdentityResourceViewModel, IS4Entities.IdentityResource, TChildEntity}" />
    /// </summary>
    /// <typeparam name="TSingleViewModel">The type of the t single view model.</typeparam>
    /// <typeparam name="TCollectionViewModel">The type of the t collection view model.</typeparam>
    /// <typeparam name="TChildEntity">The type of the t child entity.</typeparam>
    /// <seealso cref="Admin.BaseAdminCollectionController{TSingleViewModel, TCollectionViewModel, Models.Admin.IdentityResourceViewModels.IdentityResourceViewModel, IS4Entities.IdentityResource, TChildEntity}" />
    public abstract class BaseIdentityResourceCollectionController<TSingleViewModel, TCollectionViewModel, TChildEntity>(ConfigurationDbContext context, IMapper mapper)
        : BaseAdminCollectionController<TSingleViewModel, TCollectionViewModel, IdentityResourceViewModel, IS4Entities.IdentityResource, TChildEntity>(context, mapper)
        where TSingleViewModel : BaseAdminChildItemViewModel, new()
        where TCollectionViewModel : BaseIdentityResourceCollectionViewModel<TSingleViewModel>, new()
    {

        #region Constructor

        #endregion Constructor

        #region BaseIdentityResourceCollectionController Interface

        /// <summary>
        /// Populates the item list.
        /// </summary>
        /// <param name="mainEntity">The main entity.</param>
        /// <returns>IEnumerable&lt;TSingleViewModel&gt;.</returns>
        protected abstract override IEnumerable<TSingleViewModel> PopulateItemList(IS4Entities.IdentityResource mainEntity);

        /// <summary>
        /// Adds the includes.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>IQueryable&lt;IS4Entities.IdentityResource&gt;.</returns>
        protected abstract override IQueryable<IS4Entities.IdentityResource> AddIncludes(
            DbSet<IS4Entities.IdentityResource> query);

        #endregion BaseIdentityResourceCollectionController Interface

        #region BaseAdminCollectionController Implementation

        /// <summary>
        /// Gets the main entity.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>IS4Entities.IdentityResource.</returns>
        protected override IS4Entities.IdentityResource GetMainEntity(int id)
        {
            DbSet<IS4Entities.IdentityResource> query = ConfigDbContext.IdentityResources;
            IQueryable<IS4Entities.IdentityResource> includeQuery = AddIncludes(query);

            return includeQuery.FirstOrDefault(ir => ir.Id == id);
        }

        #endregion BaseAdminCollectionController Implementation
    }
}