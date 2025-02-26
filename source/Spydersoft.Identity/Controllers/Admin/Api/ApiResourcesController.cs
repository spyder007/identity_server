﻿using System;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Duende.IdentityServer.EntityFramework.DbContexts;

using Microsoft.AspNetCore.Mvc;

using Spydersoft.Identity.Models.Admin.ApiResourceViewModels;

namespace Spydersoft.Identity.Controllers.Admin.Api
{
    /// <summary>
    /// Class ApiResourcesController.
    /// Implements the <see cref="BaseAdminController" />
    /// </summary>
    /// <seealso cref="BaseAdminController" />
    public class ApiResourcesController(ConfigurationDbContext dbContext, IMapper mapper) : BaseAdminController(dbContext, mapper)
    {

        #region Client List Actions

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>IActionResult.</returns>
        public IActionResult Index()
        {
            var apisViewModel = new ApiResourcesViewModel
            {
                Apis = Mapper.ProjectTo<ApiResourceViewModel>(ConfigDbContext.ApiResources.AsQueryable())
            };

            ViewData["Title"] = "Register API Resources";
            return View(apisViewModel);
        }

        /// <summary>
        /// Edits the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            ApiResourceViewModel apiModel;
            if (!id.HasValue)
            {
                apiModel = new ApiResourceViewModel();
                ViewData["Title"] = "New API";
            }
            else
            {
                Duende.IdentityServer.EntityFramework.Entities.ApiResource client = ConfigDbContext.ApiResources.FirstOrDefault(c => c.Id == id.Value);
                if (client == null)
                {
                    return GetErrorAction("Could not load api");
                }

                apiModel = new ApiResourceViewModel(client.Id);
                _ = Mapper.Map(client, apiModel);
                ViewData["Title"] = $"Edit {apiModel.NavBar.Name}";
            }

            return View(apiModel);
        }

        /// <summary>
        /// Edits the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="apiViewModel">The API view model.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        public async Task<IActionResult> Edit(int? id, ApiResourceViewModel apiViewModel)
        {
            if (ModelState.IsValid)
            {
                Duende.IdentityServer.EntityFramework.Entities.ApiResource dbEntity;
                var isNew = false;

                if (!id.HasValue || id.Value == 0)
                {
                    apiViewModel.Created = DateTime.UtcNow;
                    dbEntity = new Duende.IdentityServer.EntityFramework.Entities.ApiResource();
                    _ = ConfigDbContext.Add(dbEntity);
                    isNew = true;
                }
                else
                {
                    apiViewModel.Id = id.Value;
                    dbEntity = ConfigDbContext.ApiResources.FirstOrDefault(c => c.Id == id.Value);
                }

                apiViewModel.Updated = DateTime.UtcNow;

                if (dbEntity != null)
                {
                    _ = Mapper.Map(apiViewModel, dbEntity);
                }

                if (!isNew)
                {
                    _ = ConfigDbContext.Update(dbEntity);
                }

                _ = await ConfigDbContext.SaveChangesAsync();

                return isNew ? RedirectToAction(nameof(Edit), new { id = dbEntity.Id }) : RedirectToAction(nameof(Index));
            }

            if (id.HasValue)
            {
                apiViewModel.Id = id.Value;
            }

            return View(apiViewModel);
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (id.HasValue)
            {
                Duende.IdentityServer.EntityFramework.Entities.ApiResource api = ConfigDbContext.ApiResources.FirstOrDefault(c => c.Id == id.Value);
                if (api == null)
                {
                    return GetErrorAction("Could not load api");
                }

                _ = ConfigDbContext.ApiResources.Remove(api);
                _ = await ConfigDbContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        #endregion Client List Actions
    }
}