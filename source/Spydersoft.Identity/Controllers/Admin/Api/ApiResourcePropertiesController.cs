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
    /// Class ApiResourcePropertiesController.
    /// Implements the <see cref="BaseApiResourceCollectionController{TSingleViewModel, TCollectionViewModel, TChildEntity}" />
    /// </summary>
    /// <seealso cref="BaseApiResourceCollectionController{TSingleViewModel, TCollectionViewModel, TChildEntity}" />
    public class ApiResourcePropertiesController(ConfigurationDbContext context, IMapper mapper) : BaseApiResourceCollectionController<ApiResourcePropertyViewModel, ApiResourcePropertiesViewModel, ApiResourceProperty>(context, mapper)
    {

        #region BaseApiCollectionController Implementation

        /// <summary>
        /// Populates the item list.
        /// </summary>
        /// <param name="mainEntity">The main entity.</param>
        /// <returns>IEnumerable&lt;ApiResourcePropertyViewModel&gt;.</returns>
        protected override IEnumerable<ApiResourcePropertyViewModel> PopulateItemList(ApiResource mainEntity)
        {
            return Mapper.ProjectTo<ApiResourcePropertyViewModel>(mainEntity.Properties.AsQueryable());
        }

        /// <summary>
        /// Adds the includes.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>IQueryable&lt;ApiResource&gt;.</returns>
        protected override IQueryable<ApiResource> AddIncludes(DbSet<ApiResource> query)
        {
            return query.Include(api => api.Properties);
        }

        /// <summary>
        /// Finds the item in collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>ApiResourceProperty.</returns>
        protected override ApiResourceProperty FindItemInCollection(List<ApiResourceProperty> collection, int id)
        {
            return collection.Find(c => c.Id == id);
        }

        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <param name="mainEntity">The main entity.</param>
        /// <returns>List&lt;ApiResourceProperty&gt;.</returns>
        protected override List<ApiResourceProperty> GetCollection(ApiResource mainEntity)
        {
            return mainEntity.Properties;
        }

        #endregion BaseApiCollectionController Implementation
    }
}