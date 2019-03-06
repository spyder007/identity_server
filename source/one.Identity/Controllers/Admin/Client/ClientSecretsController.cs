using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using one.Identity.Models.ClientViewModels;

namespace one.Identity.Controllers.Admin.Client
{
    public class ClientSecretsController : BaseClientCollectionController<ClientSecretViewModel, ClientSecretsViewModel, ClientSecret>
    {
        public ClientSecretsController(ConfigurationDbContext context) : base(context)
        {
        }

        protected override IEnumerable<ClientSecretViewModel> PopulateItemList(IdentityServer4.EntityFramework.Entities.Client client)
        {
            return client.ClientSecrets.AsQueryable().ProjectTo<ClientSecretViewModel>();
        }

        protected override IIncludableQueryable<IdentityServer4.EntityFramework.Entities.Client, List<ClientSecret>> AddIncludes(DbSet<IdentityServer4.EntityFramework.Entities.Client> query)
        {
            return query.Include(c => c.ClientSecrets);
        }

        protected override void RemoveObject(IdentityServer4.EntityFramework.Entities.Client client, int id)
        {
            var secret = client.ClientSecrets.FirstOrDefault(s => s.Id == id);
            client.ClientSecrets.Remove(secret);
        }

        protected override void AddObject(IdentityServer4.EntityFramework.Entities.Client client, int clientId, ClientSecretViewModel newItem)
        {
            client.ClientSecrets.Add(new ClientSecret()
            {
                ClientId = clientId,
                Created = DateTime.UtcNow,
                Description = newItem.Description,
                Expiration = newItem.Expiration,
                Type = newItem.Type,
                Value = newItem.Value.Sha256()
            });
        }
    }
}
