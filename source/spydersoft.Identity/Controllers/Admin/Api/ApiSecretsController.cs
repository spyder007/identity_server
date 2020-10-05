using AutoMapper.QueryableExtensions;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using spydersoft.Identity.Models.Admin.ApiViewModels;
using ApiResource = IdentityServer4.EntityFramework.Entities.ApiResource;

namespace spydersoft.Identity.Controllers.Admin.Api
{
    public class ApiSecretsController : BaseApiCollectionController<ApiSecretViewModel, ApiSecretsViewModel, ApiResourceSecret>
    {
        public ApiSecretsController(ConfigurationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        #region BaseApiCollectionController Implementation

        protected override IEnumerable<ApiSecretViewModel> PopulateItemList(ApiResource mainEntity)
        {
            return Mapper.ProjectTo<ApiSecretViewModel>(mainEntity.Secrets.ToList().AsQueryable());
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