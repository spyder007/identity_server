using AutoMapper;

using Duende.IdentityServer.EntityFramework.Entities;

using Spydersoft.Identity.Admin.Api.Models.ApiResources;
using Spydersoft.Identity.Admin.Api.Models.Clients;
using Spydersoft.Identity.Admin.Api.Models.IdentityResources;
using Spydersoft.Identity.Admin.Api.Models.Roles;
using Spydersoft.Identity.Admin.Api.Models.Scopes;
using Spydersoft.Identity.Admin.Api.Models.Users;
using Spydersoft.Identity.Core.Models.Identity;

using ApiResource = Duende.IdentityServer.EntityFramework.Entities.ApiResource;
using ApiScope = Duende.IdentityServer.EntityFramework.Entities.ApiScope;
using Client = Duende.IdentityServer.EntityFramework.Entities.Client;
using IdentityResource = Duende.IdentityServer.EntityFramework.Entities.IdentityResource;

namespace Spydersoft.Identity.Admin.Api.Data
{
    /// <summary>
    /// AutoMapper profile mapping EF entities to Admin API DTOs.
    /// </summary>
    public class ApiAutoMapperProfile : Profile
    {
        /// <summary>Initializes a new instance of the <see cref="ApiAutoMapperProfile"/> class.</summary>
        public ApiAutoMapperProfile()
        {
            CreateClientMappings();
            CreateApiResourceMappings();
            CreateIdentityResourceMappings();
            CreateScopeMappings();
            CreateUserMappings();
            CreateRoleMappings();
        }

        private void CreateClientMappings()
        {
            _ = CreateMap<Client, ClientSummaryDto>();
            _ = CreateMap<Client, ClientDto>();

            _ = CreateMap<SaveClientDto, Client>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.AllowedCorsOrigins, opt => opt.Ignore())
                .ForMember(d => d.AllowedGrantTypes, opt => opt.Ignore())
                .ForMember(d => d.AllowedScopes, opt => opt.Ignore())
                .ForMember(d => d.Claims, opt => opt.Ignore())
                .ForMember(d => d.ClientSecrets, opt => opt.Ignore())
                .ForMember(d => d.IdentityProviderRestrictions, opt => opt.Ignore())
                .ForMember(d => d.PostLogoutRedirectUris, opt => opt.Ignore())
                .ForMember(d => d.Properties, opt => opt.Ignore())
                .ForMember(d => d.RedirectUris, opt => opt.Ignore())
                .ForMember(d => d.Created, opt => opt.Ignore())
                .ForMember(d => d.Updated, opt => opt.Ignore())
                .ForMember(d => d.LastAccessed, opt => opt.Ignore());

            _ = CreateMap<ClientClaim, ClientClaimDto>()
                .ForMember(d => d.ClientId, opt => opt.MapFrom(src => src.ClientId));

            _ = CreateMap<SaveClientClaimDto, ClientClaim>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.Client, opt => opt.Ignore())
                .ForMember(d => d.ClientId, opt => opt.Ignore());

            _ = CreateMap<ClientCorsOrigin, ClientCorsOriginDto>()
                .ForMember(d => d.ClientId, opt => opt.MapFrom(src => src.ClientId));

