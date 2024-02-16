using System;
using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Duende.IdentityServer.Models;

using Microsoft.EntityFrameworkCore;

using Spydersoft.Identity.Models.Admin.ApiResourceViewModels;

using ApiResource = Duende.IdentityServer.EntityFramework.Entities.ApiResource;

namespace Spydersoft.Identity.Controllers.Admin.Api
{
    /// <summary>
    /// Class ApiResourceSecretsController.
    /// Implements the <see cref="Api.BaseApiResourceCollectionController{Models.Admin.ApiResourceViewModels.ApiResourceSecretViewModel, Models.Admin.ApiResourceViewModels.ApiResourceSecretsViewModel, Duende.IdentityServer.EntityFramework.Entities.ApiResourceSecret}" />
    /// </summary>
    /// <seealso cref="Api.BaseApiResourceCollectionController{Models.Admin.ApiResourceViewModels.ApiResourceSecretViewModel, Models.Admin.ApiResourceViewModels.ApiResourceSecretsViewModel, Duende.IdentityServer.EntityFramework.Entities.ApiResourceSecret}" />
    public class ApiResourceSecretsController(ConfigurationDbContext context, IMapper mapper) : BaseApiResourceCollectionController<ApiResourceSecretViewModel, ApiResourceSecretsViewModel, ApiResourceSecret>(context, mapper)
    {

        #region BaseApiCollectionController Implementation

        /// <summary>
        /// Populates the item list.
        /// </summary>
        /// <param name="mainEntity">The main entity.</param>
        /// <returns>IEnumerable&lt;ApiResourceSecretViewModel&gt;.</returns>
        protected override IEnumerable<ApiResourceSecretViewModel> PopulateItemList(ApiResource mainEntity)
        {
            return Mapper.ProjectTo<ApiResourceSecretViewModel>(mainEntity.Secrets.AsQueryable());
        }

        /// <summary>
        /// Adds the includes.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>IQueryable&lt;ApiResource&gt;.</returns>
        protected override IQueryable<ApiResource> AddIncludes(DbSet<ApiResource> query)
        {
            return query.Include(api => api.Secrets);
        }

        /// <summary>
        /// Sets the additional properties.
        /// </summary>
        /// <param name="newItem">The new item.</param>
        protected override void SetAdditionalProperties(ApiResourceSecret newItem)
        {
            base.SetAdditionalProperties(newItem);
            newItem.Created = DateTime.UtcNow;
            newItem.Value = newItem.Value.Sha256();
        }

        /// <summary>
        /// Finds the item in collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>ApiResourceSecret.</returns>
        protected override ApiResourceSecret FindItemInCollection(List<ApiResourceSecret> collection, int id)
        {
            return collection.Find(s => s.Id == id);
        }

        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <param name="mainEntity">The main entity.</param>
        /// <returns>List&lt;ApiResourceSecret&gt;.</returns>
        protected override List<ApiResourceSecret> GetCollection(ApiResource mainEntity)
        {
            return mainEntity.Secrets;
        }

        #endregion BaseApiCollectionController Implementation
    }
}