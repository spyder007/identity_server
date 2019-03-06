using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using one.Identity.Models.ClientViewModels;

namespace one.Identity.Controllers.Admin.Client
{
    public class ClientPropertiesController : BaseClientCollectionController<ClientPropertyViewModel, ClientPropertiesViewModel, ClientProperty>
    {
        public ClientPropertiesController(ConfigurationDbContext context) : base(context)
        {
        }

        protected override IEnumerable<ClientPropertyViewModel> PopulateItemList(IdentityServer4.EntityFramework.Entities.Client mainEntity)
        {
            return mainEntity.Properties.AsQueryable().ProjectTo<ClientPropertyViewModel>();
        }

        protected override IIncludableQueryable<IdentityServer4.EntityFramework.Entities.Client, List<ClientProperty>> AddIncludes(DbSet<IdentityServer4.EntityFramework.Entities.Client> query)
        {
            return query.Include(c => c.Properties);
        }

        protected override void RemoveObject(IdentityServer4.EntityFramework.Entities.Client mainEntity, int id)
        {
            var prop = mainEntity.Properties.FirstOrDefault(p => p.Id == id);
            mainEntity.Properties.Remove(prop);
        }

        protected override void AddObject(IdentityServer4.EntityFramework.Entities.Client mainEntity, int parentId, ClientPropertyViewModel newItem)
        {
            mainEntity.Properties.Add(new ClientProperty()
            {
                ClientId = parentId,
                Key = newItem.Key,
                Value = newItem.Value
            });
        }
    }
}
