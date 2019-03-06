using AutoMapper.QueryableExtensions;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using one.Identity.Models.ClientViewModels;
using System.Collections.Generic;
using System.Linq;
using IS4Entities = IdentityServer4.EntityFramework.Entities;

namespace one.Identity.Controllers.Admin.Client
{
    public class ClientRedirectsController : BaseClientCollectionController<ClientRedirectViewModel, ClientRedirectsViewModel, IS4Entities.ClientRedirectUri>
    {
        public ClientRedirectsController(ConfigurationDbContext context) : base(context)
        {
        }

        #region BaseClientCollectinoController Implementation

        protected override IEnumerable<ClientRedirectViewModel> PopulateItemList(IS4Entities.Client client)
        {
            return client.RedirectUris.AsQueryable().ProjectTo<ClientRedirectViewModel>();
        }

        protected override IIncludableQueryable<IS4Entities.Client, List<IS4Entities.ClientRedirectUri>> AddIncludes(DbSet<IS4Entities.Client> query)
        {
            return query.Include(c => c.RedirectUris);
        }

        protected override void RemoveObject(IS4Entities.Client client, int id)
        {
            var redirectToDelete = client.RedirectUris.FirstOrDefault(s => s.Id == id);
            client.RedirectUris.Remove(redirectToDelete);
        }

        protected override void AddObject(IS4Entities.Client client, int clientId, ClientRedirectViewModel newItem)
        {
            client.RedirectUris.Add(new IS4Entities.ClientRedirectUri()
            {
                ClientId = clientId,
                RedirectUri = newItem.RedirectUri
            });
        }

        #endregion BaseClientCollectinoController Implementation
    }
}