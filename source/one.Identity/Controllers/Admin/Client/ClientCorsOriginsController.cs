using AutoMapper.QueryableExtensions;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using one.Identity.Models.ClientViewModels;
using System.Collections.Generic;
using System.Linq;

namespace one.Identity.Controllers.Admin.Client
{
    public class ClientCorsOriginsController : BaseClientCollectionController<ClientCorsOriginViewModel, ClientCorsOriginsViewModel, ClientCorsOrigin>
    {
        public ClientCorsOriginsController(ConfigurationDbContext context) : base(context)
        {
        }

        #region BaseClientCollectionController Implementation

        protected override IEnumerable<ClientCorsOriginViewModel> PopulateItemList(IdentityServer4.EntityFramework.Entities.Client mainEntity)
        {
            return mainEntity.AllowedCorsOrigins.AsQueryable().ProjectTo<ClientCorsOriginViewModel>();
        }

        protected override IIncludableQueryable<IdentityServer4.EntityFramework.Entities.Client, List<ClientCorsOrigin>> AddIncludes(DbSet<IdentityServer4.EntityFramework.Entities.Client> query)
        {
            return query.Include(c => c.AllowedCorsOrigins);
        }

        protected override ClientCorsOrigin FindItemInCollection(List<ClientCorsOrigin> collection, int id)
        {
            return collection.FirstOrDefault(cors => cors.Id == id);
        }

        protected override List<ClientCorsOrigin> GetCollection(IdentityServer4.EntityFramework.Entities.Client mainEntity)
        {
            return mainEntity.AllowedCorsOrigins;
        }

        #endregion BaseClientCollectionController Implementation
    }
}