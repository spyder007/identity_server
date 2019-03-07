using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using one.Identity.Models.ApiViewModels;
using ApiResource = IdentityServer4.EntityFramework.Entities.ApiResource;

namespace one.Identity.Controllers.Admin.Api
{
    public class ApiSecretsController : BaseApiCollectionController<ApiSecretViewModel, ApiSecretsViewModel, ApiSecret>
    {
        public ApiSecretsController(ConfigurationDbContext context) : base(context)
        {
        }

        protected override IEnumerable<ApiSecretViewModel> PopulateItemList(ApiResource mainEntity)
        {
            return mainEntity.Secrets.AsQueryable().ProjectTo<ApiSecretViewModel>();
        }

        protected override IIncludableQueryable<ApiResource, List<ApiSecret>> AddIncludes(DbSet<ApiResource> query)
        {
            return query.Include(api => api.Secrets);
        }

        protected override void SetAdditionalProperties(ApiSecret newItem)
        {
            base.SetAdditionalProperties(newItem);
            newItem.Created = DateTime.UtcNow;
            newItem.Value = newItem.Value.Sha256();
        }

        protected override ApiSecret FindItemInCollection(List<ApiSecret> collection, int id)
        {
            return collection.FirstOrDefault(s => s.Id == id);
        }

        protected override List<ApiSecret> GetCollection(ApiResource mainEntity)
        {
            return mainEntity.Secrets;
        }
    }
}
