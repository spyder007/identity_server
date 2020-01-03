using System.Security.Claims;
using AutoMapper;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Models;
using spydersoft.Identity.Models.Admin.ApiViewModels;
using spydersoft.Identity.Models.Admin.ClientViewModels;
using spydersoft.Identity.Models.Admin.IdentityResourceViewModels;
using spydersoft.Identity.Models.Identity;
using ApiResource = IdentityServer4.EntityFramework.Entities.ApiResource;
using Client = IdentityServer4.EntityFramework.Entities.Client;
using IdentityResource = IdentityServer4.EntityFramework.Entities.IdentityResource;

namespace spydersoft.Identity.Data
{
    public class AutoMapper : Profile
    {

        public AutoMapper()
        {
            PopulateIdentityMappings();
            PopulateClientMappings();
            PopulateApiResourceMappings();
            PopulateIdentityResourceMappings();
        }

        private void PopulateIdentityMappings()
        {
            CreateMap<ApplicationRole, ApplicationRole>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.ConcurrencyStamp, opt => opt.Ignore());

            CreateMap<ApplicationUser, ApplicationUser>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.ConcurrencyStamp, opt => opt.Ignore())
                .ForMember(d => d.SecurityStamp, opt => opt.Ignore())
                .ForMember(d => d.PasswordHash, opt => opt.Ignore());

