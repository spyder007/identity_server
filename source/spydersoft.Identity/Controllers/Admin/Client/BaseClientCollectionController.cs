﻿using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using spydersoft.Identity.Models;
using System.Collections.Generic;
using System.Linq;
using spydersoft.Identity.Models.Admin.ClientViewModels;
using IS4Entities = IdentityServer4.EntityFramework.Entities;

namespace spydersoft.Identity.Controllers.Admin.Client
{
    public abstract class BaseClientCollectionController<TSingleViewModel, TCollectionViewModel, TChildEntity>
        : BaseAdminCollectionController<TSingleViewModel, TCollectionViewModel, IS4Entities.Client, TChildEntity>
        where TSingleViewModel : BaseAdminChildItemViewModel, new()
        where TCollectionViewModel : BaseClientCollectionViewModel<TSingleViewModel>, new()
    {
        #region Constructor

        protected BaseClientCollectionController(ConfigurationDbContext context) : base(context)
        {
        }

        #endregion Constructor

        #region BaseClientCollectionController Interface

        protected abstract override IEnumerable<TSingleViewModel> PopulateItemList(IS4Entities.Client mainEntity);

        protected abstract override IQueryable<IS4Entities.Client> AddIncludes(DbSet<IS4Entities.Client> query);

        #endregion BaseClientCollectionController Interface

        #region BaseAdminCollectionController Implementation

        protected override IS4Entities.Client GetMainEntity(int id)
        {
            var query = ConfigDbContext.Clients;
            var includeQuery = AddIncludes(query);

            return includeQuery.FirstOrDefault(c => c.Id == id);
        }

        #endregion BaseAdminCollectionController Implementation
    }
}