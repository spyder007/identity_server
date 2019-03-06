using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Mvc;
using one.Identity.Models.ClientViewModels;
using one.Identity.Models.IdentityResourceViewModels;

namespace one.Identity.Controllers.Admin.Identity
{
    public class IdentityResourcesController : BaseAdminController
    {
        public IdentityResourcesController(ConfigurationDbContext dbContext) : base(dbContext)
        {
        }

        public IActionResult Index()
        {
            var identityResourcesViewModel = new IdentityResourcesViewModel
            {
                IdentityResources = ConfigDbContext.IdentityResources.ProjectTo<IdentityResourceViewModel>()
            };

            return View(identityResourcesViewModel);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            IdentityResourceViewModel identityResourceViewModel;
            if (!id.HasValue)
            {
                identityResourceViewModel = new IdentityResourceViewModel();
            }
            else
            {
                var identityResource = ConfigDbContext.IdentityResources.FirstOrDefault(ir => ir.Id == id.Value);
                if (identityResource == null)
                {
                    return GetErrorAction("Could not load identity resource");
                }

                identityResourceViewModel = new IdentityResourceViewModel(id.Value);
                Mapper.Map(identityResource, identityResourceViewModel);
                identityResourceViewModel.Id = id.Value;
            }

            return View(identityResourceViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id.HasValue)
            {
                var identityResource = ConfigDbContext.IdentityResources.FirstOrDefault(ir => ir.Id == id.Value);
                if (identityResource == null)
                {
                    return GetErrorAction("Could not load resource");
                }

                ConfigDbContext.IdentityResources.Remove(identityResource);
                await ConfigDbContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int? id, IdentityResourceViewModel identityResource)
        {
            if (ModelState.IsValid)
            {
                IdentityServer4.EntityFramework.Entities.IdentityResource dbEntity;
                var isNew = false;

                if (!id.HasValue || id.Value == 0)
                {
                    dbEntity = new IdentityServer4.EntityFramework.Entities.IdentityResource();
                    ConfigDbContext.Add(dbEntity);
                    identityResource.Created = DateTime.UtcNow;
                    isNew = true;
                }
                else
                {
                    identityResource.Id = id.Value;
                    dbEntity = ConfigDbContext.IdentityResources.FirstOrDefault(ir => ir.Id == id.Value);
                }

                identityResource.Updated = DateTime.UtcNow;

                if (dbEntity != null)
                {
                    Mapper.Map(identityResource, dbEntity);
                }

                if (!isNew)
                {
                    ConfigDbContext.Update(dbEntity);
                }
                
                await ConfigDbContext.SaveChangesAsync();

                return (isNew ? RedirectToAction(nameof(Edit), new { id = dbEntity.Id }) : RedirectToAction(nameof(Index)));
            }

            return View(identityResource);
        }
    }
}
