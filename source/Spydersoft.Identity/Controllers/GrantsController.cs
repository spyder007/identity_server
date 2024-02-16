// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Duende.IdentityServer.Events;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Spydersoft.Identity.Attributes;
using Spydersoft.Identity.Models.Grants;

namespace Spydersoft.Identity.Controllers
{
    /// <summary>
    /// This sample controller allows a user to revoke grants given to clients
    /// </summary>
    [SecurityHeaders]
    [Authorize]
    public class GrantsController(IIdentityServerInteractionService interaction,
        IClientStore clients,
        IResourceStore resources,
        IEventService events) : Controller
    {
        /// <summary>
        /// The interaction
        /// </summary>
        private readonly IIdentityServerInteractionService _interaction = interaction;
        /// <summary>
        /// The clients
        /// </summary>
        private readonly IClientStore _clients = clients;
        /// <summary>
        /// The resources
        /// </summary>
        private readonly IResourceStore _resources = resources;
        /// <summary>
        /// The events
        /// </summary>
        private readonly IEventService _events = events;

        /// <summary>
        /// Show list of grants
        /// </summary>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View("Index", await BuildViewModelAsync());
        }

        /// <summary>
        /// Handle postback to revoke a client
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Revoke(string clientId)
        {
            await _interaction.RevokeUserConsentAsync(clientId);
            await _events.RaiseAsync(new GrantsRevokedEvent(User.GetSubjectId(), clientId));

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Build view model as an asynchronous operation.
        /// </summary>
        /// <returns>A Task&lt;GrantsViewModel&gt; representing the asynchronous operation.</returns>
        private async Task<GrantsViewModel> BuildViewModelAsync()
        {
            IEnumerable<Duende.IdentityServer.Models.Grant> grants = await _interaction.GetAllUserGrantsAsync();

            var list = new List<GrantViewModel>();
            foreach (Duende.IdentityServer.Models.Grant grant in grants)
            {
                Duende.IdentityServer.Models.Client client = await _clients.FindClientByIdAsync(grant.ClientId);
                if (client != null)
                {
                    Duende.IdentityServer.Models.Resources resourcesByScope = await _resources.FindResourcesByScopeAsync(grant.Scopes);

                    var item = new GrantViewModel()
                    {
                        ClientId = client.ClientId,
                        ClientName = client.ClientName ?? client.ClientId,
                        ClientLogoUrl = client.LogoUri,
                        ClientUrl = client.ClientUri,
                        Description = grant.Description,
                        Created = grant.CreationTime,
                        Expires = grant.Expiration,
                        IdentityGrantNames = resourcesByScope.IdentityResources.Select(x => x.DisplayName ?? x.Name).ToArray(),
                        ApiGrantNames = resourcesByScope.ApiScopes.Select(x => x.DisplayName ?? x.Name).ToArray()
                    };

                    list.Add(item);
                }
            }

            return new GrantsViewModel
            {
                Grants = list
            };
        }
    }
}