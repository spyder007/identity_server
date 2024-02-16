using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Duende.IdentityServer.EntityFramework.DbContexts;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Spydersoft.Identity.Models.Admin;

namespace Spydersoft.Identity.Controllers.Admin
{
    /// <summary>
    /// Class BaseAdminCollectionController.
    /// Implements the <see cref="BaseAdminController" />
    /// </summary>
    /// <typeparam name="TChildViewModel">The type of the t child view model.</typeparam>
    /// <typeparam name="TChildCollectionViewModel">The type of the t child collection view model.</typeparam>
    /// <typeparam name="TMainEntityViewModel">The type of the t main entity view model.</typeparam>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TChildEntity">The type of the t child entity.</typeparam>
    /// <seealso cref="BaseAdminController" />
    /// <remarks>
    /// Initializes a new instance of the <see cref="BaseAdminCollectionController{TChildViewModel, TChildCollectionViewModel, TMainEntityViewModel, TEntity, TChildEntity}"/> class.
    /// </remarks>
    /// <param name="context">The context.</param>
    /// <param name="mapper">The mapper.</param>
    public abstract class BaseAdminCollectionController<TChildViewModel, TChildCollectionViewModel, TMainEntityViewModel, TEntity, TChildEntity>(ConfigurationDbContext context, IMapper mapper) : BaseAdminController(context, mapper)
           where TChildViewModel : BaseAdminChildItemViewModel, new()
           where TChildCollectionViewModel : BaseAdminChildCollectionViewModel<TChildViewModel, TMainEntityViewModel>, new()
           where TMainEntityViewModel : BaseAdminViewModel, new()
           where TEntity : class
    {

        #region Constructor

        #endregion Constructor

        #region BaseClientCollectionController Interface

        /// <summary>
        /// Populates the item list.
        /// </summary>
        /// <param name="mainEntity">The main entity.</param>
        /// <returns>IEnumerable&lt;TChildViewModel&gt;.</returns>
        protected abstract IEnumerable<TChildViewModel> PopulateItemList(TEntity mainEntity);

        /// <summary>
        /// Adds the includes.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>IQueryable&lt;TEntity&gt;.</returns>
        protected abstract IQueryable<TEntity> AddIncludes(DbSet<TEntity> query);

        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <param name="mainEntity">The main entity.</param>
        /// <returns>List&lt;TChildEntity&gt;.</returns>
        protected abstract List<TChildEntity> GetCollection(TEntity mainEntity);

        /// <summary>
        /// Finds the item in collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>TChildEntity.</returns>
        protected abstract TChildEntity FindItemInCollection(List<TChildEntity> collection, int id);

        /// <summary>
        /// Gets the view.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>IActionResult.</returns>
        protected virtual IActionResult GetView(TChildCollectionViewModel model)
        {
            return View(nameof(Index), model);
        }

        /// <summary>
        /// Gets the index redirect.
        /// </summary>
        /// <param name="routeValues">The route values.</param>
        /// <returns>IActionResult.</returns>
        protected virtual IActionResult GetIndexRedirect(object routeValues)
        {
            return RedirectToAction(nameof(Index), routeValues);
        }

        /// <summary>
        /// Gets the main entity.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>TEntity.</returns>
        protected abstract TEntity GetMainEntity(int id);

        /// <summary>
        /// Sets the additional properties.
        /// </summary>
        /// <param name="newItem">The new item.</param>
        protected virtual void SetAdditionalProperties(TChildEntity newItem)
        {
        }

        #endregion BaseClientCollectionController Interface

        #region Controller Actions

        /// <summary>
        /// Indexes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>IActionResult.</returns>
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

        /// <summary>
        /// Saves the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="collectionViewModel">The collection view model.</param>
        /// <returns>IActionResult.</returns>
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
                _ = ConfigDbContext.Update(entity);
                _ = await ConfigDbContext.SaveChangesAsync();
                return GetIndexRedirect(new { id = id.Value });
            }

            collectionViewModel.SetMainViewModel(Mapper.Map<TMainEntityViewModel>(entity), ConfigDbContext);
            collectionViewModel.ItemsList.AddRange(PopulateItemList(entity));

            return GetView(collectionViewModel);
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="parentId">The parent identifier.</param>
        /// <returns>IActionResult.</returns>
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

            _ = ConfigDbContext.Update(entity);
            _ = await ConfigDbContext.SaveChangesAsync();

            return GetIndexRedirect(new { id = parentId.Value });
        }

        #endregion Controller Actions

        #region Private Methods

        /// <summary>
        /// Removes the object.
        /// </summary>
        /// <param name="mainEntity">The main entity.</param>
        /// <param name="id">The identifier.</param>
        private void RemoveObject(TEntity mainEntity, int id)
        {
            List<TChildEntity> collection = GetCollection(mainEntity);
            TChildEntity prop = FindItemInCollection(collection, id);
            if (prop != null)
            {
                _ = collection.Remove(prop);
            }
        }

        /// <summary>
        /// Adds the object.
        /// </summary>
        /// <param name="mainEntity">The main entity.</param>
        /// <param name="newItem">The new item.</param>
        private void AddObject(TEntity mainEntity, TChildViewModel newItem)
        {
            TChildEntity newEntity = Mapper.Map<TChildEntity>(newItem);
            SetAdditionalProperties(newEntity);
            GetCollection(mainEntity).Add(newEntity);
        }

        #endregion Private Methods
    }
}