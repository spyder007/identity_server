using System;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Duende.IdentityServer.EntityFramework.DbContexts;

using Microsoft.AspNetCore.Mvc;

using Spydersoft.Identity.Constants;
using Spydersoft.Identity.Core.Models.Admin.ClientViewModels;

namespace Spydersoft.Identity.Controllers.Admin.Client
{
    /// <summary>
    /// Class ClientsController.
    /// Implements the <see cref="BaseAdminController" />
    /// </summary>
    /// <seealso cref="BaseAdminController" />
    public class ClientsController(ConfigurationDbContext dbContext, IMapper mapper) : BaseAdminController(dbContext, mapper)
    {

        #region Client List Actions

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>IActionResult.</returns>
        public IActionResult Index()
        {
            ViewData["Title"] = "Registered Clients";

            var clientsModel = new ClientsViewModel
            {
                Clients = Mapper.ProjectTo<ClientViewModel>(ConfigDbContext.Clients.AsQueryable())
            };

            return View(clientsModel);
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (!ModelState.IsValid)
            {
                return GetErrorAction(Messages.InvalidRequest);
            }
            if (id.HasValue)
            {
                Duende.IdentityServer.EntityFramework.Entities.Client client = ConfigDbContext.Clients.FirstOrDefault(c => c.Id == id.Value);
                if (client == null)
                {
                    return GetErrorAction("Could not load client");
                }

                _ = ConfigDbContext.Clients.Remove(client);
                _ = await ConfigDbContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        #endregion Client List Actions

        #region Main Tab
        /// <summary>
        /// Edits the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (!ModelState.IsValid)
            {
                return GetErrorAction(Messages.InvalidRequest);
            }
            ClientViewModel clientModel;
            if (!id.HasValue)
            {
                clientModel = new ClientViewModel
                {
                    // Set default values for required fields
                    ProtocolType = "oidc",
                    AccessTokenLifetime = 3600,
                    IdentityTokenLifetime = 300,
                    AuthorizationCodeLifetime = 300,
                    AbsoluteRefreshTokenLifetime = 2592000,
                    SlidingRefreshTokenLifetime = 1296000,
                    RefreshTokenExpiration = 1,
                    RefreshTokenUsage = 1,
                    DeviceCodeLifetime = 300,
                    AccessTokenType = 0,
                    Enabled = true,
                    RequireClientSecret = true,
                    Created = DateTime.UtcNow
                };
                ViewData["Title"] = "New Client";
            }
            else
            {
                Duende.IdentityServer.EntityFramework.Entities.Client client = ConfigDbContext.Clients.FirstOrDefault(c => c.Id == id.Value);
                if (client == null)
                {
                    return GetErrorAction("Could not load client");
                }

                clientModel = new ClientViewModel(client.Id);
                _ = Mapper.Map(client, clientModel);
                ViewData["Title"] = $"Edit {clientModel.NavBar.Name}";
            }

            return View(clientModel);
        }

        /// <summary>
        /// Edits the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="client">The client.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        public async Task<IActionResult> Edit(int? id, ClientViewModel client)
        {
            if (ModelState.IsValid)
            {
                Duende.IdentityServer.EntityFramework.Entities.Client dbEntity;
                var isNew = false;

                if (!id.HasValue || id.Value == 0)
                {
                    dbEntity = new Duende.IdentityServer.EntityFramework.Entities.Client
                    {
                        Created = DateTime.UtcNow
                    };
                    _ = ConfigDbContext.Add(dbEntity);
                    isNew = true;
                }
                else
                {
                    client.Id = id.Value;
                    dbEntity = ConfigDbContext.Clients.FirstOrDefault(c => c.Id == id.Value);
                }

                if (dbEntity != null)
                {
                    _ = Mapper.Map(client, dbEntity);
                    if (!isNew)
                    {
                        dbEntity.Updated = DateTime.UtcNow;
                    }
                }

                if (!isNew)
                {
                    _ = ConfigDbContext.Update(dbEntity);
                }

                _ = await ConfigDbContext.SaveChangesAsync();

                return isNew ? RedirectToAction(nameof(Edit), new { id = dbEntity.Id }) : RedirectToAction(nameof(Index));
            }

            // If we got here, validation failed
            if (id.HasValue)
            {
                client.Id = id.Value;
            }

            // Ensure NavBar is initialized for the view
            if (client.NavBar == null)
            {
                client.NavBar = new NavBarViewModel(client);
            }

            ViewData["Title"] = client.Id == 0 ? "New Client" : $"Edit {client.ClientName}";
            return View(client);
        }

        #endregion Main Tab
    }
}