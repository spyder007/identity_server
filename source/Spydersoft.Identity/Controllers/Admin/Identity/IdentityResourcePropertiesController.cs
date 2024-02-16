using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;

using Microsoft.EntityFrameworkCore;

using Spydersoft.Identity.Models.Admin.IdentityResourceViewModels;

namespace Spydersoft.Identity.Controllers.Admin.Identity
{
    /// <summary>
    /// Class IdentityResourcePropertiesController.
    /// Implements the <see cref="Identity.BaseIdentityResourceCollectionController{Models.Admin.IdentityResourceViewModels.IdentityResourcePropertyViewModel, Models.Admin.IdentityResourceViewModels.IdentityResourcePropertiesViewModel, Duende.IdentityServer.EntityFramework.Entities.IdentityResourceProperty}" />
    /// </summary>
    /// <seealso cref="Identity.BaseIdentityResourceCollectionController{Models.Admin.IdentityResourceViewModels.IdentityResourcePropertyViewModel, Models.Admin.IdentityResourceViewModels.IdentityResourcePropertiesViewModel, Duende.IdentityServer.EntityFramework.Entities.IdentityResourceProperty}" />
    public class IdentityResourcePropertiesController(ConfigurationDbContext context, IMapper mapper) : BaseIdentityResourceCollectionController<IdentityResourcePropertyViewModel, IdentityResourcePropertiesViewModel, IdentityResourceProperty>(context, mapper)
    {

        #region BaseIdentityResourceCollectionController Implementation

        /// <summary>
        /// Populates the item list.
        /// </summary>
        /// <param name="mainEntity">The main entity.</param>
        /// <returns>IEnumerable&lt;TSingleViewModel&gt;.</returns>
        protected override IEnumerable<IdentityResourcePropertyViewModel> PopulateItemList(IdentityResource mainEntity)
        {
            return Mapper.ProjectTo<IdentityResourcePropertyViewModel>(mainEntity.Properties.AsQueryable());
        }

        /// <summary>
        /// Adds the includes.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>IQueryable&lt;IS4Entities.IdentityResource&gt;.</returns>
        protected override IQueryable<IdentityResource> AddIncludes(DbSet<IdentityResource> query)
        {
            return query.Include(ir => ir.Properties);
        }

        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <param name="mainEntity">The main entity.</param>
        /// <returns>List&lt;IdentityResourceProperty&gt;.</returns>
        protected override List<IdentityResourceProperty> GetCollection(IdentityResource mainEntity)
        {
            return mainEntity.Properties;
        }

        /// <summary>
        /// Finds the item in collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>IdentityResourceProperty.</returns>
        protected override IdentityResourceProperty FindItemInCollection(List<IdentityResourceProperty> collection, int id)
        {
            return collection.Find(prop => prop.Id == id);
        }

        #endregion BaseIdentityResourceCollectionController Implementation
    }
}