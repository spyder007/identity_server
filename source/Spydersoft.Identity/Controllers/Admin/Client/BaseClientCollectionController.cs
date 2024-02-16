using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Duende.IdentityServer.EntityFramework.DbContexts;

using Microsoft.EntityFrameworkCore;

using Spydersoft.Identity.Models.Admin;
using Spydersoft.Identity.Models.Admin.ClientViewModels;

using IS4Entities = Duende.IdentityServer.EntityFramework.Entities;

namespace Spydersoft.Identity.Controllers.Admin.Client
{
    /// <summary>
    /// Class BaseClientCollectionController.
    /// Implements the <see cref="BaseAdminCollectionController{TSingleViewModel, TCollectionViewModel, TMainEntityViewModel, TEntity, TChildEntity}" />
    /// </summary>
    /// <typeparam name="TSingleViewModel">The type of the t single view model.</typeparam>
    /// <typeparam name="TCollectionViewModel">The type of the t collection view model.</typeparam>
    /// <typeparam name="TChildEntity">The type of the t child entity.</typeparam>
    /// <seealso cref="BaseAdminCollectionController{TSingleViewModel, TCollectionViewModel, TMainEntityViewModel, TEntity, TChildEntity}" />
    public abstract class BaseClientCollectionController<TSingleViewModel, TCollectionViewModel, TChildEntity>(ConfigurationDbContext context, IMapper mapper)
        : BaseAdminCollectionController<TSingleViewModel, TCollectionViewModel, ClientViewModel, IS4Entities.Client, TChildEntity>(context, mapper)
        where TSingleViewModel : BaseAdminChildItemViewModel, new()
        where TCollectionViewModel : BaseClientCollectionViewModel<TSingleViewModel>, new()
    {

        #region Constructor

        #endregion Constructor

        #region BaseClientCollectionController Interface

        /// <summary>
        /// Populates the item list.
        /// </summary>
        /// <param name="mainEntity">The main entity.</param>
        /// <returns>IEnumerable&lt;TSingleViewModel&gt;.</returns>
        protected abstract override IEnumerable<TSingleViewModel> PopulateItemList(IS4Entities.Client mainEntity);

        /// <summary>
        /// Adds the includes.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>IQueryable&lt;IS4Entities.Client&gt;.</returns>
        protected abstract override IQueryable<IS4Entities.Client> AddIncludes(DbSet<IS4Entities.Client> query);

        #endregion BaseClientCollectionController Interface

        #region BaseAdminCollectionController Implementation

        /// <summary>
        /// Gets the main entity.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>IS4Entities.Client.</returns>
        protected override IS4Entities.Client GetMainEntity(int id)
        {
            DbSet<IS4Entities.Client> query = ConfigDbContext.Clients;
            IQueryable<IS4Entities.Client> includeQuery = AddIncludes(query);

            return includeQuery.FirstOrDefault(c => c.Id == id);
        }

        #endregion BaseAdminCollectionController Implementation
    }
}