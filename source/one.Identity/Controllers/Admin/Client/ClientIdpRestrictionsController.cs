using AutoMapper.QueryableExtensions;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Collections.Generic;
using System.Linq;
using one.Identity.Models.Admin.ClientViewModels;

namespace one.Identity.Controllers.Admin.Client
{
    public class ClientIdpRestrictionsController : BaseClientCollectionController<ClientIdpRestrictionViewModel, ClientIdpRestrictionsViewModel, ClientIdPRestriction>
    {
        public ClientIdpRestrictionsController(ConfigurationDbContext context) : base(context)
        {
        }

        #region BaseClientCollectionController Implementation

        protected override IEnumerable<ClientIdpRestrictionViewModel> PopulateItemList(IdentityServer4.EntityFramework.Entities.Client mainEntity)
        {
            return mainEntity.IdentityProviderRestrictions.AsQueryable().ProjectTo<ClientIdpRestrictionViewModel>();
        }

        protected override IIncludableQueryable<IdentityServer4.EntityFramework.Entities.Client, List<ClientIdPRestriction>> AddIncludes(DbSet<IdentityServer4.EntityFramework.Entities.Client> query)
        {
            return query.Include(c => c.IdentityProviderRestrictions);
        }

        protected override ClientIdPRestriction FindItemInCollection(List<ClientIdPRestriction> collection, int id)
        {
            return collection.FirstOrDefault(idp => idp.Id == id);
        }

        protected override List<ClientIdPRestriction> GetCollection(IdentityServer4.EntityFramework.Entities.Client mainEntity)
        {
            return mainEntity.IdentityProviderRestrictions;
        }

        #endregion BaseClientCollectionController Implementation
    }
}