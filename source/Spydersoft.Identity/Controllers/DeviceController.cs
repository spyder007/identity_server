// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Duende.IdentityServer.Configuration;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Validation;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Spydersoft.Identity.Attributes;
using Spydersoft.Identity.Models.Consent;
using Spydersoft.Identity.Models.Device;
using Spydersoft.Identity.Options;

namespace Spydersoft.Identity.Controllers
{
    /// <summary>
    /// Class DeviceController.
    /// Implements the <see cref="Controller" />
    /// </summary>
    /// <seealso cref="Controller" />
    [Authorize]
    [SecurityHeaders]
    public class DeviceController(
        IDeviceFlowInteractionService interaction,
        IEventService eventService,
        IOptions<IdentityServerOptions> options,
        ILogger<DeviceController> logger,
        IOptions<ConsentOptions> consentOptions) : Controller
    {
        /// <summary>
        /// The interaction
        /// </summary>
        private readonly IDeviceFlowInteractionService _interaction = interaction;
        /// <summary>
        /// The events
        /// </summary>
        private readonly IEventService _events = eventService;
        /// <summary>
        /// The options
        /// </summary>
        private readonly IOptions<IdentityServerOptions> _options = options;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<DeviceController> _logger = logger;
        /// <summary>
        /// The consent options
        /// </summary>
        private readonly ConsentOptions _consentOptions = consentOptions.Value;

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>Microsoft.AspNetCore.Mvc.IActionResult.</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Processing Device Verification Code");
            var userCodeParamName = _options.Value.UserInteraction.DeviceVerificationUserCodeParameter;
            string userCode = Request.Query[userCodeParamName];
            if (string.IsNullOrWhiteSpace(userCode))
            {
                return View("UserCodeCapture");
            }

            DeviceAuthorizationViewModel vm = await BuildViewModelAsync(userCode);
            if (vm == null)
            {
                return View("Error");
            }

            vm.ConfirmUserCode = true;
            return View("UserCodeConfirmation", vm);
        }

        /// <summary>
        /// Users the code capture.
        /// </summary>
        /// <param name="userCode">The user code.</param>
        /// <returns>Microsoft.AspNetCore.Mvc.IActionResult.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserCodeCapture(string userCode)
        {
            DeviceAuthorizationViewModel vm = await BuildViewModelAsync(userCode);
            return vm == null ? View("Error") : (IActionResult)View("UserCodeConfirmation", vm);
        }

