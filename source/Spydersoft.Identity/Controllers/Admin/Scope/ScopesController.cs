using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Duende.IdentityServer.EntityFramework.DbContexts;

using Microsoft.AspNetCore.Mvc;

using Spydersoft.Identity.Constants;
using Spydersoft.Identity.Models.Admin.ScopeViewModels;

namespace Spydersoft.Identity.Controllers.Admin.Scope
{
    /// <summary>
    /// Class ScopesController.
    /// Implements the <see cref="BaseAdminController" />
    /// </summary>
    /// <seealso cref="BaseAdminController" />
    public class ScopesController(ConfigurationDbContext dbContext, IMapper mapper) : BaseAdminController(dbContext, mapper)
    {

        #region Client List Actions

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>IActionResult.</returns>
        public IActionResult Index()
        {
            ViewData["Title"] = "Registered Scopes";

            var scopesModel = new ScopesViewModel()
            {
                Scopes = Mapper.ProjectTo<ScopeViewModel>(ConfigDbContext.ApiScopes.AsQueryable())
            };

            return View(scopesModel);
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
                return GetErrorAction(Messages.InvalidRequest);
            }
            if (id.HasValue)
            {
                Duende.IdentityServer.EntityFramework.Entities.ApiScope apiScope = ConfigDbContext.ApiScopes.FirstOrDefault(c => c.Id == id.Value);
                if (apiScope == null)
                {
                    return GetErrorAction("Could not load scope");
                }

                _ = ConfigDbContext.ApiScopes.Remove(apiScope);
                _ = await ConfigDbContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        #endregion Client List Actions

        #region Main Tab
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
            ScopeViewModel scopeViewModel;
            if (!id.HasValue)
            {
                scopeViewModel = new ScopeViewModel();
                ViewData["Title"] = "New Scope";
            }
            else
            {
                Duende.IdentityServer.EntityFramework.Entities.ApiScope apiScope = ConfigDbContext.ApiScopes.FirstOrDefault(c => c.Id == id.Value);
                if (apiScope == null)
                {
                    return GetErrorAction("Could not load client");
                }

                scopeViewModel = new ScopeViewModel(apiScope.Id);
                _ = Mapper.Map(apiScope, scopeViewModel);
                ViewData["Title"] = $"Edit {scopeViewModel.Name}";
            }

            return View(scopeViewModel);
        }

        /// <summary>
        /// Edits the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="client">The client.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        public async Task<IActionResult> Edit(int? id, ScopeViewModel client)
        {
            if (ModelState.IsValid)
            {
                Duende.IdentityServer.EntityFramework.Entities.ApiScope dbEntity;
                var isNew = false;

                if (!id.HasValue || id.Value == 0)
                {
                    dbEntity = new Duende.IdentityServer.EntityFramework.Entities.ApiScope();
                    _ = ConfigDbContext.Add(dbEntity);
                    isNew = true;
                }
                else
                {
                    client.Id = id.Value;
                    dbEntity = ConfigDbContext.ApiScopes.FirstOrDefault(c => c.Id == id.Value);
                }

                if (dbEntity != null)
                {
                    _ = Mapper.Map(client, dbEntity);
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
                client.Id = id.Value;
            }

            return View(client);
        }

        #endregion Main Tab
    }
}