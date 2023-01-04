using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Duende.IdentityServer.EntityFramework.DbContexts;

using Microsoft.EntityFrameworkCore;

using spydersoft.Identity.Models.Admin;
using spydersoft.Identity.Models.Admin.IdentityResourceViewModels;

using IS4Entities = Duende.IdentityServer.EntityFramework.Entities;

namespace spydersoft.Identity.Controllers.Admin.Identity
{
    public abstract class BaseIdentityResourceCollectionController<TSingleViewModel, TCollectionViewModel, TChildEntity>
        : BaseAdminCollectionController<TSingleViewModel, TCollectionViewModel, IdentityResourceViewModel, IS4Entities.IdentityResource, TChildEntity>
        where TSingleViewModel : BaseAdminChildItemViewModel, new()
        where TCollectionViewModel : BaseIdentityResourceCollectionViewModel<TSingleViewModel>, new()
    {
        #region Constructor

        protected BaseIdentityResourceCollectionController(ConfigurationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        #endregion Constructor

        #region BaseIdentityResourceCollectionController Interface

        protected abstract override IEnumerable<TSingleViewModel> PopulateItemList(IS4Entities.IdentityResource mainEntity);

        protected abstract override IQueryable<IS4Entities.IdentityResource> AddIncludes(
            DbSet<IS4Entities.IdentityResource> query);

        #endregion BaseIdentityResourceCollectionController Interface

        #region BaseAdminCollectionController Implementation

        protected override IS4Entities.IdentityResource GetMainEntity(int id)
        {
            DbSet<IS4Entities.IdentityResource> query = ConfigDbContext.IdentityResources;
            IQueryable<IS4Entities.IdentityResource> includeQuery = AddIncludes(query);

            return includeQuery.FirstOrDefault(ir => ir.Id == id);
        }

        #endregion BaseAdminCollectionController Implementation
    }
}