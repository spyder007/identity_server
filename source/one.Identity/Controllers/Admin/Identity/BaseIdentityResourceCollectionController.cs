using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using one.Identity.Models.ClientViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using one.Identity.Models.IdentityResourceViewModels;
using IS4Entities = IdentityServer4.EntityFramework.Entities;

namespace one.Identity.Controllers.Admin.Identity
{
    public abstract class BaseIdentityResourceCollectionController<TSingleViewModel, TCollectionViewModel, TChildEntity> : BaseAdminController
        where TSingleViewModel : BaseIdentityResourceChildItemViewModel, new()
        where TCollectionViewModel : BaseIdentityResourceCollectionViewModel<TSingleViewModel>, new()
    {
        #region Constructor

        protected BaseIdentityResourceCollectionController(ConfigurationDbContext context) : base(context)
        {
        }

        #endregion Constructor

        #region BaseIdentityResourceCollectionController Interface

        protected abstract IEnumerable<TSingleViewModel> PopulateItemList(IS4Entities.IdentityResource identityResource);

        protected abstract Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<IS4Entities.IdentityResource, List<TChildEntity>> AddIncludes(DbSet<IS4Entities.IdentityResource> query);

        protected abstract void RemoveObject(IS4Entities.IdentityResource identityResource, int id);

        protected abstract void AddObject(IS4Entities.IdentityResource identityResource, int identityResourceId,
            TSingleViewModel newItem);

        protected virtual IActionResult GetView(TCollectionViewModel model)
        {
            return View(nameof(Edit), model);
        }

        protected virtual IActionResult GetEditRedirect(object routeValues)
        {
            return RedirectToAction(nameof(Edit), routeValues);
        }

        #endregion BaseIdentityResourceCollectionController Interface

        #region Controller Actions

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            var collectionViewModel = new TCollectionViewModel();
            if (id.HasValue)
            {
                collectionViewModel.SetId(id.Value);

                IS4Entities.IdentityResource identityResource = GetIdentityResource(id.Value);
                if (identityResource == null)
                {
                    return GetErrorAction("Could not load identity resource");
                }

                collectionViewModel.ItemsList.AddRange(PopulateItemList(identityResource));
            }

            return View(collectionViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Save(int? id, TCollectionViewModel collectionViewModel)
        {
            if (!id.HasValue)
            {
                return GetErrorAction("No Client ID Supplied");
            }
            IS4Entities.IdentityResource identityResource = GetIdentityResource(id.Value);
            if (identityResource == null)
            {
                return GetErrorAction("Could not load client");
            }

            if (ModelState.IsValid)
            {
                AddObject(identityResource, id.Value, collectionViewModel.NewItem);

                ConfigDbContext.IdentityResources.Update(identityResource);
                await ConfigDbContext.SaveChangesAsync();
                return GetEditRedirect(new { id = id.Value });
            }

            collectionViewModel.SetId(id.Value);
            collectionViewModel.ItemsList.AddRange(PopulateItemList(identityResource));

            return GetView(collectionViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id, int? identityResourceId)
        {
            if (!id.HasValue)
            {
                return GetErrorAction("No Item ID Supplied");
            }

            if (!identityResourceId.HasValue)
            {
                return GetErrorAction("No Identity Resource ID Supplied");
            }

            IS4Entities.IdentityResource identityResource = GetIdentityResource(identityResourceId.Value);
            if (identityResource == null)
            {
                return GetErrorAction("Could not load client");
            }

            RemoveObject(identityResource, id.Value);

            ConfigDbContext.IdentityResources.Update(identityResource);
            await ConfigDbContext.SaveChangesAsync();

            return GetEditRedirect(new { id = identityResourceId.Value });
        }

        #endregion Controller Actions

        #region Private Methods

        private IS4Entities.IdentityResource GetIdentityResource(int id)
        {
            var query = ConfigDbContext.IdentityResources;
            var includeQuery = AddIncludes(query);

            return includeQuery.FirstOrDefault(ir => ir.Id == id);
        }

        #endregion Private Methods
    }
}