using AutoMapper.QueryableExtensions;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using spydersoft.Identity.Models.Admin.ClientViewModels;

namespace spydersoft.Identity.Controllers.Admin.Client
{
    public class ClientClaimsController : BaseClientCollectionController<ClientClaimViewModel, ClientClaimsViewModel, ClientClaim>
    {
        public ClientClaimsController(ConfigurationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        #region BaseClientCollectionController Implementation

        protected override IEnumerable<ClientClaimViewModel> PopulateItemList(Duende.IdentityServer.EntityFramework.Entities.Client mainEntity)
        {
            return Mapper.ProjectTo<ClientClaimViewModel>(mainEntity.Claims.ToList().AsQueryable());
        }

        protected override IQueryable<Duende.IdentityServer.EntityFramework.Entities.Client> AddIncludes(
            DbSet<Duende.IdentityServer.EntityFramework.Entities.Client> query)
        {
            return query.Include(c => c.Claims);
        }

        protected override ClientClaim FindItemInCollection(List<ClientClaim> collection, int id)
        {
            return collection.FirstOrDefault(c => c.Id == id);
        }

        protected override List<ClientClaim> GetCollection(Duende.IdentityServer.EntityFramework.Entities.Client mainEntity)
        {
            return mainEntity.Claims;
        }

        #endregion BaseClientCollectionController Implementation
    }
}