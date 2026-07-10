using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Spydersoft.Identity.Admin.Api.Controllers.V1
{
    /// <summary>
    /// Base controller for all Admin API v1 controllers.
    /// Provides common route prefix, authorization, and AutoMapper access.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize(Policy = AdminApiPolicies.Read)]
    public abstract class BaseAdminApiController(IMapper mapper) : ControllerBase
    {
        /// <summary>Gets the AutoMapper instance.</summary>
        protected IMapper Mapper { get; } = mapper;
    }
}
