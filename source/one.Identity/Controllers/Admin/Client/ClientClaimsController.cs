﻿using AutoMapper.QueryableExtensions;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using one.Identity.Models.ClientViewModels;
using System.Collections.Generic;
using System.Linq;

namespace one.Identity.Controllers.Admin.Client
{
    public class ClientClaimsController : BaseClientCollectionController<ClientClaimViewModel, ClientClaimsViewModel, ClientClaim>
    {
        public ClientClaimsController(ConfigurationDbContext context) : base(context)
        {
        }

        protected override IEnumerable<ClientClaimViewModel> PopulateItemList(IdentityServer4.EntityFramework.Entities.Client mainEntity)
        {
            return mainEntity.Claims.AsQueryable().ProjectTo<ClientClaimViewModel>();
        }

        protected override IIncludableQueryable<IdentityServer4.EntityFramework.Entities.Client, List<ClientClaim>> AddIncludes(DbSet<IdentityServer4.EntityFramework.Entities.Client> query)
        {
            return query.Include(c => c.Claims);
        }

        protected override void RemoveObject(IdentityServer4.EntityFramework.Entities.Client mainEntity, int id)
        {
            var claimToDelete = mainEntity.Claims.FirstOrDefault(s => s.Id == id);
            mainEntity.Claims.Remove(claimToDelete);
        }

        protected override void AddObject(IdentityServer4.EntityFramework.Entities.Client mainEntity, int parentId, ClientClaimViewModel newItem)
        {
            mainEntity.Claims.Add(new ClientClaim()
            {
                ClientId = parentId,
                Type = newItem.Type,
                Value = newItem.Value
            });
        }
    }
}