using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;

using Microsoft.EntityFrameworkCore;

using spydersoft.Identity.Models.Admin.ClientViewModels;

namespace spydersoft.Identity.Controllers.Admin.Client
{
    public class ClientGrantTypesController : BaseClientCollectionController<ClientGrantTypeViewModel, ClientGrantTypesViewModel, ClientGrantType>
    {
        public ClientGrantTypesController(ConfigurationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        #region BaseClientCollectionController Implementation

        protected override IEnumerable<ClientGrantTypeViewModel> PopulateItemList(Duende.IdentityServer.EntityFramework.Entities.Client mainEntity)
        {
            return Mapper.ProjectTo<ClientGrantTypeViewModel>(mainEntity.AllowedGrantTypes.AsQueryable());
        }

        protected override IQueryable<Duende.IdentityServer.EntityFramework.Entities.Client> AddIncludes(
            DbSet<Duende.IdentityServer.EntityFramework.Entities.Client> query)
        {
            return query.Include(c => c.AllowedGrantTypes);
        }

        protected override ClientGrantType FindItemInCollection(List<ClientGrantType> collection, int id)
        {
            return collection.FirstOrDefault(g => g.Id == id);
        }

        protected override List<ClientGrantType> GetCollection(Duende.IdentityServer.EntityFramework.Entities.Client mainEntity)
        {
            return mainEntity.AllowedGrantTypes;
        }

        #endregion BaseClientCollectionController Implementation
    }
}