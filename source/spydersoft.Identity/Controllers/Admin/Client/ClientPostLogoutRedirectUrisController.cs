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
    public class ClientPostLogoutRedirectUrisController : BaseClientCollectionController<ClientPostLogoutRedirectUriViewModel, ClientPostLogoutRedirectUrisViewModel, ClientPostLogoutRedirectUri>
    {
        public ClientPostLogoutRedirectUrisController(ConfigurationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        #region BaseClientCollectionController Implementation

        protected override IEnumerable<ClientPostLogoutRedirectUriViewModel> PopulateItemList(Duende.IdentityServer.EntityFramework.Entities.Client mainEntity)
        {
            return Mapper.ProjectTo<ClientPostLogoutRedirectUriViewModel>(mainEntity.PostLogoutRedirectUris.ToList().AsQueryable());
        }

        protected override IQueryable<Duende.IdentityServer.EntityFramework.Entities.Client> AddIncludes(
            DbSet<Duende.IdentityServer.EntityFramework.Entities.Client> query)
        {
            return query.Include(c => c.PostLogoutRedirectUris);
        }

        protected override ClientPostLogoutRedirectUri FindItemInCollection(List<ClientPostLogoutRedirectUri> collection, int id)
        {
            return collection.FirstOrDefault(p => p.Id == id);
        }

        protected override List<ClientPostLogoutRedirectUri> GetCollection(Duende.IdentityServer.EntityFramework.Entities.Client mainEntity)
        {
            return mainEntity.PostLogoutRedirectUris;
        }

        #endregion BaseClientCollectionController Implementation
    }
}