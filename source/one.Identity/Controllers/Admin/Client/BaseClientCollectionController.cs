using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using one.Identity.Models.ClientViewModels;

namespace one.Identity.Controllers.Admin.Client
{
    public abstract class BaseClientCollectionController<TSingleViewModel, TCollectionViewModel, TChildEntity> : BaseClientController 
        where TSingleViewModel : BaseClientChildItemViewModel, new()
        where TCollectionViewModel : BaseClientCollectionViewModel<TSingleViewModel>, new()

    {
        protected BaseClientCollectionController(ConfigurationDbContext context) : base(context)
        {

        }

        protected abstract IEnumerable<TSingleViewModel> PopulateItemList(
            IdentityServer4.EntityFramework.Entities.Client client);

        protected abstract Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<IdentityServer4.EntityFramework.Entities.Client, List<TChildEntity>> AddIncludes(DbSet<IdentityServer4.EntityFramework.Entities.Client> query);

        protected abstract void RemoveObject(IdentityServer4.EntityFramework.Entities.Client client, int id);

        protected abstract void AddObject(IdentityServer4.EntityFramework.Entities.Client client, int clientId,
            TSingleViewModel newItem);

        protected abstract IActionResult GetView(TCollectionViewModel model);

        protected abstract IActionResult GetEditRedirect(object routeValues);

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            var collectionViewModel = new TCollectionViewModel();
            if (id.HasValue)
            {
                collectionViewModel.SetId(id.Value);

                IdentityServer4.EntityFramework.Entities.Client client = GetClient(id.Value);
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
            IdentityServer4.EntityFramework.Entities.Client client = GetClient(id.Value);
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
                return GetErrorAction("No Scope ID Supplied");
            }

            if (!clientId.HasValue)
            {
                return GetErrorAction("No Client ID Supplied");
            }

            IdentityServer4.EntityFramework.Entities.Client client = GetClient(clientId.Value);
            if (client == null)
            {
                return GetErrorAction("Could not load client");
            }

            RemoveObject(client, id.Value);
            
            ConfigDbContext.Clients.Update(client);
            await ConfigDbContext.SaveChangesAsync();

            return GetEditRedirect(new { id = clientId.Value });
        }

        private IdentityServer4.EntityFramework.Entities.Client GetClient(int id)
        {
            var query = ConfigDbContext.Clients;
            var includeQuery = AddIncludes(query);

            return includeQuery.FirstOrDefault(c => c.Id == id);
        }
    }
}
