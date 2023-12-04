using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;

using Microsoft.EntityFrameworkCore;

using Spydersoft.Identity.Models.Admin.ClientViewModels;

namespace Spydersoft.Identity.Controllers.Admin.Client
{
    public class ClientIdpRestrictionsController : BaseClientCollectionController<ClientIdpRestrictionViewModel, ClientIdpRestrictionsViewModel, ClientIdPRestriction>
    {
        public ClientIdpRestrictionsController(ConfigurationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        #region BaseClientCollectionController Implementation

        protected override IEnumerable<ClientIdpRestrictionViewModel> PopulateItemList(Duende.IdentityServer.EntityFramework.Entities.Client mainEntity)
        {
            return Mapper.ProjectTo<ClientIdpRestrictionViewModel>(mainEntity.IdentityProviderRestrictions.AsQueryable());
        }

        protected override IQueryable<Duende.IdentityServer.EntityFramework.Entities.Client> AddIncludes(
            DbSet<Duende.IdentityServer.EntityFramework.Entities.Client> query)
        {
            return query.Include(c => c.IdentityProviderRestrictions);
        }

        protected override ClientIdPRestriction FindItemInCollection(List<ClientIdPRestriction> collection, int id)
        {
            return collection.Find(idp => idp.Id == id);
        }

        protected override List<ClientIdPRestriction> GetCollection(Duende.IdentityServer.EntityFramework.Entities.Client mainEntity)
        {
            return mainEntity.IdentityProviderRestrictions;
        }

        #endregion BaseClientCollectionController Implementation
    }
}