using AutoMapper.QueryableExtensions;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using one.Identity.Models.ClientViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace one.Identity.Controllers.Admin.Client
{
    public class ClientScopesController : BaseClientController
    {
        #region Constructor

        public ClientScopesController(ConfigurationDbContext dbContext) : base(dbContext)
        {
        }

        #endregion Constructor

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            ClientScopesViewModel scopesViewModel = new ClientScopesViewModel();
            if (id.HasValue)
            {
                scopesViewModel.SetId(id.Value);

                var client = ConfigDbContext.Clients.Include(c => c.AllowedScopes).FirstOrDefault(c => c.Id == id.Value);
                if (client == null)
                {
                    return GetErrorAction("Could not load client");
                }

                scopesViewModel.ItemsList.AddRange(client.AllowedScopes.AsQueryable().ProjectTo<ClientScopeViewModel>());
            }

            return View(scopesViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Save(int? id, ClientScopesViewModel clientScopes)
        {
            if (!id.HasValue)
            {
                return GetErrorAction("No Client ID Supplied");
            }

            var client = ConfigDbContext.Clients.Include(c => c.AllowedScopes).FirstOrDefault(c => c.Id == id.Value);
            if (client == null)
            {
                return GetErrorAction("Could not load client");
            }

            if (ModelState.IsValid)
            {
                client.AllowedScopes.Add(new IdentityServer4.EntityFramework.Entities.ClientScope()
                {
                    ClientId = id.Value,
                    Scope = clientScopes.NewItem.Scope
                });
                ConfigDbContext.Clients.Update(client);
                await ConfigDbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Edit), new { id = id.Value });
            }

            clientScopes.ItemsList.AddRange(client.AllowedScopes.AsQueryable().ProjectTo<ClientScopeViewModel>());
            clientScopes.SetId(id.Value);

            return View(nameof(Edit), clientScopes);
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

            var client = ConfigDbContext.Clients.Include(c => c.AllowedScopes).FirstOrDefault(c => c.Id == clientId.Value);
            if (client == null)
            {
                return GetErrorAction("Could not load client");
            }

            var scopeToDelete = client.AllowedScopes.FirstOrDefault(s => s.Id == id.Value);
            client.AllowedScopes.Remove(scopeToDelete);
            ConfigDbContext.Clients.Update(client);
            await ConfigDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Edit), new { id = clientId.Value });
        }
    }
}