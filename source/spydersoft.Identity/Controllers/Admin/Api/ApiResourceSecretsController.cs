using AutoMapper.QueryableExtensions;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Duende.IdentityServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using spydersoft.Identity.Models.Admin.ApiResourceViewModels;
using ApiResource = Duende.IdentityServer.EntityFramework.Entities.ApiResource;

namespace spydersoft.Identity.Controllers.Admin.Api
{
    public class ApiResourceSecretsController : BaseApiResourceCollectionController<ApiResourceSecretViewModel, ApiResourceSecretsViewModel, ApiResourceSecret>
    {
        public ApiResourceSecretsController(ConfigurationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        #region BaseApiCollectionController Implementation

        protected override IEnumerable<ApiResourceSecretViewModel> PopulateItemList(ApiResource mainEntity)
        {
            return Mapper.ProjectTo<ApiResourceSecretViewModel>(mainEntity.Secrets.ToList().AsQueryable());
        }

        protected override IQueryable<ApiResource> AddIncludes(DbSet<ApiResource> query)
        {
            return query.Include(api => api.Secrets);
        }

        protected override void SetAdditionalProperties(ApiResourceSecret newItem)
        {
            base.SetAdditionalProperties(newItem);
            newItem.Created = DateTime.UtcNow;
            newItem.Value = newItem.Value.Sha256();
        }

        protected override ApiResourceSecret FindItemInCollection(List<ApiResourceSecret> collection, int id)
        {
            return collection.FirstOrDefault(s => s.Id == id);
        }

        protected override List<ApiResourceSecret> GetCollection(ApiResource mainEntity)
        {
            return mainEntity.Secrets;
        }

        #endregion BaseApiCollectionController Implementation
    }
}