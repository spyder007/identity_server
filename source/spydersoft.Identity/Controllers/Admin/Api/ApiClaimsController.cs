using AutoMapper.QueryableExtensions;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using spydersoft.Identity.Models.Admin.ApiViewModels;

namespace spydersoft.Identity.Controllers.Admin.Api
{
    public class ApiClaimsController : BaseApiCollectionController<ApiClaimViewModel, ApiClaimsViewModel, ApiResourceClaim>
    {
        public ApiClaimsController(ConfigurationDbContext context, MapperConfiguration mapperConfig) : base(context, mapperConfig)
        {
        }

        #region BaseApiCollectionController Implementation

        protected override IEnumerable<ApiClaimViewModel> PopulateItemList(ApiResource mainEntity)
        {
            return mainEntity.UserClaims.AsQueryable().ProjectTo<ApiClaimViewModel>(AutoMapperConfiguration);
        }

        protected override IQueryable<ApiResource> AddIncludes(DbSet<ApiResource> query)
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

        #endregion BaseApiCollectionController Implementation
    }
}