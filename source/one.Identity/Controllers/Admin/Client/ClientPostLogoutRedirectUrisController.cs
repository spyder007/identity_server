using AutoMapper.QueryableExtensions;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Collections.Generic;
using System.Linq;
using one.Identity.Models.Admin.ClientViewModels;

namespace one.Identity.Controllers.Admin.Client
{
    public class ClientPostLogoutRedirectUrisController : BaseClientCollectionController<ClientPostLogoutRedirectUriViewModel, ClientPostLogoutRedirectUrisViewModel, ClientPostLogoutRedirectUri>
    {
        public ClientPostLogoutRedirectUrisController(ConfigurationDbContext context) : base(context)
        {
        }

        #region BaseClientCollectionController Implementation

        protected override IEnumerable<ClientPostLogoutRedirectUriViewModel> PopulateItemList(IdentityServer4.EntityFramework.Entities.Client mainEntity)
        {
            return mainEntity.PostLogoutRedirectUris.AsQueryable().ProjectTo<ClientPostLogoutRedirectUriViewModel>();
        }

        protected override IQueryable<IdentityServer4.EntityFramework.Entities.Client> AddIncludes(
            DbSet<IdentityServer4.EntityFramework.Entities.Client> query)
        {
            return query.Include(c => c.PostLogoutRedirectUris);
        }

        protected override ClientPostLogoutRedirectUri FindItemInCollection(List<ClientPostLogoutRedirectUri> collection, int id)
        {
            return collection.FirstOrDefault(p => p.Id == id);
        }

        protected override List<ClientPostLogoutRedirectUri> GetCollection(IdentityServer4.EntityFramework.Entities.Client mainEntity)
        {
            return mainEntity.PostLogoutRedirectUris;
        }

        #endregion BaseClientCollectionController Implementation
    }
}