        /// <summary>
        /// Callbacks the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Microsoft.AspNetCore.Mvc.IActionResult.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Callback(DeviceAuthorizationInputModel model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            ProcessConsentResult result = await ProcessConsent(model);
            return result.HasValidationError ? View("Error") : (IActionResult)View("Success");
        }

        /// <summary>
        /// Processes the consent.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Spydersoft.Identity.Models.Consent.ProcessConsentResult.</returns>
        private async Task<ProcessConsentResult> ProcessConsent(DeviceAuthorizationInputModel model)
        {
            var result = new ProcessConsentResult();

            DeviceFlowAuthorizationRequest request = await _interaction.GetAuthorizationContextAsync(model.UserCode);
            if (request == null)
            {
                return result;
            }

            ConsentResponse grantedConsent = null;

            // user clicked 'no' - send back the standard 'access_denied' response
            if (model.Button == "no")
            {
                grantedConsent = new ConsentResponse { Error = AuthorizationError.AccessDenied };

                // emit event
                await _events.RaiseAsync(new ConsentDeniedEvent(User.GetSubjectId(), request.Client.ClientId, request.ValidatedResources.RawScopeValues));
            }
            // user clicked 'yes' - validate the data
            else if (model.Button == "yes")
            {
                // if the user consented to some scope, build the response model
                if (model.ScopesConsented != null && model.ScopesConsented.Any())
                {
                    IEnumerable<string> scopes = model.ScopesConsented;
                    if (!_consentOptions.EnableOfflineAccess)
                    {
                        scopes = scopes.Where(x => x != Duende.IdentityServer.IdentityServerConstants.StandardScopes.OfflineAccess);
                    }

                    grantedConsent = new ConsentResponse
                    {
                        RememberConsent = model.RememberConsent,
                        ScopesValuesConsented = scopes.ToArray(),
                        Description = model.Description
                    };

                    // emit event
                    await _events.RaiseAsync(new ConsentGrantedEvent(User.GetSubjectId(), request.Client.ClientId, request.ValidatedResources.RawScopeValues, grantedConsent.ScopesValuesConsented, grantedConsent.RememberConsent));
                }
                else
                {
                    result.ValidationError = _consentOptions.MustChooseOneErrorMessage;
                }
            }
            else
            {
                result.ValidationError = _consentOptions.InvalidSelectionErrorMessage;
            }

            if (grantedConsent != null)
            {
                // communicate outcome of consent back to identityserver
                _ = await _interaction.HandleRequestAsync(model.UserCode, grantedConsent);

                // indicate that's it ok to redirect back to authorization endpoint
                result.RedirectUri = model.ReturnUrl;
                result.Client = request.Client;
            }
            else
            {
                // we need to redisplay the consent UI
                result.ViewModel = await BuildViewModelAsync(model.UserCode, model);
            }

            return result;
        }

        /// <summary>
        /// Build view model as an asynchronous operation.
        /// </summary>
        /// <param name="userCode">The user code.</param>
        /// <param name="model">The model.</param>
        /// <returns>A Task&lt;Spydersoft.Identity.Models.Device.DeviceAuthorizationViewModel&gt; representing the asynchronous operation.</returns>
        private async Task<DeviceAuthorizationViewModel> BuildViewModelAsync(string userCode, DeviceAuthorizationInputModel model = null)
        {
            DeviceFlowAuthorizationRequest request = await _interaction.GetAuthorizationContextAsync(userCode);
            return request != null ? CreateConsentViewModel(userCode, model, request) : null;
        }

        /// <summary>
        /// Creates the consent view model.
        /// </summary>
        /// <param name="userCode">The user code.</param>
        /// <param name="model">The model.</param>
        /// <param name="request">The request.</param>
        /// <returns>Spydersoft.Identity.Models.Device.DeviceAuthorizationViewModel.</returns>
        private DeviceAuthorizationViewModel CreateConsentViewModel(string userCode, DeviceAuthorizationInputModel model, DeviceFlowAuthorizationRequest request)
        {
            var vm = new DeviceAuthorizationViewModel
            {
                UserCode = userCode,
                Description = model?.Description,

                RememberConsent = model?.RememberConsent ?? true,
                ScopesConsented = model?.ScopesConsented ?? [],

                ClientName = request.Client.ClientName ?? request.Client.ClientId,
                ClientUrl = request.Client.ClientUri,
                ClientLogoUrl = request.Client.LogoUri,
                AllowRememberConsent = request.Client.AllowRememberConsent
            };

            vm.IdentityScopes = request.ValidatedResources.Resources.IdentityResources.Select(x => CreateScopeViewModel(x, vm.ScopesConsented.Contains(x.Name) || model == null)).ToArray();

            var apiScopes = new List<ScopeViewModel>();
            foreach (ParsedScopeValue parsedScope in request.ValidatedResources.ParsedScopes)
            {
                ApiScope apiScope = request.ValidatedResources.Resources.FindApiScope(parsedScope.ParsedName);
                if (apiScope != null)
                {
                    ScopeViewModel scopeVm = CreateScopeViewModel(parsedScope, apiScope, vm.ScopesConsented.Contains(parsedScope.RawValue) || model == null);
                    apiScopes.Add(scopeVm);
                }
            }
            if (_consentOptions.EnableOfflineAccess && request.ValidatedResources.Resources.OfflineAccess)
            {
                apiScopes.Add(GetOfflineAccessScope(vm.ScopesConsented.Contains(Duende.IdentityServer.IdentityServerConstants.StandardScopes.OfflineAccess) || model == null));
            }
            vm.ApiScopes = apiScopes;

            return vm;
        }

        /// <summary>
        /// Creates the scope view model.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <param name="check">The check.</param>
        /// <returns>Spydersoft.Identity.Models.Consent.ScopeViewModel.</returns>
        private static ScopeViewModel CreateScopeViewModel(IdentityResource identity, bool check)
        {
            return new ScopeViewModel
            {
                Value = identity.Name,
                DisplayName = identity.DisplayName ?? identity.Name,
                Description = identity.Description,
                Emphasize = identity.Emphasize,
                Required = identity.Required,
                Checked = check || identity.Required
            };
        }

        /// <summary>
        /// Creates the scope view model.
        /// </summary>
        /// <param name="parsedScopeValue">The parsed scope value.</param>
        /// <param name="apiScope">The API scope.</param>
        /// <param name="check">The check.</param>
        /// <returns>Spydersoft.Identity.Models.Consent.ScopeViewModel.</returns>
        public static ScopeViewModel CreateScopeViewModel(ParsedScopeValue parsedScopeValue, ApiScope apiScope, bool check)
        {
            return new ScopeViewModel
            {
                Value = parsedScopeValue.RawValue,
                DisplayName = apiScope.DisplayName ?? apiScope.Name,
                Description = apiScope.Description,
                Emphasize = apiScope.Emphasize,
                Required = apiScope.Required,
                Checked = check || apiScope.Required
            };
        }
        /// <summary>
        /// Gets the offline access scope.
        /// </summary>
        /// <param name="check">The check.</param>
        /// <returns>Spydersoft.Identity.Models.Consent.ScopeViewModel.</returns>
        private ScopeViewModel GetOfflineAccessScope(bool check)
        {
            return new ScopeViewModel
            {
                Value = Duende.IdentityServer.IdentityServerConstants.StandardScopes.OfflineAccess,
                DisplayName = _consentOptions.OfflineAccessDisplayName,
                Description = _consentOptions.OfflineAccessDescription,
                Emphasize = true,
                Checked = check
            };
        }
    }
}