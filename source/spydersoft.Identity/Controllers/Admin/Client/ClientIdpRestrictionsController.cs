using AutoMapper.QueryableExtensions;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using spydersoft.Identity.Models.Admin.ClientViewModels;

namespace spydersoft.Identity.Controllers.Admin.Client
{
    public class ClientIdpRestrictionsController : BaseClientCollectionController<ClientIdpRestrictionViewModel, ClientIdpRestrictionsViewModel, ClientIdPRestriction>
    {
        public ClientIdpRestrictionsController(ConfigurationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        #region BaseClientCollectionController Implementation

        protected override IEnumerable<ClientIdpRestrictionViewModel> PopulateItemList(IdentityServer4.EntityFramework.Entities.Client mainEntity)
        {
            return Mapper.ProjectTo<ClientIdpRestrictionViewModel>(mainEntity.IdentityProviderRestrictions.ToList().AsQueryable());
        }

        protected override IQueryable<IdentityServer4.EntityFramework.Entities.Client> AddIncludes(
            DbSet<IdentityServer4.EntityFramework.Entities.Client> query)
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