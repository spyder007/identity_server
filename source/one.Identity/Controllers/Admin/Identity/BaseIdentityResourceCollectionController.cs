using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using one.Identity.Models.ClientViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using one.Identity.Models;
using one.Identity.Models.IdentityResourceViewModels;
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

        protected abstract override IEnumerable<TSingleViewModel> PopulateItemList(IS4Entities.IdentityResource identityResource);

        protected abstract override Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<IS4Entities.IdentityResource, List<TChildEntity>> AddIncludes(DbSet<IS4Entities.IdentityResource> query);

        protected abstract override void RemoveObject(IS4Entities.IdentityResource identityResource, int id);

        protected abstract override void AddObject(IS4Entities.IdentityResource identityResource, int identityResourceId,
            TSingleViewModel newItem);

        #endregion BaseIdentityResourceCollectionController Interface

        
        #region Private Methods

        protected override IS4Entities.IdentityResource GetMainEntity(int id)
        {
            var query = ConfigDbContext.IdentityResources;
            var includeQuery = AddIncludes(query);

            return includeQuery.FirstOrDefault(ir => ir.Id == id);
        }

        #endregion Private Methods
    }
}