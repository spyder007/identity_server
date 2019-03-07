using AutoMapper;
using IdentityServer4.EntityFramework.Entities;
using one.Identity.Models.Admin.ApiViewModels;
using one.Identity.Models.Admin.ClientViewModels;
using one.Identity.Models.Admin.IdentityResourceViewModels;

namespace one.Identity.Data
{
    public class AutoMapper : Profile
    {
        public static void Initialize()
        {
            Mapper.Initialize(cfg =>
            {
                PopulateClientMappings(cfg);
                PopulateIdentityResourceMappings(cfg);
                PopulateApiResourceMappings(cfg);
               
            });
        }

        private static void PopulateClientMappings(IMapperConfigurationExpression cfg)
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

            cfg.CreateMap<ClientClaimViewModel, ClientClaim>()
                .ForMember(d => d.Client, opt => opt.Ignore())
                .ForMember(d => d.ClientId, opt => opt.MapFrom(src => src.ParentId))
                .ReverseMap();

            cfg.CreateMap<ClientCorsOriginViewModel, ClientCorsOrigin>()
                .ForMember(d => d.Client, opt => opt.Ignore())
                .ForMember(d => d.ClientId, opt => opt.MapFrom(src => src.ParentId))
                .ReverseMap();

            cfg.CreateMap<ClientGrantTypeViewModel, ClientGrantType>()
                .ForMember(d => d.Client, opt => opt.Ignore())
                .ForMember(d => d.ClientId, opt => opt.MapFrom(src => src.ParentId))
                .ReverseMap();

            cfg.CreateMap<ClientIdpRestrictionViewModel, ClientIdPRestriction>()
                .ForMember(d => d.Client, opt => opt.Ignore())
                .ForMember(d => d.ClientId, opt => opt.MapFrom(src => src.ParentId))
                .ReverseMap();

            cfg.CreateMap<ClientPostLogoutRedirectUriViewModel, ClientPostLogoutRedirectUri>()
                .ForMember(d => d.Client, opt => opt.Ignore())
                .ForMember(d => d.ClientId, opt => opt.MapFrom(src => src.ParentId))
                .ReverseMap();

            cfg.CreateMap<ClientPropertyViewModel, ClientProperty>()
                .ForMember(d => d.Client, opt => opt.Ignore())
                .ForMember(d => d.ClientId, opt => opt.MapFrom(src => src.ParentId))
                .ReverseMap();

            cfg.CreateMap<ClientSecretViewModel, ClientSecret>()
                .ForMember(d => d.Client, opt => opt.Ignore())
                .ForMember(d => d.ClientId, opt => opt.MapFrom(src => src.ParentId))
                .ReverseMap();

            cfg.CreateMap<ClientScopeViewModel, ClientScope>()
                .ForMember(d => d.Client, opt => opt.Ignore())
                .ForMember(d => d.ClientId, opt => opt.MapFrom(src => src.ParentId))
                .ReverseMap();

            cfg.CreateMap<ClientRedirectViewModel, ClientRedirectUri>()
                .ForMember(d => d.Client, opt => opt.Ignore())
                .ForMember(d => d.ClientId, opt => opt.MapFrom(src => src.ParentId))
                .ReverseMap();
        }

        private static void PopulateApiResourceMappings(IMapperConfigurationExpression cfg)
        {
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
                .ForMember(d => d.ApiResourceId, opt => opt.MapFrom(src => src.ParentId))
                .ReverseMap();

            cfg.CreateMap<ApiPropertyViewModel, ApiResourceProperty>()
                .ForMember(d => d.ApiResource, opt => opt.Ignore())
                .ForMember(d => d.ApiResourceId, opt => opt.MapFrom(src => src.ParentId))
                .ReverseMap();

            cfg.CreateMap<ApiClaimViewModel, ApiResourceClaim>()
                .ForMember(d => d.ApiResource, opt => opt.Ignore())
                .ForMember(d => d.ApiResourceId, opt => opt.MapFrom(src => src.ParentId))
                .ReverseMap();

            cfg.CreateMap<ApiSecretViewModel, ApiSecret>()
                .ForMember(d => d.ApiResource, opt => opt.Ignore())
                .ForMember(d => d.ApiResourceId, opt => opt.MapFrom(src => src.ParentId))
                .ReverseMap();
        }

        private static void PopulateIdentityResourceMappings(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<IdentityResourceViewModel, IdentityResource>()
                .ForMember(d => d.Properties, opt => opt.Ignore())
                .ForMember(d => d.UserClaims, opt => opt.Ignore())
                .ReverseMap();

            cfg.CreateMap<IdentityResourceClaimViewModel, IdentityClaim>()
                .ForMember(d => d.IdentityResource, opt => opt.Ignore())
                .ForMember(d => d.IdentityResourceId, opt => opt.MapFrom(src => src.ParentId))
                .ReverseMap();

            cfg.CreateMap<IdentityResourcePropertyViewModel, IdentityResourceProperty>()
                .ForMember(d => d.IdentityResource, opt => opt.Ignore())
                .ForMember(d => d.IdentityResourceId, opt => opt.MapFrom(src => src.ParentId))
                .ReverseMap();
        }
    }
}
