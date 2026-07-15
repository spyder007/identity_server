using System;
using System.Linq;
using System.Threading.Tasks;

using Asp.Versioning;

using AutoMapper;

using Duende.IdentityServer.EntityFramework.DbContexts;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Spydersoft.Identity.Admin.Api.Models.Clients;

using Client = Duende.IdentityServer.EntityFramework.Entities.Client;

namespace Spydersoft.Identity.Admin.Api.Controllers.V1
{
    /// <summary>REST API controller for managing OAuth2/OIDC clients.</summary>
    [ApiVersion("1.0")]
    public class ClientsController(ConfigurationDbContext dbContext, IMapper mapper) : BaseAdminApiController(mapper)
    {
        /// <summary>Returns a list of all registered clients.</summary>
        [HttpGet]
        [ProducesResponseType<ClientSummaryDto[]>(StatusCodes.Status200OK)]
        public IActionResult GetAll()
        {
            var clients = Mapper.ProjectTo<ClientSummaryDto>(dbContext.Clients.AsQueryable());
            return Ok(clients);
        }

        /// <summary>Returns the full detail of a single client.</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType<ClientDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(int id)
        {
            var client = dbContext.Clients.FirstOrDefault(c => c.Id == id);
            if (client is null)
                return NotFound();

            return Ok(Mapper.Map<ClientDto>(client));
        }

        /// <summary>Returns the full detail of a single client, looked up by its natural <c>ClientId</c> key.</summary>
        [HttpGet("by-client-id/{clientId}")]
        [ProducesResponseType<ClientDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetByClientId(string clientId)
        {
            var client = dbContext.Clients.FirstOrDefault(c => c.ClientId == clientId);
            if (client is null)
                return NotFound();

            return Ok(Mapper.Map<ClientDto>(client));
        }

        /// <summary>Creates a new client.</summary>
        [HttpPost]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType<ClientDto>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] SaveClientDto dto)
        {
            var entity = Mapper.Map<Client>(dto);
            entity.Created = DateTime.UtcNow;

            _ = dbContext.Clients.Add(entity);
            _ = await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, Mapper.Map<ClientDto>(entity));
        }

        /// <summary>Updates an existing client.</summary>
        [HttpPut("{id:int}")]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] SaveClientDto dto)
        {
            var entity = await dbContext.Clients.FirstOrDefaultAsync(c => c.Id == id);
            if (entity is null)
                return NotFound();

            _ = Mapper.Map(dto, entity);
            entity.Updated = DateTime.UtcNow;

            _ = dbContext.Clients.Update(entity);
            _ = await dbContext.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>Deletes a client.</summary>
        [HttpDelete("{id:int}")]
        [Authorize(Policy = AdminApiPolicies.Write)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await dbContext.Clients.FirstOrDefaultAsync(c => c.Id == id);
            if (entity is null)
                return NotFound();

            _ = dbContext.Clients.Remove(entity);
            _ = await dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
