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
    public class ClientCorsOriginsController : BaseClientCollectionController<ClientCorsOriginViewModel, ClientCorsOriginsViewModel, ClientCorsOrigin>
    {
        public ClientCorsOriginsController(ConfigurationDbContext context) : base(context)
        {
        }

        protected override IEnumerable<ClientCorsOriginViewModel> PopulateItemList(IdentityServer4.EntityFramework.Entities.Client client)
        {
            return client.AllowedCorsOrigins.AsQueryable().ProjectTo<ClientCorsOriginViewModel>();
        }

        protected override IIncludableQueryable<IdentityServer4.EntityFramework.Entities.Client, List<ClientCorsOrigin>> AddIncludes(DbSet<IdentityServer4.EntityFramework.Entities.Client> query)
        {
            return query.Include(c => c.AllowedCorsOrigins);
        }

        protected override void RemoveObject(IdentityServer4.EntityFramework.Entities.Client client, int id)
        {
            var originToDelete = client.AllowedCorsOrigins.FirstOrDefault(o => o.Id == id);
            client.AllowedCorsOrigins.Remove(originToDelete);
        }

        protected override void AddObject(IdentityServer4.EntityFramework.Entities.Client client, int clientId, ClientCorsOriginViewModel newItem)
        {
            client.AllowedCorsOrigins.Add(new ClientCorsOrigin()
            {
                ClientId = clientId,
                Origin = newItem.Origin
            });
        }
    }
}
