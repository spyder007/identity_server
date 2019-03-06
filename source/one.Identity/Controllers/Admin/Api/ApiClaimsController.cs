using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using one.Identity.Models.ApiViewModels;

namespace one.Identity.Controllers.Admin.Api
{
    public class ApiClaimsController : BaseApiCollectionController<ApiClaimViewModel, ApiClaimsViewModel, ApiResourceClaim>
    {
        public ApiClaimsController(ConfigurationDbContext context) : base(context)
        {
        }

        protected override IEnumerable<ApiClaimViewModel> PopulateItemList(ApiResource mainEntity)
        {
            return mainEntity.UserClaims.AsQueryable().ProjectTo<ApiClaimViewModel>();
        }

        protected override IIncludableQueryable<ApiResource, List<ApiResourceClaim>> AddIncludes(DbSet<ApiResource> query)
        {
            return query.Include(c => c.UserClaims);
        }

        protected override void RemoveObject(ApiResource mainEntity, int id)
        {
            var claim = mainEntity.UserClaims.FirstOrDefault(uc => uc.Id == id);
            mainEntity.UserClaims.Remove(claim);
        }

        protected override void AddObject(ApiResource mainEntity, int parentId, ApiClaimViewModel newItem)
        {
            mainEntity.UserClaims.Add(new ApiResourceClaim()
            {
                ApiResourceId = parentId,
                Type = newItem.Type
            });
        }
    }
}