            _ = CreateMap<SaveClientCorsOriginDto, ClientCorsOrigin>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.Client, opt => opt.Ignore())
                .ForMember(d => d.ClientId, opt => opt.Ignore());

            _ = CreateMap<ClientGrantType, ClientGrantTypeDto>()
                .ForMember(d => d.ClientId, opt => opt.MapFrom(src => src.ClientId));

            _ = CreateMap<SaveClientGrantTypeDto, ClientGrantType>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.Client, opt => opt.Ignore())
                .ForMember(d => d.ClientId, opt => opt.Ignore());

            _ = CreateMap<ClientIdPRestriction, ClientIdpRestrictionDto>()
                .ForMember(d => d.ClientId, opt => opt.MapFrom(src => src.ClientId));

            _ = CreateMap<SaveClientIdpRestrictionDto, ClientIdPRestriction>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.Client, opt => opt.Ignore())
                .ForMember(d => d.ClientId, opt => opt.Ignore());

            _ = CreateMap<ClientPostLogoutRedirectUri, ClientPostLogoutRedirectUriDto>()
                .ForMember(d => d.ClientId, opt => opt.MapFrom(src => src.ClientId));

            _ = CreateMap<SaveClientPostLogoutRedirectUriDto, ClientPostLogoutRedirectUri>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.Client, opt => opt.Ignore())
                .ForMember(d => d.ClientId, opt => opt.Ignore());

            _ = CreateMap<ClientProperty, ClientPropertyDto>()
                .ForMember(d => d.ClientId, opt => opt.MapFrom(src => src.ClientId));

            _ = CreateMap<SaveClientPropertyDto, ClientProperty>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.Client, opt => opt.Ignore())
                .ForMember(d => d.ClientId, opt => opt.Ignore());

            _ = CreateMap<ClientRedirectUri, ClientRedirectUriDto>()
                .ForMember(d => d.ClientId, opt => opt.MapFrom(src => src.ClientId));

            _ = CreateMap<SaveClientRedirectUriDto, ClientRedirectUri>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.Client, opt => opt.Ignore())
                .ForMember(d => d.ClientId, opt => opt.Ignore());

            _ = CreateMap<ClientScope, ClientScopeDto>()
                .ForMember(d => d.ClientId, opt => opt.MapFrom(src => src.ClientId));

            _ = CreateMap<SaveClientScopeDto, ClientScope>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.Client, opt => opt.Ignore())
                .ForMember(d => d.ClientId, opt => opt.Ignore());

            _ = CreateMap<ClientSecret, ClientSecretDto>()
                .ForMember(d => d.ClientId, opt => opt.MapFrom(src => src.ClientId))
                .ForMember(d => d.Expiration, opt => opt.MapFrom(src => src.Expiration.HasValue ? src.Expiration.Value.ToString("O") : null));

            _ = CreateMap<SaveClientSecretDto, ClientSecret>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.Client, opt => opt.Ignore())
                .ForMember(d => d.ClientId, opt => opt.Ignore())
                .ForMember(d => d.Created, opt => opt.Ignore())
                .ForMember(d => d.Expiration, opt => opt.Ignore());
        }

        private void CreateApiResourceMappings()
        {
            _ = CreateMap<ApiResource, ApiResourceSummaryDto>();
            _ = CreateMap<ApiResource, ApiResourceDto>();

            _ = CreateMap<SaveApiResourceDto, ApiResource>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.UserClaims, opt => opt.Ignore())
                .ForMember(d => d.Properties, opt => opt.Ignore())
                .ForMember(d => d.Scopes, opt => opt.Ignore())
                .ForMember(d => d.Secrets, opt => opt.Ignore())
                .ForMember(d => d.Created, opt => opt.Ignore())
                .ForMember(d => d.Updated, opt => opt.Ignore())
                .ForMember(d => d.LastAccessed, opt => opt.Ignore());

            _ = CreateMap<ApiResourceClaim, ApiResourceClaimDto>()
                .ForMember(d => d.ApiResourceId, opt => opt.MapFrom(src => src.ApiResourceId));

            _ = CreateMap<SaveApiResourceClaimDto, ApiResourceClaim>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.ApiResource, opt => opt.Ignore())
                .ForMember(d => d.ApiResourceId, opt => opt.Ignore());

            _ = CreateMap<ApiResourceProperty, ApiResourcePropertyDto>()
                .ForMember(d => d.ApiResourceId, opt => opt.MapFrom(src => src.ApiResourceId));

            _ = CreateMap<SaveApiResourcePropertyDto, ApiResourceProperty>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.ApiResource, opt => opt.Ignore())
                .ForMember(d => d.ApiResourceId, opt => opt.Ignore());

            _ = CreateMap<ApiResourceScope, ApiResourceScopeDto>()
                .ForMember(d => d.ApiResourceId, opt => opt.MapFrom(src => src.ApiResourceId));

            _ = CreateMap<SaveApiResourceScopeDto, ApiResourceScope>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.ApiResource, opt => opt.Ignore())
                .ForMember(d => d.ApiResourceId, opt => opt.Ignore());

            _ = CreateMap<ApiResourceSecret, ApiResourceSecretDto>()
                .ForMember(d => d.ApiResourceId, opt => opt.MapFrom(src => src.ApiResourceId))
                .ForMember(d => d.Expiration, opt => opt.MapFrom(src => src.Expiration.HasValue ? src.Expiration.Value.ToString("O") : null));

            _ = CreateMap<SaveApiResourceSecretDto, ApiResourceSecret>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.ApiResource, opt => opt.Ignore())
                .ForMember(d => d.ApiResourceId, opt => opt.Ignore())
                .ForMember(d => d.Created, opt => opt.Ignore())
                .ForMember(d => d.Expiration, opt => opt.Ignore());
        }

        private void CreateIdentityResourceMappings()
        {
            _ = CreateMap<IdentityResource, IdentityResourceSummaryDto>();
            _ = CreateMap<IdentityResource, IdentityResourceDto>();

            _ = CreateMap<SaveIdentityResourceDto, IdentityResource>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.Properties, opt => opt.Ignore())
                .ForMember(d => d.UserClaims, opt => opt.Ignore())
                .ForMember(d => d.Created, opt => opt.Ignore())
                .ForMember(d => d.Updated, opt => opt.Ignore());

            _ = CreateMap<IdentityResourceClaim, IdentityResourceClaimDto>()
                .ForMember(d => d.IdentityResourceId, opt => opt.MapFrom(src => src.IdentityResourceId));

            _ = CreateMap<SaveIdentityResourceClaimDto, IdentityResourceClaim>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.IdentityResource, opt => opt.Ignore())
                .ForMember(d => d.IdentityResourceId, opt => opt.Ignore());

            _ = CreateMap<IdentityResourceProperty, IdentityResourcePropertyDto>()
                .ForMember(d => d.IdentityResourceId, opt => opt.MapFrom(src => src.IdentityResourceId));

            _ = CreateMap<SaveIdentityResourcePropertyDto, IdentityResourceProperty>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.IdentityResource, opt => opt.Ignore())
                .ForMember(d => d.IdentityResourceId, opt => opt.Ignore());
        }

        private void CreateScopeMappings()
        {
            _ = CreateMap<ApiScope, ScopeSummaryDto>();
            _ = CreateMap<ApiScope, ScopeDto>();

            _ = CreateMap<SaveScopeDto, ApiScope>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.UserClaims, opt => opt.Ignore())
                .ForMember(d => d.Properties, opt => opt.Ignore());

            _ = CreateMap<ApiScopeClaim, ScopeClaimDto>()
                .ForMember(d => d.ScopeId, opt => opt.MapFrom(src => src.ScopeId));

            _ = CreateMap<SaveScopeClaimDto, ApiScopeClaim>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.Scope, opt => opt.Ignore())
                .ForMember(d => d.ScopeId, opt => opt.Ignore());

            _ = CreateMap<ApiScopeProperty, ScopePropertyDto>()
                .ForMember(d => d.ScopeId, opt => opt.MapFrom(src => src.ScopeId));

            _ = CreateMap<SaveScopePropertyDto, ApiScopeProperty>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.Scope, opt => opt.Ignore())
                .ForMember(d => d.ScopeId, opt => opt.Ignore());
        }

        private void CreateUserMappings()
        {
            _ = CreateMap<ApplicationUser, UserSummaryDto>();
            _ = CreateMap<ApplicationUser, UserDto>();

            _ = CreateMap<SaveUserDto, ApplicationUser>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.ConcurrencyStamp, opt => opt.Ignore())
                .ForMember(d => d.SecurityStamp, opt => opt.Ignore())
                .ForMember(d => d.PasswordHash, opt => opt.Ignore())
                .ForMember(d => d.NormalizedEmail, opt => opt.Ignore())
                .ForMember(d => d.NormalizedUserName, opt => opt.Ignore())
                .ForMember(d => d.EmailConfirmed, opt => opt.Ignore())
                .ForMember(d => d.PhoneNumberConfirmed, opt => opt.Ignore())
                .ForMember(d => d.AccessFailedCount, opt => opt.Ignore());
        }

        private void CreateRoleMappings()
        {
            _ = CreateMap<ApplicationRole, RoleSummaryDto>();
            _ = CreateMap<ApplicationRole, RoleDto>();
        }
    }
}
