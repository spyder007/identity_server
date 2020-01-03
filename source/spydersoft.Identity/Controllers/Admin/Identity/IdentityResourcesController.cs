using AutoMapper;
using AutoMapper.QueryableExtensions;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using spydersoft.Identity.Models.Admin.IdentityResourceViewModels;

namespace spydersoft.Identity.Controllers.Admin.Identity
{
    public class IdentityResourcesController : BaseAdminController
    {
        public IdentityResourcesController(ConfigurationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        #region Controller Actions

        public IActionResult Index()
        {
            ViewData["Title"] = "Registered Identity Resources";
            var identityResourcesViewModel = new IdentityResourcesViewModel
            {
                IdentityResources = Mapper.ProjectTo<IdentityResourceViewModel>(ConfigDbContext.IdentityResources.ToList().AsQueryable())
            };

            List<IdentityResourceViewModel> availableStandards = GetAllStandardsAsViewModels(identityResourcesViewModel.IdentityResources);

            identityResourcesViewModel.AvailableStandardResources = availableStandards
                .Where(s => !identityResourcesViewModel.IdentityResources.Any(ir => ir.Name == s.Name)).AsQueryable();

            return View(identityResourcesViewModel);
        }

        private List<IdentityResourceViewModel> GetAllStandardsAsViewModels(IQueryable<IdentityResourceViewModel> currentList)
        {
            var list = new List<IdentityResourceViewModel>();

            List<IdentityResource> standardTypes = GetStandardTypes();
            foreach (IdentityResource standardType in standardTypes)
            {
                if (!currentList.Any(irvm => irvm.Name == standardType.Name))
                {
                    var idViewModel = new IdentityResourceViewModel();
                    Mapper.Map(standardType, idViewModel);

                    //list.Add(Mapper.Map<IdentityResourceViewModel>(standardType));
                    list.Add(idViewModel);
                }
            }

            return list;
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            IdentityResourceViewModel identityResourceViewModel;
            if (!id.HasValue)
            {
                identityResourceViewModel = new IdentityResourceViewModel();
                ViewData["Title"] = "New Identity Resource";
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
                ViewData["Title"] = $"Edit {identityResourceViewModel.NavBar.Name}";
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
        public async Task<IActionResult> AddStandard(IdentityResourcesViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.SelectedAvailableResource))
            {
                IdentityServer4.EntityFramework.Entities.IdentityResource resource = GetStandardProfile(model.SelectedAvailableResource);
                if (resource != null)
                {
                    ConfigDbContext.IdentityResources.Add(resource);
                    await ConfigDbContext.SaveChangesAsync();
                }
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

            if (id.HasValue)
            {
                identityResource.Id = id.Value;
            }

            return View(identityResource);
        }
        #endregion Controller Actions

        private IdentityServer4.EntityFramework.Entities.IdentityResource GetStandardProfile(string modelSelectedAvailableResource)
        {
            if (string.IsNullOrWhiteSpace(modelSelectedAvailableResource))
            {
                return null;
            }

            List<IdentityResource> standardTypes = GetStandardTypes();
            var typeToAdd = standardTypes.FirstOrDefault(t => t.Name == modelSelectedAvailableResource);

            return typeToAdd?.ToEntity();
        }

        private List<IdentityResource> GetStandardTypes()
        {
            return new List<IdentityResource>()
            {
                new IdentityResources.Profile(),
                new IdentityResources.Address(),
                new IdentityResources.Email(),
                new IdentityResources.OpenId(),
                new IdentityResources.Phone()
            };
        }
    }
}