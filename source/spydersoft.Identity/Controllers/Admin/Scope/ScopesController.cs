using AutoMapper;
using AutoMapper.QueryableExtensions;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using spydersoft.Identity.Models.Admin.ScopeViewModels;

namespace spydersoft.Identity.Controllers.Admin.Scope
{
    public class ScopesController : BaseAdminController
    {
        public ScopesController(ConfigurationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        #region Client List Actions

        public IActionResult Index()
        {
            ViewData["Title"] = "Registered Scopes";

            var scopesModel = new ScopesViewModel()
            {
               Scopes = Mapper.ProjectTo<ScopeViewModel>(ConfigDbContext.ApiScopes.ToList().AsQueryable())
            };

            return View(scopesModel);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            ScopeViewModel scopeViewModel;
            if (!id.HasValue)
            {
                scopeViewModel = new ScopeViewModel();
                ViewData["Title"] = "New Scope";
            }
            else
            {
                var apiScope = ConfigDbContext.ApiScopes.FirstOrDefault(c => c.Id == id.Value);
                if (apiScope == null)
                {
                    return GetErrorAction("Could not load client");
                }

                scopeViewModel = new ScopeViewModel(apiScope.Id);
                Mapper.Map(apiScope, scopeViewModel);
                ViewData["Title"] = $"Edit {scopeViewModel.Name}";
            }

            return View(scopeViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id.HasValue)
            {
                var apiScope = ConfigDbContext.ApiScopes.FirstOrDefault(c => c.Id == id.Value);
                if (apiScope == null)
                {
                    return GetErrorAction("Could not load scope");
                }

                ConfigDbContext.ApiScopes.Remove(apiScope);
                await ConfigDbContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        #endregion Client List Actions

        #region Main Tab

        [HttpPost]
        public async Task<IActionResult> Edit(int? id, ScopeViewModel client)
        {
            if (ModelState.IsValid)
            {
                IdentityServer4.EntityFramework.Entities.ApiScope dbEntity;
                var isNew = false;

                if (!id.HasValue || id.Value == 0)
                {
                    dbEntity = new IdentityServer4.EntityFramework.Entities.ApiScope();
                    ConfigDbContext.Add(dbEntity);
                    isNew = true;
                }
                else
                {
                    client.Id = id.Value;
                    dbEntity = ConfigDbContext.ApiScopes.FirstOrDefault(c => c.Id == id.Value);
                }

                if (dbEntity != null)
                {
                    Mapper.Map(client, dbEntity);
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
                client.Id = id.Value;
            }

            return View(client);
        }

        #endregion Main Tab
    }
}