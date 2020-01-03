using AutoMapper;
using AutoMapper.QueryableExtensions;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using spydersoft.Identity.Models.Admin.ApiViewModels;

namespace spydersoft.Identity.Controllers.Admin.Api
{
    public class ApisController : BaseAdminController
    {
        public ApisController(ConfigurationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        #region Client List Actions

        public IActionResult Index()
        {
            var apisViewModel = new ApisViewModel
            {
                Apis = Mapper.ProjectTo<ApiViewModel>(ConfigDbContext.ApiResources.ToList().AsQueryable())
            };

            ViewData["Title"] = "Register APIs";
            return View(apisViewModel);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            ApiViewModel apiModel;
            if (!id.HasValue)
            {
                apiModel = new ApiViewModel();
                ViewData["Title"] = "New API";
            }
            else
            {
                var client = ConfigDbContext.ApiResources.FirstOrDefault(c => c.Id == id.Value);
                if (client == null)
                {
                    return GetErrorAction("Could not load api");
                }

                apiModel = new ApiViewModel(client.Id);
                Mapper.Map(client, apiModel);
                ViewData["Title"] = $"Edit {apiModel.NavBar.Name}";
            }
            
            return View(apiModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id.HasValue)
            {
                var api = ConfigDbContext.ApiResources.FirstOrDefault(c => c.Id == id.Value);
                if (api == null)
                {
                    return GetErrorAction("Could not load api");
                }

                ConfigDbContext.ApiResources.Remove(api);
                await ConfigDbContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        #endregion Client List Actions

        #region Main Tab

        [HttpPost]
        public async Task<IActionResult> Edit(int? id, ApiViewModel apiViewModel)
        {
            if (ModelState.IsValid)
            {
                IdentityServer4.EntityFramework.Entities.ApiResource dbEntity;
                var isNew = false;

                if (!id.HasValue || id.Value == 0)
                {
                    apiViewModel.Created = DateTime.UtcNow;
                    dbEntity = new IdentityServer4.EntityFramework.Entities.ApiResource();
                    ConfigDbContext.Add(dbEntity);
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
                    Mapper.Map(apiViewModel, dbEntity);
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
                apiViewModel.Id = id.Value;
            }

            return View(apiViewModel);
        }

        #endregion Main Tab
    }
}