﻿using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;

using Microsoft.EntityFrameworkCore;

using Spydersoft.Identity.Models.Admin.ApiResourceViewModels;

namespace Spydersoft.Identity.Controllers.Admin.Api
{
    public class ApiResourceClaimsController : BaseApiResourceCollectionController<ApiResourceClaimViewModel, ApiResourceClaimsViewModel, ApiResourceClaim>
    {
        public ApiResourceClaimsController(ConfigurationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        #region BaseApiCollectionController Implementation

        protected override IEnumerable<ApiResourceClaimViewModel> PopulateItemList(ApiResource mainEntity)
        {
            return Mapper.ProjectTo<ApiResourceClaimViewModel>(mainEntity.UserClaims.AsQueryable());
        }

        protected override IQueryable<ApiResource> AddIncludes(DbSet<ApiResource> query)
        {
            return query.Include(c => c.UserClaims);
        }

        protected override ApiResourceClaim FindItemInCollection(List<ApiResourceClaim> collection, int id)
        {
            return collection.Find(c => c.Id == id);
        }

        protected override List<ApiResourceClaim> GetCollection(ApiResource mainEntity)
        {
            return mainEntity.UserClaims;
        }

        #endregion BaseApiCollectionController Implementation
    }
}