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
    public class ClientPropertiesController : BaseClientCollectionController<ClientPropertyViewModel, ClientPropertiesViewModel, ClientProperty>
    {
        public ClientPropertiesController(ConfigurationDbContext context) : base(context)
        {
        }

        #region BaseClientCollectionController Implementation

        protected override IEnumerable<ClientPropertyViewModel> PopulateItemList(IdentityServer4.EntityFramework.Entities.Client mainEntity)
        {
            return mainEntity.Properties.AsQueryable().ProjectTo<ClientPropertyViewModel>();
        }

        protected override IIncludableQueryable<IdentityServer4.EntityFramework.Entities.Client, List<ClientProperty>> AddIncludes(DbSet<IdentityServer4.EntityFramework.Entities.Client> query)
        {
            return query.Include(c => c.Properties);
        }

        protected override ClientProperty FindItemInCollection(List<ClientProperty> collection, int id)
        {
            return collection.FirstOrDefault(p => p.Id == id);
        }

        protected override List<ClientProperty> GetCollection(IdentityServer4.EntityFramework.Entities.Client mainEntity)
        {
            return mainEntity.Properties;
        }

        #endregion BaseClientCollectionController Implementation
    }
}