using AutoMapper.QueryableExtensions;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using spydersoft.Identity.Models.Admin.ApiResourceViewModels;

namespace spydersoft.Identity.Controllers.Admin.Api
{
    public class ApiResourceClaimsController : BaseApiResourceCollectionController<ApiResourceClaimViewModel, ApiResourceClaimsViewModel, ApiResourceClaim>
    {
        public ApiResourceClaimsController(ConfigurationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        #region BaseApiCollectionController Implementation

        protected override IEnumerable<ApiResourceClaimViewModel> PopulateItemList(ApiResource mainEntity)
        {
            return Mapper.ProjectTo<ApiResourceClaimViewModel>(mainEntity.UserClaims.ToList().AsQueryable());
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