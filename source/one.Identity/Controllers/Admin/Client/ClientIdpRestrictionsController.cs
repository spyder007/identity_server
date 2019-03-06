using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using one.Identity.Models.ClientViewModels;

namespace one.Identity.Controllers.Admin.Client
{
    public class ClientIdpRestrictionsController : BaseClientCollectionController<ClientIdpRestrictionViewModel, ClientIdpRestrictionsViewModel, ClientIdPRestriction>
    {
        public ClientIdpRestrictionsController(ConfigurationDbContext context) : base(context)
        {
        }

        protected override IEnumerable<ClientIdpRestrictionViewModel> PopulateItemList(IdentityServer4.EntityFramework.Entities.Client mainEntity)
        {
            return mainEntity.IdentityProviderRestrictions.AsQueryable().ProjectTo<ClientIdpRestrictionViewModel>();
        }

        protected override IIncludableQueryable<IdentityServer4.EntityFramework.Entities.Client, List<ClientIdPRestriction>> AddIncludes(DbSet<IdentityServer4.EntityFramework.Entities.Client> query)
        {
            return query.Include(c => c.IdentityProviderRestrictions);
        }

        protected override void RemoveObject(IdentityServer4.EntityFramework.Entities.Client mainEntity, int id)
        {
            var idpToRemove = mainEntity.IdentityProviderRestrictions.FirstOrDefault(idp => idp.Id == id);
            mainEntity.IdentityProviderRestrictions.Remove(idpToRemove);
        }

        protected override void AddObject(IdentityServer4.EntityFramework.Entities.Client mainEntity, int parentId, ClientIdpRestrictionViewModel newItem)
        {
            mainEntity.IdentityProviderRestrictions.Add(new ClientIdPRestriction()
            {
                ClientId = parentId,
                Provider = newItem.Provider
            });
        }
    }
}
