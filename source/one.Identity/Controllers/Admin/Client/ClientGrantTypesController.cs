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
    public class ClientGrantTypesController : BaseClientCollectionController<ClientGrantTypeViewModel, ClientGrantTypesViewModel, ClientGrantType>
    {
        public ClientGrantTypesController(ConfigurationDbContext context) : base(context)
        {
        }

        protected override IEnumerable<ClientGrantTypeViewModel> PopulateItemList(IdentityServer4.EntityFramework.Entities.Client mainEntity)
        {
            return mainEntity.AllowedGrantTypes.AsQueryable().ProjectTo<ClientGrantTypeViewModel>();
        }

        protected override IIncludableQueryable<IdentityServer4.EntityFramework.Entities.Client, List<ClientGrantType>> AddIncludes(DbSet<IdentityServer4.EntityFramework.Entities.Client> query)
        {
            return query.Include(c => c.AllowedGrantTypes);
        }

        protected override void RemoveObject(IdentityServer4.EntityFramework.Entities.Client mainEntity, int id)
        {
            var grantToRemove = mainEntity.AllowedGrantTypes.FirstOrDefault(g => g.Id == id);
            mainEntity.AllowedGrantTypes.Remove(grantToRemove);
        }

        protected override void AddObject(IdentityServer4.EntityFramework.Entities.Client mainEntity, int parentId, ClientGrantTypeViewModel newItem)
        {
            mainEntity.AllowedGrantTypes.Add(new ClientGrantType()
            {
                ClientId = parentId,
                GrantType = newItem.GrantType
            });
        }
    }
}
