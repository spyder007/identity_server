﻿using AutoMapper.QueryableExtensions;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Collections.Generic;
using System.Linq;
using spydersoft.Identity.Models.Admin.ApiViewModels;

namespace spydersoft.Identity.Controllers.Admin.Api
{
    public class ApiPropertiesController : BaseApiCollectionController<ApiPropertyViewModel, ApiPropertiesViewModel, ApiResourceProperty>
    {
        public ApiPropertiesController(ConfigurationDbContext context) : base(context)
        {
        }

        #region BaseApiCollectionController Implementation

        protected override IEnumerable<ApiPropertyViewModel> PopulateItemList(ApiResource mainEntity)
        {
            return mainEntity.Properties.AsQueryable().ProjectTo<ApiPropertyViewModel>();
        }

        protected override IQueryable<ApiResource> AddIncludes(DbSet<ApiResource> query)
        {
            return query.Include(api => api.Properties);
        }

        protected override ApiResourceProperty FindItemInCollection(List<ApiResourceProperty> collection, int id)
        {
            return collection.FirstOrDefault(c => c.Id == id);
        }

        protected override List<ApiResourceProperty> GetCollection(ApiResource mainEntity)
        {
            return mainEntity.Properties;
        }

        #endregion BaseApiCollectionController Implementation
    }
}