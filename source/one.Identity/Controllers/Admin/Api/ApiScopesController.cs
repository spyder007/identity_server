﻿using AutoMapper.QueryableExtensions;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Collections.Generic;
using System.Linq;
using one.Identity.Models.Admin.ApiViewModels;

namespace one.Identity.Controllers.Admin.Api
{
    public class ApiScopesController : BaseApiCollectionController<ApiScopeViewModel, ApiScopesViewModel, ApiScope>
    {
        public ApiScopesController(ConfigurationDbContext context) : base(context)
        {
        }

        #region BaseApiCollectionController Implementation

        protected override IEnumerable<ApiScopeViewModel> PopulateItemList(ApiResource mainEntity)
        {
            return mainEntity.Scopes.AsQueryable().ProjectTo<ApiScopeViewModel>();
        }

        protected override IIncludableQueryable<ApiResource, List<ApiScope>> AddIncludes(DbSet<ApiResource> query)
        {
            return query.Include(c => c.Scopes);
        }

        protected override ApiScope FindItemInCollection(List<ApiScope> collection, int id)
        {
            return collection.Find(s => s.Id == id);
        }

        protected override List<ApiScope> GetCollection(ApiResource mainEntity)
        {
            return mainEntity.Scopes;
        }

        #endregion BaseApiCollectionController Implementation
    }
}