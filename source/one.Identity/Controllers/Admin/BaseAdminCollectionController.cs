using AutoMapper;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using one.Identity.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using one.Identity.Models.Admin;

namespace one.Identity.Controllers.Admin
{
    public abstract class BaseAdminCollectionController<TSingleViewModel, TCollectionViewModel, TEntity, TChildEntity> : BaseAdminController
           where TSingleViewModel : BaseAdminChildItemViewModel, new()
           where TCollectionViewModel : BaseAdminChildCollectionViewModel<TSingleViewModel>, new()
           where TEntity : class
    {
        #region Constructor

        protected BaseAdminCollectionController(ConfigurationDbContext context) : base(context)
        {
        }

        #endregion Constructor

        #region BaseClientCollectionController Interface

        protected abstract IEnumerable<TSingleViewModel> PopulateItemList(TEntity mainEntity);

        protected abstract Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<TEntity, List<TChildEntity>> AddIncludes(DbSet<TEntity> query);

        protected abstract List<TChildEntity> GetCollection(TEntity mainEntity);

        protected abstract TChildEntity FindItemInCollection(List<TChildEntity> collection, int id);

        protected virtual IActionResult GetView(TCollectionViewModel model)
        {
            return View(nameof(Edit), model);
        }

        protected virtual IActionResult GetEditRedirect(object routeValues)
        {
            return RedirectToAction(nameof(Edit), routeValues);
        }

        protected abstract TEntity GetMainEntity(int id);

        protected virtual void SetAdditionalProperties(TChildEntity newItem)
        {
        }

        #endregion BaseClientCollectionController Interface

        #region Controller Actions

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            var collectionViewModel = new TCollectionViewModel();
            if (id.HasValue)
            {
                collectionViewModel.SetId(id.Value);

                TEntity entity = GetMainEntity(id.Value);

                if (entity == null)
                {
                    return GetErrorAction("Could not load mainEntity");
                }

                collectionViewModel.ItemsList.AddRange(PopulateItemList(entity));
            }

            return View(collectionViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Save(int? id, TCollectionViewModel collectionViewModel)
        {
            if (!id.HasValue)
            {
                return GetErrorAction("No ID Supplied for child item");
            }
            TEntity entity = GetMainEntity(id.Value);
            if (entity == null)
            {
                return GetErrorAction("Could not load main mainEntity");
            }

            if (ModelState.IsValid)
            {
                collectionViewModel.NewItem.ParentId = id.Value;
                AddObject(entity, collectionViewModel.NewItem);
                ConfigDbContext.Update(entity);
                await ConfigDbContext.SaveChangesAsync();
                return GetEditRedirect(new { id = id.Value });
            }

            collectionViewModel.SetId(id.Value);
            collectionViewModel.ItemsList.AddRange(PopulateItemList(entity));

            return GetView(collectionViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id, int? parentId)
        {
            if (!id.HasValue)
            {
                return GetErrorAction("No Item ID Supplied");
            }

            if (!parentId.HasValue)
            {
                return GetErrorAction("No main mainEntity ID supplied");
            }

            TEntity entity = GetMainEntity(parentId.Value);
            if (entity == null)
            {
                return GetErrorAction("Could not main mainEntity");
            }

            RemoveObject(entity, id.Value);

            ConfigDbContext.Update(entity);
            await ConfigDbContext.SaveChangesAsync();

            return GetEditRedirect(new { id = parentId.Value });
        }

        #endregion Controller Actions

        #region Private Methods

        private void RemoveObject(TEntity mainEntity, int id)
        {
            List<TChildEntity> collection = GetCollection(mainEntity);
            var prop = FindItemInCollection(collection, id);
            if (prop != null)
            {
                collection.Remove(prop);
            }
        }

        private void AddObject(TEntity mainEntity, TSingleViewModel newItem)
        {
            var newEntity = Mapper.Map<TChildEntity>(newItem);
            SetAdditionalProperties(newEntity);
            GetCollection(mainEntity).Add(newEntity);
        }

        #endregion Private Methods
    }
}