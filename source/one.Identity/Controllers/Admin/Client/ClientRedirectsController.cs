using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using one.Identity.Models.ClientViewModels;

namespace one.Identity.Controllers.Admin.Client
{
    public class ClientRedirectsController : BaseClientCollectionController<ClientRedirectViewModel, ClientRedirectsViewModel, ClientRedirectUri>
    {

        public ClientRedirectsController(ConfigurationDbContext context) : base(context)
        {
        }


        protected override IEnumerable<ClientRedirectViewModel> PopulateItemList(IdentityServer4.EntityFramework.Entities.Client client)
        {
            return client.RedirectUris.AsQueryable().ProjectTo<ClientRedirectViewModel>();
        }

        protected override IIncludableQueryable<IdentityServer4.EntityFramework.Entities.Client, List<ClientRedirectUri>> AddIncludes(DbSet<IdentityServer4.EntityFramework.Entities.Client> query)
        {
            return query.Include(c => c.RedirectUris);
        }

        protected override void RemoveObject(IdentityServer4.EntityFramework.Entities.Client client, int id)
        {
            var redirectToDelete = client.RedirectUris.FirstOrDefault(s => s.Id == id);
            client.RedirectUris.Remove(redirectToDelete);
        }

        protected override void AddObject(IdentityServer4.EntityFramework.Entities.Client client, int clientId, ClientRedirectViewModel newItem)
        {
            client.RedirectUris.Add(new IdentityServer4.EntityFramework.Entities.ClientRedirectUri()
            {
                ClientId = clientId,
                RedirectUri = newItem.RedirectUri
            });
        }

        protected override IActionResult GetView(ClientRedirectsViewModel model)
        {
            return View(nameof(Edit), model);
        }

        protected override IActionResult GetEditRedirect(object routeValues)
        {
            return RedirectToAction(nameof(Edit), routeValues);
        }
    }
}
