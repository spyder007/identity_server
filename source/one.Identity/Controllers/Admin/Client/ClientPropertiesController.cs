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

        protected override IEnumerable<ClientPropertyViewModel> PopulateItemList(IdentityServer4.EntityFramework.Entities.Client client)
        {
            return client.Properties.AsQueryable().ProjectTo<ClientPropertyViewModel>();
        }

        protected override IIncludableQueryable<IdentityServer4.EntityFramework.Entities.Client, List<ClientProperty>> AddIncludes(DbSet<IdentityServer4.EntityFramework.Entities.Client> query)
        {
            return query.Include(c => c.Properties);
        }

        protected override void RemoveObject(IdentityServer4.EntityFramework.Entities.Client client, int id)
        {
            var prop = client.Properties.FirstOrDefault(p => p.Id == id);
            client.Properties.Remove(prop);
        }

        protected override void AddObject(IdentityServer4.EntityFramework.Entities.Client client, int clientId, ClientPropertyViewModel newItem)
        {
            client.Properties.Add(new ClientProperty()
            {
                ClientId = clientId,
                Key = newItem.Key,
                Value = newItem.Value
            });
        }
    }
}
