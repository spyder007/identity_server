using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;

using Microsoft.EntityFrameworkCore;

using Spydersoft.Identity.Models.Admin.ClientViewModels;

namespace Spydersoft.Identity.Controllers.Admin.Client
{
    public class ClientCorsOriginsController : BaseClientCollectionController<ClientCorsOriginViewModel, ClientCorsOriginsViewModel, ClientCorsOrigin>
    {
        public ClientCorsOriginsController(ConfigurationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        #region BaseClientCollectionController Implementation

        protected override IEnumerable<ClientCorsOriginViewModel> PopulateItemList(Duende.IdentityServer.EntityFramework.Entities.Client mainEntity)
        {
            return Mapper.ProjectTo<ClientCorsOriginViewModel>(mainEntity.AllowedCorsOrigins.AsQueryable());
        }

        protected override IQueryable<Duende.IdentityServer.EntityFramework.Entities.Client> AddIncludes(
            DbSet<Duende.IdentityServer.EntityFramework.Entities.Client> query)
        {
            return query.Include(c => c.AllowedCorsOrigins);
        }

        protected override ClientCorsOrigin FindItemInCollection(List<ClientCorsOrigin> collection, int id)
        {
            return collection.Find(cors => cors.Id == id);
        }

        protected override List<ClientCorsOrigin> GetCollection(Duende.IdentityServer.EntityFramework.Entities.Client mainEntity)
        {
            return mainEntity.AllowedCorsOrigins;
        }

        #endregion BaseClientCollectionController Implementation
    }
}