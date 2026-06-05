using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Duende.IdentityServer.Events;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Validation;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Spydersoft.Identity.Extensions;
using Spydersoft.Identity.Models.Consent;
using Spydersoft.Identity.Options;

namespace Spydersoft.Identity.Pages.Consent
{
    /// <summary>OAuth consent screen.</summary>
    public class IndexModel(
        IIdentityServerInteractionService interaction,
        IEventService events,
        ILogger<IndexModel> logger,
        IOptions<ConsentOptions> consentOptions) : PageModel
    {
        private readonly ConsentOptions _consentOptions = consentOptions.Value;

        /// <summary>The return URL for the authorization request.</summary>
        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; }

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

        /// <summary>The display model.</summary>
        public ConsentViewModel View { get; set; }

        /// <summary>Shows the consent screen.</summary>
        public async Task<IActionResult> OnGetAsync()
        {
            View = await BuildViewModelAsync(ReturnUrl);
            return View != null ? Page() : RedirectToPage("/Home/Error");
        }

        /// <summary>Handles the consent decision.</summary>
        public async Task<IActionResult> OnPostAsync()
        {
            var model = new ConsentInputModel
            {
                ReturnUrl = ReturnUrl,
                Button = Button,
                ScopesConsented = ScopesConsented,
                RememberConsent = RememberConsent,
                Description = Description
            };

            ProcessConsentResult result = await ProcessConsent(model);

            if (result.IsRedirect)
            {
                AuthorizationRequest context = await interaction.GetAuthorizationContextAsync(ReturnUrl);
                if (context?.IsNativeClient() == true)
                {
                    return RedirectToPage("/Redirect/Index", new { redirectUri = result.RedirectUri });
                }

                return Redirect(result.RedirectUri);
            }

            if (result.HasValidationError)
            {
                ModelState.AddModelError(string.Empty, result.ValidationError);
            }

            if (result.ShowView)
            {
                View = result.ViewModel;
                return Page();
            }

            return RedirectToPage("/Home/Error");
        }

        private async Task<ProcessConsentResult> ProcessConsent(ConsentInputModel model)
        {
            var result = new ProcessConsentResult();

            AuthorizationRequest request = await interaction.GetAuthorizationContextAsync(model.ReturnUrl);
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
                await interaction.GrantConsentAsync(request, grantedConsent);
                result.RedirectUri = model.ReturnUrl;
                result.Client = request.Client;
            }
            else
            {
                result.ViewModel = await BuildViewModelAsync(model.ReturnUrl, model);
            }

            return result;
        }

        private async Task<ConsentViewModel> BuildViewModelAsync(string returnUrl, ConsentInputModel model = null)
        {
            AuthorizationRequest request = await interaction.GetAuthorizationContextAsync(returnUrl);
            if (request != null)
            {
                return CreateConsentViewModel(model, returnUrl, request);
            }

            logger.LogError("No consent request matching request: {ReturnUrl}", returnUrl);
            return null;
        }

        private ConsentViewModel CreateConsentViewModel(ConsentInputModel model, string returnUrl, AuthorizationRequest request)
        {
            var vm = new ConsentViewModel
            {
                RememberConsent = model?.RememberConsent ?? true,
                ScopesConsented = model?.ScopesConsented ?? [],
                Description = model?.Description,
                ReturnUrl = returnUrl,
                ClientName = request.Client.ClientName ?? request.Client.ClientId,
                ClientUrl = request.Client.ClientUri,
                ClientLogoUrl = request.Client.LogoUri,
                AllowRememberConsent = request.Client.AllowRememberConsent
            };

            vm.IdentityScopes = [.. request.ValidatedResources.Resources.IdentityResources.Select(x => x.CreateScopeViewModel(vm.ScopesConsented.Contains(x.Name) || model == null))];

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

        private static ScopeViewModel CreateScopeViewModel(ParsedScopeValue parsedScopeValue, ApiScope apiScope, bool check)
        {
            var displayName = apiScope.DisplayName ?? apiScope.Name;
            if (!string.IsNullOrWhiteSpace(parsedScopeValue.ParsedParameter))
            {
                displayName += ":" + parsedScopeValue.ParsedParameter;
            }

            return new ScopeViewModel
            {
                Value = parsedScopeValue.RawValue,
                DisplayName = displayName,
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