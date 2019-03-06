using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using one.Identity.Models.ClientViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IS4Entities = IdentityServer4.EntityFramework.Entities;

namespace one.Identity.Controllers.Admin.Client
{
    public abstract class BaseClientCollectionController<TSingleViewModel, TCollectionViewModel, TChildEntity> : BaseAdminController
        where TSingleViewModel : BaseClientChildItemViewModel, new()
        where TCollectionViewModel : BaseClientCollectionViewModel<TSingleViewModel>, new()
    {
        #region Constructor

        protected BaseClientCollectionController(ConfigurationDbContext context) : base(context)
        {
        }

        #endregion Constructor

        #region BaseClientCollectionController Interface

        protected abstract IEnumerable<TSingleViewModel> PopulateItemList(
            IS4Entities.Client client);

        protected abstract Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<IS4Entities.Client, List<TChildEntity>> AddIncludes(DbSet<IS4Entities.Client> query);

        protected abstract void RemoveObject(IS4Entities.Client client, int id);

        protected abstract void AddObject(IS4Entities.Client client, int clientId,
            TSingleViewModel newItem);

        protected virtual IActionResult GetView(TCollectionViewModel model)
        {
            return View(nameof(Edit), model);
        }

        protected virtual IActionResult GetEditRedirect(object routeValues)
        {
            return RedirectToAction(nameof(Edit), routeValues);
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

                IS4Entities.Client client = GetClient(id.Value);
                if (client == null)
                {
                    return GetErrorAction("Could not load client");
                }

                collectionViewModel.ItemsList.AddRange(PopulateItemList(client));
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
            IS4Entities.Client client = GetClient(id.Value);
            if (client == null)
            {
                return GetErrorAction("Could not load client");
            }

            if (ModelState.IsValid)
            {
                AddObject(client, id.Value, collectionViewModel.NewItem);

                ConfigDbContext.Clients.Update(client);
                await ConfigDbContext.SaveChangesAsync();
                return GetEditRedirect(new { id = id.Value });
            }

            collectionViewModel.SetId(id.Value);
            collectionViewModel.ItemsList.AddRange(PopulateItemList(client));

            return GetView(collectionViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id, int? clientId)
        {
            if (!id.HasValue)
            {
                return GetErrorAction("No Item ID Supplied");
            }

            if (!clientId.HasValue)
            {
                return GetErrorAction("No Client ID Supplied");
            }

            IS4Entities.Client client = GetClient(clientId.Value);
            if (client == null)
            {
                return GetErrorAction("Could not load client");
            }

            RemoveObject(client, id.Value);

            ConfigDbContext.Clients.Update(client);
            await ConfigDbContext.SaveChangesAsync();

            return GetEditRedirect(new { id = clientId.Value });
        }

        #endregion Controller Actions

        #region Private Methods

        private IS4Entities.Client GetClient(int id)
        {
            var query = ConfigDbContext.Clients;
            var includeQuery = AddIncludes(query);

            return includeQuery.FirstOrDefault(c => c.Id == id);
        }

        #endregion Private Methods
    }
}