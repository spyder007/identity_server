﻿using AutoMapper.QueryableExtensions;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using one.Identity.Models.ClientViewModels;
using System.Collections.Generic;
using System.Linq;

namespace one.Identity.Controllers.Admin.Client
{
    public class ClientScopesController : BaseClientCollectionController<ClientScopeViewModel, ClientScopesViewModel, ClientScope>
    {
        #region Constructor

        public ClientScopesController(ConfigurationDbContext dbContext) : base(dbContext)
        {
        }

        #endregion Constructor

        #region BaseClientCollectionController Implementation

        protected override IEnumerable<ClientScopeViewModel> PopulateItemList(IdentityServer4.EntityFramework.Entities.Client mainEntity)
        {
            return mainEntity.AllowedScopes.AsQueryable().ProjectTo<ClientScopeViewModel>();
        }

        protected override Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<IdentityServer4.EntityFramework.Entities.Client, List<ClientScope>> AddIncludes(DbSet<IdentityServer4.EntityFramework.Entities.Client> query)
        {
            return query.Include(c => c.AllowedScopes);
        }

        protected override void RemoveObject(IdentityServer4.EntityFramework.Entities.Client mainEntity, int id)
        {
            var scopeToDelete = mainEntity.AllowedScopes.FirstOrDefault(s => s.Id == id);
            mainEntity.AllowedScopes.Remove(scopeToDelete);
        }

        protected override void AddObject(IdentityServer4.EntityFramework.Entities.Client mainEntity, int parentId, ClientScopeViewModel newItem)
        {
            mainEntity.AllowedScopes.Add(new ClientScope()
            {
                ClientId = parentId,
                Scope = newItem.Scope
            });
        }

        #endregion BaseClientCollectionController Implementation
    }
}