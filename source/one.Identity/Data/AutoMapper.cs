using AutoMapper;
using IdentityServer4.EntityFramework.Entities;
using one.Identity.Models.ApiViewModels;
using one.Identity.Models.ClientViewModels;
using one.Identity.Models.IdentityResourceViewModels;

namespace one.Identity.Data
{
    public class AutoMapper : Profile
    {
        public static void Initialize()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<ClientViewModel, Client>()
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

                cfg.CreateMap<IdentityResourceViewModel, IdentityResource>()
                    .ForMember(d => d.Properties, opt => opt.Ignore())
                    .ForMember(d => d.UserClaims, opt => opt.Ignore())
                    .ReverseMap()
                    .ForMember(d => d.Id, opt => opt.Ignore());

                cfg.CreateMap<ApiViewModel, ApiResource>()
                    .ForMember(d => d.UserClaims, opt => opt.Ignore())
                    .ForMember(d => d.Properties, opt => opt.Ignore())
                    .ForMember(d => d.Scopes, opt => opt.Ignore())
                    .ForMember(d => d.Secrets, opt => opt.Ignore())
                    .ReverseMap()
                    .ForMember(d => d.Id, opt => opt.Ignore());

                cfg.CreateMap<ApiScopeViewModel, ApiScope>()
                    .ForMember(d => d.UserClaims, opt => opt.Ignore())
                    .ForMember(d => d.ApiResource, opt => opt.Ignore())
                    .ReverseMap();

                cfg.CreateMap<ApiPropertyViewModel, ApiResourceProperty>()
                    .ForMember(d => d.ApiResource, opt => opt.Ignore())
                    .ReverseMap();

                cfg.CreateMap<ApiClaimViewModel, ApiResourceClaim>()
                    .ForMember(d => d.ApiResource, opt => opt.Ignore())
                    .ReverseMap();

                cfg.CreateMap<ApiSecretViewModel, ApiSecret>()
                    .ForMember(d => d.ApiResource, opt => opt.Ignore())
                    .ReverseMap();
            });
        }
    }
}
