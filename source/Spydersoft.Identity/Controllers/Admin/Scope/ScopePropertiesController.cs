using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;

using Microsoft.EntityFrameworkCore;

using Spydersoft.Identity.Models.Admin.ScopeViewModels;

namespace Spydersoft.Identity.Controllers.Admin.Scope
{
    /// <summary>
    /// Class ScopePropertiesController.
    /// Implements the <see cref="Scope.BaseScopeCollectionController{Models.Admin.ScopeViewModels.ScopePropertyViewModel, Models.Admin.ScopeViewModels.ScopePropertiesViewModel, Duende.IdentityServer.EntityFramework.Entities.ApiScopeProperty}" />
    /// </summary>
    /// <seealso cref="Scope.BaseScopeCollectionController{Models.Admin.ScopeViewModels.ScopePropertyViewModel, Models.Admin.ScopeViewModels.ScopePropertiesViewModel, Duende.IdentityServer.EntityFramework.Entities.ApiScopeProperty}" />
    public class ScopePropertiesController(ConfigurationDbContext context, IMapper mapper) : BaseScopeCollectionController<ScopePropertyViewModel, ScopePropertiesViewModel, ApiScopeProperty>(context, mapper)
    {

        #region BaseApiCollectionController Implementation

        /// <summary>
        /// Populates the item list.
        /// </summary>
        /// <param name="mainEntity">The main entity.</param>
        /// <returns>IEnumerable&lt;TSingleViewModel&gt;.</returns>
        protected override IEnumerable<ScopePropertyViewModel> PopulateItemList(ApiScope mainEntity)
        {
            return Mapper.ProjectTo<ScopePropertyViewModel>(mainEntity.Properties.AsQueryable());
        }

        /// <summary>
        /// Adds the includes.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>IQueryable&lt;IS4Entities.ApiScope&gt;.</returns>
        protected override IQueryable<ApiScope> AddIncludes(DbSet<ApiScope> query)
        {
            return query.Include(api => api.Properties);
        }

        /// <summary>
        /// Finds the item in collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>ApiScopeProperty.</returns>
        protected override ApiScopeProperty FindItemInCollection(List<ApiScopeProperty> collection, int id)
        {
            return collection.Find(c => c.Id == id);
        }

        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <param name="mainEntity">The main entity.</param>
        /// <returns>List&lt;ApiScopeProperty&gt;.</returns>
        protected override List<ApiScopeProperty> GetCollection(ApiScope mainEntity)
        {
            return mainEntity.Properties;
        }

        #endregion BaseApiCollectionController Implementation
    }
}