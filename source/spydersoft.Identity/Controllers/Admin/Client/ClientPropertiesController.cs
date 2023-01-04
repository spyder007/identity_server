using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;

using Microsoft.EntityFrameworkCore;

using spydersoft.Identity.Models.Admin.ClientViewModels;

namespace spydersoft.Identity.Controllers.Admin.Client
{
    public class ClientPropertiesController : BaseClientCollectionController<ClientPropertyViewModel, ClientPropertiesViewModel, ClientProperty>
    {
        public ClientPropertiesController(ConfigurationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        #region BaseClientCollectionController Implementation

        protected override IEnumerable<ClientPropertyViewModel> PopulateItemList(Duende.IdentityServer.EntityFramework.Entities.Client mainEntity)
        {
            return Mapper.ProjectTo<ClientPropertyViewModel>(mainEntity.Properties.AsQueryable());
        }

        protected override IQueryable<Duende.IdentityServer.EntityFramework.Entities.Client> AddIncludes(
            DbSet<Duende.IdentityServer.EntityFramework.Entities.Client> query)
        {
            return query.Include(c => c.Properties);
        }

        protected override ClientProperty FindItemInCollection(List<ClientProperty> collection, int id)
        {
            return collection.FirstOrDefault(p => p.Id == id);
        }

        protected override List<ClientProperty> GetCollection(Duende.IdentityServer.EntityFramework.Entities.Client mainEntity)
        {
            return mainEntity.Properties;
        }

        #endregion BaseClientCollectionController Implementation
    }
}