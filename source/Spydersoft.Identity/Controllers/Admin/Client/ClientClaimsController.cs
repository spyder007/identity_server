using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;

using Microsoft.EntityFrameworkCore;

using Spydersoft.Identity.Models.Admin.ClientViewModels;

namespace Spydersoft.Identity.Controllers.Admin.Client
{
    public class ClientClaimsController(ConfigurationDbContext context, IMapper mapper) : BaseClientCollectionController<ClientClaimViewModel, ClientClaimsViewModel, ClientClaim>(context, mapper)
    {

        #region BaseClientCollectionController Implementation

        protected override IEnumerable<ClientClaimViewModel> PopulateItemList(Duende.IdentityServer.EntityFramework.Entities.Client mainEntity)
        {
            return Mapper.ProjectTo<ClientClaimViewModel>(mainEntity.Claims.AsQueryable());
        }

        protected override IQueryable<Duende.IdentityServer.EntityFramework.Entities.Client> AddIncludes(
            DbSet<Duende.IdentityServer.EntityFramework.Entities.Client> query)
        {
            return query.Include(c => c.Claims);
        }

        protected override ClientClaim FindItemInCollection(List<ClientClaim> collection, int id)
        {
            return collection.Find(c => c.Id == id);
        }

        protected override List<ClientClaim> GetCollection(Duende.IdentityServer.EntityFramework.Entities.Client mainEntity)
        {
            return mainEntity.Claims;
        }

        #endregion BaseClientCollectionController Implementation
    }
}