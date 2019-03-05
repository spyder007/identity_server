using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.EntityFrameworkCore;
using one.Identity.Models.ClientViewModels;

namespace one.Identity.Controllers.Admin.Client
{
    public class ClientRedirectsController : BaseClientController
    {

        public ClientRedirectsController(ConfigurationDbContext context) : base(context)
        {
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            var clientRedirectsViewModel = new ClientRedirectsViewModel();
            if (id.HasValue)
            {
                clientRedirectsViewModel.SetId(id.Value);

                var client = ConfigDbContext.Clients.Include(c => c.RedirectUris).FirstOrDefault(c => c.Id == id.Value);
                if (client == null)
                {
                    return GetErrorAction("Could not load client");
                }

                clientRedirectsViewModel.ItemsList.AddRange(client.RedirectUris.AsQueryable().ProjectTo<ClientRedirectViewModel>());
            }

            return View(clientRedirectsViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Save(int? id, ClientRedirectsViewModel clientRedirects)
        {
            if (!id.HasValue)
            {
                return GetErrorAction("No Client ID Supplied");
            }

            var client = ConfigDbContext.Clients.Include(c => c.RedirectUris).FirstOrDefault(c => c.Id == id.Value);
            if (client == null)
            {
                return GetErrorAction("Could not load client");
            }

            if (ModelState.IsValid)
            {
                client.RedirectUris.Add(new IdentityServer4.EntityFramework.Entities.ClientRedirectUri()
                {
                    ClientId = id.Value,
                    RedirectUri = clientRedirects.NewItem.RedirectUri
                });
                ConfigDbContext.Clients.Update(client);
                await ConfigDbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Edit), new { id = id.Value });
            }

            clientRedirects.SetId(id.Value);
            clientRedirects.ItemsList.AddRange(client.RedirectUris.AsQueryable().ProjectTo<ClientRedirectViewModel>());

            return View(nameof(Edit), clientRedirects);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id, int? clientId)
        {
            if (!id.HasValue)
            {
                return GetErrorAction("No Scope ID Supplied");
            }

            if (!clientId.HasValue)
            {
                return GetErrorAction("No Client ID Supplied");
            }

            var client = ConfigDbContext.Clients.Include(c => c.RedirectUris).FirstOrDefault(c => c.Id == clientId.Value);
            if (client == null)
            {
                return GetErrorAction("Could not load client");
            }

            var redirectToDelete = client.RedirectUris.FirstOrDefault(s => s.Id == id.Value);
            client.RedirectUris.Remove(redirectToDelete);
            ConfigDbContext.Clients.Update(client);
            await ConfigDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Edit), new { id = clientId.Value });
        }
    }
}
