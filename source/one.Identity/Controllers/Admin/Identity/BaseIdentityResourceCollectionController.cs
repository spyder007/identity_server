using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using one.Identity.Models;
using one.Identity.Models.IdentityResourceViewModels;
using System.Collections.Generic;
using System.Linq;
using IS4Entities = IdentityServer4.EntityFramework.Entities;

namespace one.Identity.Controllers.Admin.Identity
{
    public abstract class BaseIdentityResourceCollectionController<TSingleViewModel, TCollectionViewModel, TChildEntity>
        : BaseAdminCollectionController<TSingleViewModel, TCollectionViewModel, IS4Entities.IdentityResource, TChildEntity>
        where TSingleViewModel : BaseAdminChildItemViewModel, new()
        where TCollectionViewModel : BaseIdentityResourceCollectionViewModel<TSingleViewModel>, new()
    {
        #region Constructor

        protected BaseIdentityResourceCollectionController(ConfigurationDbContext context) : base(context)
        {
        }

        #endregion Constructor

        #region BaseIdentityResourceCollectionController Interface

        protected abstract override IEnumerable<TSingleViewModel> PopulateItemList(IS4Entities.IdentityResource mainEntity);

        protected abstract override Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<IS4Entities.IdentityResource, List<TChildEntity>> AddIncludes(DbSet<IS4Entities.IdentityResource> query);

        #endregion BaseIdentityResourceCollectionController Interface

        #region BaseAdminCollectionController Implementation

        protected override IS4Entities.IdentityResource GetMainEntity(int id)
        {
            var query = ConfigDbContext.IdentityResources;
            var includeQuery = AddIncludes(query);

            return includeQuery.FirstOrDefault(ir => ir.Id == id);
        }

        #endregion BaseAdminCollectionController Implementation
    }
}