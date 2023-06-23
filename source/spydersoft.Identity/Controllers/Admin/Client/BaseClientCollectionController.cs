using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Duende.IdentityServer.EntityFramework.DbContexts;

using Microsoft.EntityFrameworkCore;

using Spydersoft.Identity.Models.Admin;
using Spydersoft.Identity.Models.Admin.ClientViewModels;

using IS4Entities = Duende.IdentityServer.EntityFramework.Entities;

namespace Spydersoft.Identity.Controllers.Admin.Client
{
    public abstract class BaseClientCollectionController<TSingleViewModel, TCollectionViewModel, TChildEntity>
        : BaseAdminCollectionController<TSingleViewModel, TCollectionViewModel, ClientViewModel, IS4Entities.Client, TChildEntity>
        where TSingleViewModel : BaseAdminChildItemViewModel, new()
        where TCollectionViewModel : BaseClientCollectionViewModel<TSingleViewModel>, new()
    {
        #region Constructor

        protected BaseClientCollectionController(ConfigurationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        #endregion Constructor

        #region BaseClientCollectionController Interface

        protected abstract override IEnumerable<TSingleViewModel> PopulateItemList(IS4Entities.Client mainEntity);

        protected abstract override IQueryable<IS4Entities.Client> AddIncludes(DbSet<IS4Entities.Client> query);

        #endregion BaseClientCollectionController Interface

        #region BaseAdminCollectionController Implementation

        protected override IS4Entities.Client GetMainEntity(int id)
        {
            DbSet<IS4Entities.Client> query = ConfigDbContext.Clients;
            IQueryable<IS4Entities.Client> includeQuery = AddIncludes(query);

            return includeQuery.FirstOrDefault(c => c.Id == id);
        }

        #endregion BaseAdminCollectionController Implementation
    }
}