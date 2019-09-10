using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using spydersoft.Identity.Models;
using spydersoft.Identity.Models.Admin;
using spydersoft.Identity.Models.Admin.ApiViewModels;
using IS4Entities = IdentityServer4.EntityFramework.Entities;

namespace spydersoft.Identity.Controllers.Admin.Api
{
    public abstract class BaseApiCollectionController<TSingleViewModel, TCollectionViewModel, TChildEntity>
        : BaseAdminCollectionController<TSingleViewModel, TCollectionViewModel, ApiViewModel, IS4Entities.ApiResource, TChildEntity>
        where TSingleViewModel : BaseAdminChildItemViewModel, new()
        where TCollectionViewModel : BaseApiCollectionViewModel<TSingleViewModel>, new()
    {
        #region Constructor

        protected BaseApiCollectionController(ConfigurationDbContext context, MapperConfiguration mapperConfig) : base(context, mapperConfig)
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
            var query = ConfigDbContext.ApiResources;
            var includeQuery = AddIncludes(query);

            return includeQuery.FirstOrDefault(c => c.Id == id);
        }

        #endregion BaseAdminCollectionController Implementation
    }
}
