using AutoMapper.QueryableExtensions;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using one.Identity.Models.ClientViewModels;
using System.Collections.Generic;
using System.Linq;
using IS4Entities = IdentityServer4.EntityFramework.Entities;

namespace one.Identity.Controllers.Admin.Client
{
    public class ClientRedirectsController : BaseClientCollectionController<ClientRedirectViewModel, ClientRedirectsViewModel, IS4Entities.ClientRedirectUri>
    {
        public ClientRedirectsController(ConfigurationDbContext context) : base(context)
        {
        }

        #region BaseClientCollectinoController Implementation

        protected override IEnumerable<ClientRedirectViewModel> PopulateItemList(IS4Entities.Client mainEntity)
        {
            return mainEntity.RedirectUris.AsQueryable().ProjectTo<ClientRedirectViewModel>();
        }

        protected override IIncludableQueryable<IS4Entities.Client, List<IS4Entities.ClientRedirectUri>> AddIncludes(DbSet<IS4Entities.Client> query)
        {
            return query.Include(c => c.RedirectUris);
        }

        protected override IS4Entities.ClientRedirectUri FindItemInCollection(List<IS4Entities.ClientRedirectUri> collection, int id)
        {
            return collection.FirstOrDefault(r => r.Id == id);
        }

        protected override List<IS4Entities.ClientRedirectUri> GetCollection(IS4Entities.Client mainEntity)
        {
            return mainEntity.RedirectUris;
        }

        #endregion BaseClientCollectinoController Implementation
    }
}