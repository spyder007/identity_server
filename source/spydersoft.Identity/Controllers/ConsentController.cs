// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using spydersoft.Identity.Attributes;
using spydersoft.Identity.Models;
using spydersoft.Identity.Services;
using spydersoft.Identity.Models.Consent;

namespace spydersoft.Identity.Controllers
{
    /// <summary>
    /// This controller processes the consent UI
    /// </summary>
    [SecurityHeaders]
    public class ConsentController : Controller
    {
        private readonly ConsentService _consent;

        public ConsentController(
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IResourceStore resourceStore,
            ILogger<ConsentController> logger)
        {
            _consent = new ConsentService(interaction, clientStore, resourceStore, logger);
        }

        /// <summary>
        /// Shows the consent screen
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index(string returnUrl)
        {
            //var vm = await _consent.BuildViewModelAsync(returnUrl);

            var vm = new ConsentViewModel
            {
                ClientName = "test",
                AllowRememberConsent = true,
                ClientUrl = "http://google.com",
                ReturnUrl = "http://google.com",
                Button = "hi",
                RememberConsent = false,
                IdentityScopes = new List<ScopeViewModel>()
                {
                    new ScopeViewModel()
                    {
                        Checked = true,
                        Description = "test one",
                        DisplayName = "test 1",
                        Emphasize = true,
                        Name = "test1",
                        Required = true
                    },
                    new ScopeViewModel()
                    {
                        Checked = false,
                        Description = "test two",
                        DisplayName = "test 2",
                        Emphasize = false,
                        Name = "test2",
                        Required = false
                    }
                },
                ResourceScopes = new List<ScopeViewModel>()
                {
                    new ScopeViewModel()
                    {
                        Checked = true,
                        Description = "test three",
                        DisplayName = "test 3",
                        Emphasize = true,
                        Name = "test3",
                        Required = true
                    },
                    new ScopeViewModel()
                    {
                        Checked = false,
                        Description = "test four",
                        DisplayName = "test 4",
                        Emphasize = false,
                        Name = "test4",
                        Required = false
                    }
                }
            };

            if (vm != null)
            {
                return View("Index", vm);
            }

            return View("Error");
        }

        /// <summary>
        /// Handles the consent screen postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ConsentInputModel model)
        {
            var result = await _consent.ProcessConsent(model);

            if (result.IsRedirect)
            {
                return Redirect(result.RedirectUri);
            }

            if (result.HasValidationError)
            {
                ModelState.AddModelError("", result.ValidationError);
            }

            if (result.ShowView)
            {
                return View("Index", result.ViewModel);
            }

            return View("Error");
        }
    }
}