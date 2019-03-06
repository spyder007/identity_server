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

        protected override IEnumerable<ClientIdpRestrictionViewModel> PopulateItemList(IdentityServer4.EntityFramework.Entities.Client client)
        {
            return client.IdentityProviderRestrictions.AsQueryable().ProjectTo<ClientIdpRestrictionViewModel>();
        }

        protected override IIncludableQueryable<IdentityServer4.EntityFramework.Entities.Client, List<ClientIdPRestriction>> AddIncludes(DbSet<IdentityServer4.EntityFramework.Entities.Client> query)
        {
            return query.Include(c => c.IdentityProviderRestrictions);
        }

        protected override void RemoveObject(IdentityServer4.EntityFramework.Entities.Client client, int id)
        {
            var idpToRemove = client.IdentityProviderRestrictions.FirstOrDefault(idp => idp.Id == id);
            client.IdentityProviderRestrictions.Remove(idpToRemove);
        }

        protected override void AddObject(IdentityServer4.EntityFramework.Entities.Client client, int clientId, ClientIdpRestrictionViewModel newItem)
        {
            client.IdentityProviderRestrictions.Add(new ClientIdPRestriction()
            {
                ClientId = clientId,
                Provider = newItem.Provider
            });
        }
    }
}
