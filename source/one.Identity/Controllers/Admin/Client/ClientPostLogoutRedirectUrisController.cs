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
    public class ClientPostLogoutRedirectUrisController : BaseClientCollectionController<ClientPostLogoutRedirectUriViewModel, ClientPostLogoutRedirectUrisViewModel, ClientPostLogoutRedirectUri>
    {
        public ClientPostLogoutRedirectUrisController(ConfigurationDbContext context) : base(context)
        {
        }

        protected override IEnumerable<ClientPostLogoutRedirectUriViewModel> PopulateItemList(IdentityServer4.EntityFramework.Entities.Client client)
        {
            return client.PostLogoutRedirectUris.AsQueryable().ProjectTo<ClientPostLogoutRedirectUriViewModel>();
        }

        protected override IIncludableQueryable<IdentityServer4.EntityFramework.Entities.Client, List<ClientPostLogoutRedirectUri>> AddIncludes(DbSet<IdentityServer4.EntityFramework.Entities.Client> query)
        {
            return query.Include(c => c.PostLogoutRedirectUris);
        }

        protected override void RemoveObject(IdentityServer4.EntityFramework.Entities.Client client, int id)
        {
            var uriToDelete = client.PostLogoutRedirectUris.FirstOrDefault(uri => uri.Id == id);
            client.PostLogoutRedirectUris.Remove(uriToDelete);
        }

        protected override void AddObject(IdentityServer4.EntityFramework.Entities.Client client, int clientId, ClientPostLogoutRedirectUriViewModel newItem)
        {
            client.PostLogoutRedirectUris.Add(new ClientPostLogoutRedirectUri()
            {
                ClientId = clientId,
                PostLogoutRedirectUri = newItem.PostLogoutRedirectUri
            });
        }
    }
}
