using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Duende.IdentityServer.EntityFramework.DbContexts;

using Microsoft.EntityFrameworkCore;

using Spydersoft.Identity.Models.Admin;
using Spydersoft.Identity.Models.Admin.ScopeViewModels;

using IS4Entities = Duende.IdentityServer.EntityFramework.Entities;

namespace Spydersoft.Identity.Controllers.Admin.Scope
{
    /// <summary>
    /// Class BaseScopeCollectionController.
    /// Implements the <see cref="Admin.BaseAdminCollectionController{TSingleViewModel, TCollectionViewModel, Models.Admin.ScopeViewModels.ScopeViewModel, IS4Entities.ApiScope, TChildEntity}" />
    /// </summary>
    /// <typeparam name="TSingleViewModel">The type of the t single view model.</typeparam>
    /// <typeparam name="TCollectionViewModel">The type of the t collection view model.</typeparam>
    /// <typeparam name="TChildEntity">The type of the t child entity.</typeparam>
    /// <seealso cref="Admin.BaseAdminCollectionController{TSingleViewModel, TCollectionViewModel, Models.Admin.ScopeViewModels.ScopeViewModel, IS4Entities.ApiScope, TChildEntity}" />
    public abstract class BaseScopeCollectionController<TSingleViewModel, TCollectionViewModel, TChildEntity>(ConfigurationDbContext context, IMapper mapper)
        : BaseAdminCollectionController<TSingleViewModel, TCollectionViewModel, ScopeViewModel, IS4Entities.ApiScope, TChildEntity>(context, mapper)
        where TSingleViewModel : BaseAdminChildItemViewModel, new()
        where TCollectionViewModel : BaseScopeCollectionViewModel<TSingleViewModel>, new()
    {

        #region Constructor

        #endregion Constructor

        #region BaseClientCollectionController Interface

        /// <summary>
        /// Populates the item list.
        /// </summary>
        /// <param name="mainEntity">The main entity.</param>
        /// <returns>IEnumerable&lt;TSingleViewModel&gt;.</returns>
        protected abstract override IEnumerable<TSingleViewModel> PopulateItemList(IS4Entities.ApiScope mainEntity);

        /// <summary>
        /// Adds the includes.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>IQueryable&lt;IS4Entities.ApiScope&gt;.</returns>
        protected abstract override IQueryable<IS4Entities.ApiScope> AddIncludes(
            DbSet<IS4Entities.ApiScope> query);

        #endregion BaseClientCollectionController Interface

        #region BaseAdminCollectionController Implementation

        /// <summary>
        /// Gets the main entity.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>IS4Entities.ApiScope.</returns>
        protected override IS4Entities.ApiScope GetMainEntity(int id)
        {
            DbSet<IS4Entities.ApiScope> query = ConfigDbContext.ApiScopes;
            IQueryable<IS4Entities.ApiScope> includeQuery = AddIncludes(query);

            return includeQuery.FirstOrDefault(c => c.Id == id);
        }

        #endregion BaseAdminCollectionController Implementation
    }
}