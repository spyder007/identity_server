using AutoMapper;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spydersoft.Identity.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using spydersoft.Identity.Models.Admin;

namespace spydersoft.Identity.Controllers.Admin
{
    public abstract class BaseAdminCollectionController<TChildViewModel, TChildCollectionViewModel, TMainEntityViewModel, TEntity, TChildEntity> : BaseAdminController
           where TChildViewModel : BaseAdminChildItemViewModel, new()
           where TChildCollectionViewModel : BaseAdminChildCollectionViewModel<TChildViewModel, TMainEntityViewModel>, new()
           where TMainEntityViewModel : BaseAdminViewModel, new()
           where TEntity : class
    {
        #region Constructor

        protected BaseAdminCollectionController(ConfigurationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        #endregion Constructor

        #region BaseClientCollectionController Interface

        protected abstract IEnumerable<TChildViewModel> PopulateItemList(TEntity mainEntity);

        protected abstract IQueryable<TEntity> AddIncludes(DbSet<TEntity> query);

        protected abstract List<TChildEntity> GetCollection(TEntity mainEntity);

        protected abstract TChildEntity FindItemInCollection(List<TChildEntity> collection, int id);

        protected virtual IActionResult GetView(TChildCollectionViewModel model)
        {
            return View(nameof(Index), model);
        }

        protected virtual IActionResult GetIndexRedirect(object routeValues)
        {
            return RedirectToAction(nameof(Index), routeValues);
        }

        protected abstract TEntity GetMainEntity(int id);

        protected virtual void SetAdditionalProperties(TChildEntity newItem)
        {
        }

        #endregion BaseClientCollectionController Interface

        #region Controller Actions

        [HttpGet]
        public IActionResult Index(int? id)
        {
            var collectionViewModel = new TChildCollectionViewModel();
            if (id.HasValue)
            {
                TEntity entity = GetMainEntity(id.Value);

                if (entity == null)
                {
                    return GetErrorAction("Could not load mainEntity");
                }

                collectionViewModel.SetMainViewModel(Mapper.Map<TMainEntityViewModel>(entity), ConfigDbContext);
                collectionViewModel.ItemsList.AddRange(PopulateItemList(entity));
                ViewData["Title"] = $"Edit {collectionViewModel.NavBar.Name}";
            }

            return View(collectionViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Save(int? id, TChildCollectionViewModel collectionViewModel)
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
                return GetIndexRedirect(new { id = id.Value });
            }

            collectionViewModel.SetMainViewModel(Mapper.Map<TMainEntityViewModel>(entity), ConfigDbContext);
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

            return GetIndexRedirect(new { id = parentId.Value });
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

        private void AddObject(TEntity mainEntity, TChildViewModel newItem)
        {
            var newEntity = Mapper.Map<TChildEntity>(newItem);
            SetAdditionalProperties(newEntity);
            GetCollection(mainEntity).Add(newEntity);
        }

        #endregion Private Methods
    }
}