using AutoMapper.QueryableExtensions;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using one.Identity.Models.ClientViewModels;
using System.Collections.Generic;
using System.Linq;

namespace one.Identity.Controllers.Admin.Client
{
    public class ClientScopesController : BaseClientCollectionController<ClientScopeViewModel, ClientScopesViewModel, ClientScope>
    {
        #region Constructor

        public ClientScopesController(ConfigurationDbContext dbContext) : base(dbContext)
        {
        }

        #endregion Constructor

        #region BaseClientCollectionController Implementation

        protected override IEnumerable<ClientScopeViewModel> PopulateItemList(IdentityServer4.EntityFramework.Entities.Client mainEntity)
        {
            return mainEntity.AllowedScopes.AsQueryable().ProjectTo<ClientScopeViewModel>();
        }

        protected override Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<IdentityServer4.EntityFramework.Entities.Client, List<ClientScope>> AddIncludes(DbSet<IdentityServer4.EntityFramework.Entities.Client> query)
        {
            return query.Include(c => c.AllowedScopes);
        }

        protected override ClientScope FindItemInCollection(List<ClientScope> collection, int id)
        {
            return collection.FirstOrDefault(s => s.Id == id);
        }

        protected override List<ClientScope> GetCollection(IdentityServer4.EntityFramework.Entities.Client mainEntity)
        {
            return mainEntity.AllowedScopes;
        }

        #endregion BaseClientCollectionController Implementation
    }
}