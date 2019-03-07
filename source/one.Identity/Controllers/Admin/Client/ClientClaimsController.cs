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
    public class ClientClaimsController : BaseClientCollectionController<ClientClaimViewModel, ClientClaimsViewModel, ClientClaim>
    {
        public ClientClaimsController(ConfigurationDbContext context) : base(context)
        {
        }

        #region BaseClientCollectionController Implementation

        protected override IEnumerable<ClientClaimViewModel> PopulateItemList(IdentityServer4.EntityFramework.Entities.Client mainEntity)
        {
            return mainEntity.Claims.AsQueryable().ProjectTo<ClientClaimViewModel>();
        }

        protected override IIncludableQueryable<IdentityServer4.EntityFramework.Entities.Client, List<ClientClaim>> AddIncludes(DbSet<IdentityServer4.EntityFramework.Entities.Client> query)
        {
            return query.Include(c => c.Claims);
        }

        protected override ClientClaim FindItemInCollection(List<ClientClaim> collection, int id)
        {
            return collection.FirstOrDefault(c => c.Id == id);
        }

        protected override List<ClientClaim> GetCollection(IdentityServer4.EntityFramework.Entities.Client mainEntity)
        {
            return mainEntity.Claims;
        }

        #endregion BaseClientCollectionController Implementation
    }
}