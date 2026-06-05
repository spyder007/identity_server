using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Duende.IdentityServer.Configuration;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Validation;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Spydersoft.Identity.Models.Consent;
using Spydersoft.Identity.Models.Device;
using Spydersoft.Identity.Options;

namespace Spydersoft.Identity.Pages.Device
{
    /// <summary>Device-flow user-code capture and consent.</summary>
    public class IndexModel(
        IDeviceFlowInteractionService interaction,
        IEventService events,
        IOptions<IdentityServerOptions> options,
        ILogger<IndexModel> logger,
        IOptions<ConsentOptions> consentOptions) : PageModel
    {
        private readonly ConsentOptions _consentOptions = consentOptions.Value;

        /// <summary>The device user code.</summary>
        [BindProperty]
        public string UserCode { get; set; }

        /// <summary>The chosen button ("yes"/"no").</summary>
        [BindProperty]
        public string Button { get; set; }

        /// <summary>The scopes the user consented to.</summary>
        [BindProperty]
        public IEnumerable<string> ScopesConsented { get; set; }

        /// <summary>Whether to remember the consent decision.</summary>
        [BindProperty]
        public bool RememberConsent { get; set; }

        /// <summary>Optional device description.</summary>
        [BindProperty]
        public string Description { get; set; }

        /// <summary>True when the user-code entry form should be shown.</summary>
        public bool ShowCapture { get; set; }

        /// <summary>The consent display model (when a valid code is present).</summary>
        public DeviceAuthorizationViewModel View { get; set; }

        /// <summary>Shows the capture form, or the consent screen when a code is present.</summary>
        public async Task<IActionResult> OnGetAsync()
        {
            logger.LogInformation("Processing Device Verification Code");
            var userCodeParamName = options.Value.UserInteraction.DeviceVerificationUserCodeParameter;
            string userCode = Request.Query[userCodeParamName];
            if (string.IsNullOrWhiteSpace(userCode))
            {
                ShowCapture = true;
                return Page();
            }

            View = await BuildViewModelAsync(userCode);
            if (View == null)
            {
                return RedirectToPage("/Home/Error");
            }

            View.ConfirmUserCode = true;
            return Page();
        }

        /// <summary>Handles the submitted user code.</summary>
        public async Task<IActionResult> OnPostUserCodeCaptureAsync()
        {
            View = await BuildViewModelAsync(UserCode);
            if (View == null)
            {
                return RedirectToPage("/Home/Error");
            }

            return Page();
        }

        /// <summary>Handles the consent decision.</summary>
        public async Task<IActionResult> OnPostCallbackAsync()
        {
            var model = new DeviceAuthorizationInputModel
            {
                UserCode = UserCode,
                Button = Button,
                ScopesConsented = ScopesConsented,
                RememberConsent = RememberConsent,
                Description = Description
            };

            ProcessConsentResult result = await ProcessConsent(model);
            return result.HasValidationError ? RedirectToPage("/Home/Error") : RedirectToPage("./Success");
        }

        private async Task<ProcessConsentResult> ProcessConsent(DeviceAuthorizationInputModel model)
        {
            var result = new ProcessConsentResult();

            DeviceFlowAuthorizationRequest request = await interaction.GetAuthorizationContextAsync(model.UserCode);
            if (request == null)
            {
                return result;
            }

            ConsentResponse grantedConsent = null;

            if (model.Button == "no")
            {
                grantedConsent = new ConsentResponse { Error = AuthorizationError.AccessDenied };
                await events.RaiseAsync(new ConsentDeniedEvent(User.GetSubjectId(), request.Client.ClientId, request.ValidatedResources.RawScopeValues));
            }
            else if (model.Button == "yes")
            {
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
                        ScopesValuesConsented = [.. scopes],
                        Description = model.Description
                    };

                    await events.RaiseAsync(new ConsentGrantedEvent(User.GetSubjectId(), request.Client.ClientId, request.ValidatedResources.RawScopeValues, grantedConsent.ScopesValuesConsented, grantedConsent.RememberConsent));
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
                _ = await interaction.HandleRequestAsync(model.UserCode, grantedConsent);
                result.RedirectUri = model.ReturnUrl;
                result.Client = request.Client;
            }
            else
            {
                result.ViewModel = await BuildViewModelAsync(model.UserCode, model);
            }

            return result;
        }

        private async Task<DeviceAuthorizationViewModel> BuildViewModelAsync(string userCode, DeviceAuthorizationInputModel model = null)
        {
            DeviceFlowAuthorizationRequest request = await interaction.GetAuthorizationContextAsync(userCode);
            return request != null ? CreateConsentViewModel(userCode, model, request) : null;
        }

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

            vm.IdentityScopes = [.. request.ValidatedResources.Resources.IdentityResources.Select(x => CreateScopeViewModel(x, vm.ScopesConsented.Contains(x.Name) || model == null))];

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

        private static ScopeViewModel CreateScopeViewModel(ParsedScopeValue parsedScopeValue, ApiScope apiScope, bool check)
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