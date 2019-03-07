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
    public class ClientGrantTypesController : BaseClientCollectionController<ClientGrantTypeViewModel, ClientGrantTypesViewModel, ClientGrantType>
    {
        public ClientGrantTypesController(ConfigurationDbContext context) : base(context)
        {
        }

        #region BaseClientCollectionController Implementation

        protected override IEnumerable<ClientGrantTypeViewModel> PopulateItemList(IdentityServer4.EntityFramework.Entities.Client mainEntity)
        {
            return mainEntity.AllowedGrantTypes.AsQueryable().ProjectTo<ClientGrantTypeViewModel>();
        }

        protected override IIncludableQueryable<IdentityServer4.EntityFramework.Entities.Client, List<ClientGrantType>> AddIncludes(DbSet<IdentityServer4.EntityFramework.Entities.Client> query)
        {
            return query.Include(c => c.AllowedGrantTypes);
        }

        protected override ClientGrantType FindItemInCollection(List<ClientGrantType> collection, int id)
        {
            return collection.FirstOrDefault(g => g.Id == id);
        }

        protected override List<ClientGrantType> GetCollection(IdentityServer4.EntityFramework.Entities.Client mainEntity)
        {
            return mainEntity.AllowedGrantTypes;
        }

        #endregion BaseClientCollectionController Implementation
    }
}