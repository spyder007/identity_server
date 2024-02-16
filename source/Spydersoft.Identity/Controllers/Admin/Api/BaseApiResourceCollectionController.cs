using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Duende.IdentityServer.EntityFramework.DbContexts;

using Microsoft.EntityFrameworkCore;

using Spydersoft.Identity.Models.Admin;
using Spydersoft.Identity.Models.Admin.ApiResourceViewModels;

using IS4Entities = Duende.IdentityServer.EntityFramework.Entities;

namespace Spydersoft.Identity.Controllers.Admin.Api
{
    /// <summary>
    /// Class BaseApiResourceCollectionController.
    /// Implements the <see cref="Admin.BaseAdminCollectionController{TSingleViewModel, TCollectionViewModel, Models.Admin.ApiResourceViewModels.ApiResourceViewModel, IS4Entities.ApiResource, TChildEntity}" />
    /// </summary>
    /// <typeparam name="TSingleViewModel">The type of the t single view model.</typeparam>
    /// <typeparam name="TCollectionViewModel">The type of the t collection view model.</typeparam>
    /// <typeparam name="TChildEntity">The type of the t child entity.</typeparam>
    /// <seealso cref="Admin.BaseAdminCollectionController{TSingleViewModel, TCollectionViewModel, Models.Admin.ApiResourceViewModels.ApiResourceViewModel, IS4Entities.ApiResource, TChildEntity}" />
    public abstract class BaseApiResourceCollectionController<TSingleViewModel, TCollectionViewModel, TChildEntity>(ConfigurationDbContext context, IMapper mapper)
        : BaseAdminCollectionController<TSingleViewModel, TCollectionViewModel, ApiResourceViewModel, IS4Entities.ApiResource, TChildEntity>(context, mapper)
        where TSingleViewModel : BaseAdminChildItemViewModel, new()
        where TCollectionViewModel : BaseApiResourceCollectionViewModel<TSingleViewModel>, new()
    {

        #region Constructor

        #endregion Constructor

        #region BaseClientCollectionController Interface

        /// <summary>
        /// Populates the item list.
        /// </summary>
        /// <param name="mainEntity">The main entity.</param>
        /// <returns>IEnumerable&lt;TSingleViewModel&gt;.</returns>
        protected abstract override IEnumerable<TSingleViewModel> PopulateItemList(IS4Entities.ApiResource mainEntity);

        /// <summary>
        /// Adds the includes.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>IQueryable&lt;IS4Entities.ApiResource&gt;.</returns>
        protected abstract override IQueryable<IS4Entities.ApiResource> AddIncludes(
            DbSet<IS4Entities.ApiResource> query);

        #endregion BaseClientCollectionController Interface

        #region BaseAdminCollectionController Implementation

        /// <summary>
        /// Gets the main entity.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>IS4Entities.ApiResource.</returns>
        protected override IS4Entities.ApiResource GetMainEntity(int id)
        {
            DbSet<IS4Entities.ApiResource> query = ConfigDbContext.ApiResources;
            IQueryable<IS4Entities.ApiResource> includeQuery = AddIncludes(query);

            return includeQuery.FirstOrDefault(c => c.Id == id);
        }

        #endregion BaseAdminCollectionController Implementation
    }
}