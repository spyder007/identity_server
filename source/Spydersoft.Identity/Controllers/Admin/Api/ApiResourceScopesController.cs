using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;

using Microsoft.EntityFrameworkCore;

using Spydersoft.Identity.Models.Admin.ApiResourceViewModels;

namespace Spydersoft.Identity.Controllers.Admin.Api
{
    /// <summary>
    /// Class ApiResourceScopesController.
    /// Implements the <see cref="BaseApiResourceCollectionController{TSingleViewModel, TCollectionViewModel, TChildEntity}" />
    /// </summary>
    /// <seealso cref="BaseApiResourceCollectionController{TSingleViewModel, TCollectionViewModel, TChildEntity}" />
    public class ApiResourceScopesController(ConfigurationDbContext context, IMapper mapper) : BaseApiResourceCollectionController<ApiResourceScopeViewModel, ApiResourceScopesViewModel, ApiResourceScope>(context, mapper)
    {

        #region BaseApiCollectionController Implementation

        /// <summary>
        /// Populates the item list.
        /// </summary>
        /// <param name="mainEntity">The main entity.</param>
        /// <returns>IEnumerable&lt;ApiResourceScopeViewModel&gt;.</returns>
        protected override IEnumerable<ApiResourceScopeViewModel> PopulateItemList(ApiResource mainEntity)
        {
            return Mapper.ProjectTo<ApiResourceScopeViewModel>(mainEntity.Scopes.AsQueryable());
        }

        /// <summary>
        /// Adds the includes.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>IQueryable&lt;ApiResource&gt;.</returns>
        protected override IQueryable<ApiResource> AddIncludes(DbSet<ApiResource> query)
        {
            return query.Include(c => c.Scopes);
        }

        /// <summary>
        /// Finds the item in collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>ApiResourceScope.</returns>
        protected override ApiResourceScope FindItemInCollection(List<ApiResourceScope> collection, int id)
        {
            return collection.Find(s => s.Id == id);
        }

        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <param name="mainEntity">The main entity.</param>
        /// <returns>List&lt;ApiResourceScope&gt;.</returns>
        protected override List<ApiResourceScope> GetCollection(ApiResource mainEntity)
        {
            return mainEntity.Scopes;
        }

        #endregion BaseApiCollectionController Implementation
    }
}