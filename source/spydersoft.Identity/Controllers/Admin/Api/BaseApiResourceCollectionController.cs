using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Duende.IdentityServer.EntityFramework.DbContexts;

using Microsoft.EntityFrameworkCore;

using Spydersoft.Identity.Models.Admin;
using Spydersoft.Identity.Models.Admin.ApiResourceViewModels;

using IS4Entities = Duende.IdentityServer.EntityFramework.Entities;

namespace Spydersoft.Identity.Controllers.Admin.Api
{
    public abstract class BaseApiResourceCollectionController<TSingleViewModel, TCollectionViewModel, TChildEntity>
        : BaseAdminCollectionController<TSingleViewModel, TCollectionViewModel, ApiResourceViewModel, IS4Entities.ApiResource, TChildEntity>
        where TSingleViewModel : BaseAdminChildItemViewModel, new()
        where TCollectionViewModel : BaseApiResourceCollectionViewModel<TSingleViewModel>, new()
    {
        #region Constructor

        protected BaseApiResourceCollectionController(ConfigurationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        #endregion Constructor

        #region BaseClientCollectionController Interface

        protected abstract override IEnumerable<TSingleViewModel> PopulateItemList(IS4Entities.ApiResource mainEntity);

        protected abstract override IQueryable<IS4Entities.ApiResource> AddIncludes(
            DbSet<IS4Entities.ApiResource> query);

        #endregion BaseClientCollectionController Interface

        #region BaseAdminCollectionController Implementation

        protected override IS4Entities.ApiResource GetMainEntity(int id)
        {
            DbSet<IS4Entities.ApiResource> query = ConfigDbContext.ApiResources;
            IQueryable<IS4Entities.ApiResource> includeQuery = AddIncludes(query);

            return includeQuery.FirstOrDefault(c => c.Id == id);
        }

        #endregion BaseAdminCollectionController Implementation
    }
}