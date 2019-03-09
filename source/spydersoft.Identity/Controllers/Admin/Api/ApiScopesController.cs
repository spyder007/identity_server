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
using spydersoft.Identity.Models.Admin.ApiViewModels;

namespace spydersoft.Identity.Controllers.Admin.Api
{
    public class ApiScopesController : BaseApiCollectionController<ApiScopeViewModel, ApiScopesViewModel, ApiScope>
    {
        public ApiScopesController(ConfigurationDbContext context) : base(context)
        {
        }

        #region BaseApiCollectionController Implementation

        protected override IEnumerable<ApiScopeViewModel> PopulateItemList(ApiResource mainEntity)
        {
            return mainEntity.Scopes.AsQueryable().ProjectTo<ApiScopeViewModel>();
        }

        protected override IQueryable<ApiResource> AddIncludes(DbSet<ApiResource> query)
        {
            return query.Include(c => c.Scopes).ThenInclude(s => s.UserClaims);
        }

        protected override ApiScope FindItemInCollection(List<ApiScope> collection, int id)
        {
            return collection.Find(s => s.Id == id);
        }

        protected override List<ApiScope> GetCollection(ApiResource mainEntity)
        {
            return mainEntity.Scopes;
        }

        #endregion BaseApiCollectionController Implementation

        public IActionResult Edit(int? id, int? parentid)
        {
            ApiScopeViewModel model;
            if (!id.HasValue)
            {
                model = new ApiScopeViewModel();
            }
            else
            {
                ApiScope apiScope = GetScope(parentid, id);
                if (apiScope == null)
                {
                    return GetErrorAction("Could not load api scope");
                }

                model = new ApiScopeViewModel { Id = id.Value, ParentId = parentid.Value };

                Mapper.Map(apiScope, model);
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

            ApiScope apiScope = GetScope(apiId, scopeId);
            if (apiScope == null)
            {
                return GetErrorAction("Could not load api scope");
            }

            var claim = apiScope.UserClaims.FirstOrDefault(uc => uc.Id == id.Value);
            apiScope.UserClaims.Remove(claim);
            ConfigDbContext.Update(apiScope);
            await ConfigDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Edit), new { id = scopeId.Value, parentid = apiId.Value });
        }

        [HttpPost]
        public async Task<IActionResult> AddClaim(int? scopeId, int? apiId, ApiScopeViewModel scopeModel)
        {
            ApiScope apiScope = GetScope(apiId, scopeId);
            if (apiScope == null)
            {
                return GetErrorAction("Could not load api scope");
            }

            if (ModelState.IsValid)
            {
                scopeModel.NewClaim.ParentId = scopeId.Value;
                apiScope.UserClaims.Add(Mapper.Map<ApiScopeClaim>(scopeModel.NewClaim));
                ConfigDbContext.Update(apiScope);
                await ConfigDbContext.SaveChangesAsync();
            }
            else
            {
                scopeModel.UserClaims.AddRange(apiScope.UserClaims.AsQueryable().ProjectTo<ApiScopeClaimViewModel>());
            }

            return RedirectToAction(nameof(Edit), new { id = scopeId.Value, parentid = apiId.Value });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int? id, int? parentid, ApiScopeViewModel scopeModel)
        {
            if (ModelState.IsValid)
            {
                ApiScope dbEntity;
                var isNew = false;

                if (!id.HasValue || id.Value == 0)
                {
                    dbEntity = new ApiScope();
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
                ApiScope scope = GetScope(parentid, id);
                scopeModel.UserClaims.AddRange(scope.UserClaims.AsQueryable().ProjectTo<ApiScopeClaimViewModel>());
            }

            return View(nameof(Edit), scopeModel);
        }

        private ApiScope GetScope(int? apiId, int? id)
        {
            var apiResource = ConfigDbContext.ApiResources.Include(a => a.Scopes).ThenInclude(s => s.UserClaims).FirstOrDefault(c => c.Id == apiId.Value);

            var apiScope = apiResource?.Scopes.FirstOrDefault(s => s.Id == id.Value);

            return apiScope;
        }
    }
}