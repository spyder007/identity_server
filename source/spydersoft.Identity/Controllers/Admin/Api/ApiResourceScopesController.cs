using System;
using AutoMapper.QueryableExtensions;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using spydersoft.Identity.Models.Admin.ApiResourceViewModels;

namespace spydersoft.Identity.Controllers.Admin.Api
{
    public class ApiResourceScopesController : BaseApiResourceCollectionController<ApiResourceScopeViewModel, ApiResourceScopesViewModel, ApiResourceScope>
    {
        public ApiResourceScopesController(ConfigurationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        #region BaseApiCollectionController Implementation

        protected override IEnumerable<ApiResourceScopeViewModel> PopulateItemList(ApiResource mainEntity)
        {
            return Mapper.ProjectTo<ApiResourceScopeViewModel>(mainEntity.Scopes.ToList().AsQueryable());
        }

        protected override IQueryable<ApiResource> AddIncludes(DbSet<ApiResource> query)
        {
            return query.Include(c => c.Scopes).ThenInclude(s => s.ApiResource);
        }

        protected override ApiResourceScope FindItemInCollection(List<ApiResourceScope> collection, int id)
        {
            return collection.Find(s => s.Id == id);
        }

        protected override List<ApiResourceScope> GetCollection(ApiResource mainEntity)
        {
            return mainEntity.Scopes;
        }

        #endregion BaseApiCollectionController Implementation

        public IActionResult Edit(int? id, int? parentid)
        {
            ApiResourceScopeViewModel model;
            if (!id.HasValue)
            {
                model = new ApiResourceScopeViewModel();
                ViewData["Title"] = "New API Scope";
            }
            else
            {
                ApiResourceScope apiScope = GetScope(parentid, id);
                if (apiScope == null)
                {
                    return GetErrorAction("Could not load api scope");
                }

                model = new ApiResourceScopeViewModel { Id = id.Value, ParentId = parentid.Value };

                Mapper.Map(apiScope, model);
                ViewData["Title"] = $"Edit API Scope {model.Scope}";
            }

            return View(nameof(Edit), model);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteClaim(int? id, int? scopeId, int? apiId)
        {
            if (!id.HasValue)
            {
                return GetErrorAction("No ID provided");
            }

            ApiResourceScope apiScope = GetScope(apiId, scopeId);
            if (apiScope == null)
            {
                return GetErrorAction("Could not load api scope");
            }
            
            var claim = apiScope.ApiResource.UserClaims.FirstOrDefault(uc => uc.Id == id.Value);
            apiScope.ApiResource.UserClaims.Remove(claim);
            ConfigDbContext.Update(apiScope);
            await ConfigDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Edit), new { id = scopeId.Value, parentid = apiId.Value });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int? id, int? parentid, ApiResourceScopeViewModel scopeModel)
        {
            if (ModelState.IsValid)
            {
                ApiResourceScope dbEntity;
                var isNew = false;

                if (!id.HasValue || id.Value == 0)
                {
                    dbEntity = new ApiResourceScope();
                    ConfigDbContext.Add(dbEntity);
                    isNew = true;
                }
                else
                {
                    scopeModel.Id = id.Value;
                    dbEntity = GetScope(parentid, id);
                }

                if (dbEntity != null)
                {
                    Mapper.Map(scopeModel, dbEntity);
                }

                if (!isNew)
                {
                    ConfigDbContext.Update(dbEntity);
                }

                await ConfigDbContext.SaveChangesAsync();

                return (isNew ? RedirectToAction(nameof(Edit), new { id = dbEntity.Id, parentid = parentid.Value }) : RedirectToAction(nameof(Index), new { id = parentid.Value }));
            }

            if (id.HasValue)
            {
                scopeModel.Id = id.Value;
                ApiResourceScope scope = GetScope(parentid, id);
            }

            return View(nameof(Edit), scopeModel);
        }

        private ApiResourceScope GetScope(int? apiId, int? id)
        {
            var apiResource = ConfigDbContext.ApiResources.Include(a => a.Scopes).ThenInclude(s => s.ApiResource).FirstOrDefault(c => c.Id == apiId.Value);

            var apiScope = apiResource?.Scopes.FirstOrDefault(s => s.Id == id.Value);
            
            return apiScope;
        }
    }
}