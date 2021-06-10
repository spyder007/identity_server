using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using spydersoft.Identity.Models;
using spydersoft.Identity.Models.Admin;
using spydersoft.Identity.Models.Admin.ScopeViewModels;
using IS4Entities = Duende.IdentityServer.EntityFramework.Entities;

namespace spydersoft.Identity.Controllers.Admin.Scope
{
    public abstract class BaseScopeCollectionController<TSingleViewModel, TCollectionViewModel, TChildEntity>
        : BaseAdminCollectionController<TSingleViewModel, TCollectionViewModel, ScopeViewModel, IS4Entities.ApiScope, TChildEntity>
        where TSingleViewModel : BaseAdminChildItemViewModel, new()
        where TCollectionViewModel : BaseScopeCollectionViewModel<TSingleViewModel>, new()
    {
        #region Constructor

        protected BaseScopeCollectionController(ConfigurationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        #endregion Constructor

        #region BaseClientCollectionController Interface

        protected abstract override IEnumerable<TSingleViewModel> PopulateItemList(IS4Entities.ApiScope mainEntity);

        protected abstract override IQueryable<IS4Entities.ApiScope> AddIncludes(
            DbSet<IS4Entities.ApiScope> query);

        #endregion BaseClientCollectionController Interface

        #region BaseAdminCollectionController Implementation
        
        protected override IS4Entities.ApiScope GetMainEntity(int id)
        {
            var query = ConfigDbContext.ApiScopes;
            var includeQuery = AddIncludes(query);

            return includeQuery.FirstOrDefault(c => c.Id == id);
        }

        #endregion BaseAdminCollectionController Implementation
    }
}
