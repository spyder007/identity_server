using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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

        protected override ApiResourceClaim FindItemInCollection(List<ApiResourceClaim> collection, int id)
        {
            return collection.FirstOrDefault(c => c.Id == id);
        }

        protected override List<ApiResourceClaim> GetCollection(ApiResource mainEntity)
        {
            return mainEntity.UserClaims;
        }
    }
}