            CreateMap<ClaimModel, Claim>()
                .ForMember(d => d.Issuer, opt => opt.Ignore())
                .ForMember(d => d.OriginalIssuer, opt => opt.Ignore())
                .ForMember(d => d.Properties, opt => opt.Ignore())
                .ForMember(d => d.Subject, opt => opt.Ignore())
                .ForMember(d => d.ValueType, opt => opt.Ignore())
                .ReverseMap();
        }

        private void PopulateClientMappings()
        {
            CreateMap<ClientViewModel, Client>()
                .ForMember(d => d.AllowedCorsOrigins, opt => opt.Ignore())
                .ForMember(d => d.AllowedGrantTypes, opt => opt.Ignore())
                .ForMember(d => d.AllowedScopes, opt => opt.Ignore())
                .ForMember(d => d.Claims, opt => opt.Ignore())
                .ForMember(d => d.ClientSecrets, opt => opt.Ignore())
                .ForMember(d => d.IdentityProviderRestrictions, opt => opt.Ignore())
                .ForMember(d => d.PostLogoutRedirectUris, opt => opt.Ignore())
                .ForMember(d => d.Properties, opt => opt.Ignore())
                .ForMember(d => d.RedirectUris, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(d => d.Id, opt => opt.Ignore());

            CreateMap<ClientClaimViewModel, ClientClaim>()
                .ForMember(d => d.Client, opt => opt.Ignore())
                .ForMember(d => d.ClientId, opt => opt.MapFrom(src => src.ParentId))
                .ReverseMap();

            CreateMap<ClientCorsOriginViewModel, ClientCorsOrigin>()
                .ForMember(d => d.Client, opt => opt.Ignore())
                .ForMember(d => d.ClientId, opt => opt.MapFrom(src => src.ParentId))
                .ReverseMap();

            CreateMap<ClientGrantTypeViewModel, ClientGrantType>()
                .ForMember(d => d.Client, opt => opt.Ignore())
                .ForMember(d => d.ClientId, opt => opt.MapFrom(src => src.ParentId))
                .ReverseMap();

            CreateMap<ClientIdpRestrictionViewModel, ClientIdPRestriction>()
                .ForMember(d => d.Client, opt => opt.Ignore())
                .ForMember(d => d.ClientId, opt => opt.MapFrom(src => src.ParentId))
                .ReverseMap();

            CreateMap<ClientPostLogoutRedirectUriViewModel, ClientPostLogoutRedirectUri>()
                .ForMember(d => d.Client, opt => opt.Ignore())
                .ForMember(d => d.ClientId, opt => opt.MapFrom(src => src.ParentId))
                .ReverseMap();

            CreateMap<ClientPropertyViewModel, ClientProperty>()
                .ForMember(d => d.Client, opt => opt.Ignore())
                .ForMember(d => d.ClientId, opt => opt.MapFrom(src => src.ParentId))
                .ReverseMap();

            CreateMap<ClientSecretViewModel, ClientSecret>()
                .ForMember(d => d.Client, opt => opt.Ignore())
                .ForMember(d => d.ClientId, opt => opt.MapFrom(src => src.ParentId))
                .ReverseMap();

            CreateMap<ClientScopeViewModel, ClientScope>()
                .ForMember(d => d.Client, opt => opt.Ignore())
                .ForMember(d => d.ClientId, opt => opt.MapFrom(src => src.ParentId))
                .ReverseMap();

            CreateMap<ClientRedirectViewModel, ClientRedirectUri>()
                .ForMember(d => d.Client, opt => opt.Ignore())
                .ForMember(d => d.ClientId, opt => opt.MapFrom(src => src.ParentId))
                .ReverseMap();
        }

        private void PopulateApiResourceMappings()
        {
            CreateMap<ApiViewModel, ApiResource>()
                .ForMember(d => d.UserClaims, opt => opt.Ignore())
                .ForMember(d => d.Properties, opt => opt.Ignore())
                .ForMember(d => d.Scopes, opt => opt.Ignore())
                .ForMember(d => d.Secrets, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(d => d.Id, opt => opt.Ignore());

            CreateMap<ApiScopeViewModel, ApiScope>()
                .ForMember(d => d.ApiResource, opt => opt.Ignore())
                .ForMember(d => d.ApiResourceId, opt => opt.MapFrom(src => src.ParentId))
                .ReverseMap();

            CreateMap<ApiScopeClaimViewModel, ApiScopeClaim>()
                .ForMember(d => d.ApiScope, opt => opt.Ignore())
                .ForMember(d => d.ApiScopeId, opt => opt.MapFrom(src => src.ParentId))
                .ReverseMap();

            CreateMap<ApiPropertyViewModel, ApiResourceProperty>()
                .ForMember(d => d.ApiResource, opt => opt.Ignore())
                .ForMember(d => d.ApiResourceId, opt => opt.MapFrom(src => src.ParentId))
                .ReverseMap();

            CreateMap<ApiClaimViewModel, ApiResourceClaim>()
                .ForMember(d => d.ApiResource, opt => opt.Ignore())
                .ForMember(d => d.ApiResourceId, opt => opt.MapFrom(src => src.ParentId))
                .ReverseMap();

            CreateMap<ApiSecretViewModel, ApiSecret>()
                .ForMember(d => d.ApiResource, opt => opt.Ignore())
                .ForMember(d => d.ApiResourceId, opt => opt.MapFrom(src => src.ParentId))
                .ReverseMap();
        }

        private void PopulateIdentityResourceMappings()
        {
            CreateMap<IdentityResourceViewModel, IdentityResource>()
                .ForMember(d => d.Properties, opt => opt.Ignore())
                .ForMember(d => d.UserClaims, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(d => d.NavBar, opt => opt.Ignore());

            CreateMap<IdentityResources.Address, IdentityResourceViewModel>()
                .ForMember(d => d.NavBar, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<IdentityResources.Email, IdentityResourceViewModel>()
                .ForMember(d => d.NavBar, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<IdentityResources.Profile, IdentityResourceViewModel>()
                .ForMember(d => d.NavBar, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<IdentityResources.Phone, IdentityResourceViewModel>()
                .ForMember(d => d.NavBar, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<IdentityResources.OpenId, IdentityResourceViewModel>()
                .ForMember(d => d.NavBar, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<IdentityResourceClaimViewModel, IdentityClaim>()
                .ForMember(d => d.IdentityResource, opt => opt.Ignore())
                .ForMember(d => d.IdentityResourceId, opt => opt.MapFrom(src => src.ParentId))
                .ReverseMap();

            CreateMap<IdentityResourcePropertyViewModel, IdentityResourceProperty>()
                .ForMember(d => d.IdentityResource, opt => opt.Ignore())
                .ForMember(d => d.IdentityResourceId, opt => opt.MapFrom(src => src.ParentId))
                .ReverseMap();
        }
    }
}
