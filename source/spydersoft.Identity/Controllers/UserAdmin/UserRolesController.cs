using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using spydersoft.Identity.Models;
using spydersoft.Identity.Models.Admin.ApiViewModels;
using spydersoft.Identity.Models.Identity;

namespace spydersoft.Identity.Controllers.UserAdmin
{
    public class UserRolesController : BaseUserAdminController
    {
        public UserRolesController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager) : base(userManager, roleManager)
        {
        }

        public IActionResult Index()
        {
            var viewModel = new ApplicationRolesViewModel
            {
                Roles = RoleManager.Roles
            };
            ViewData["Title"] = "Roles";
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            ApplicationRole role;
            if (string.IsNullOrEmpty(id))
            {
                role = new ApplicationRole();
                ViewData["Title"] = "New Role";
            }
            else
            {
                role = RoleManager.Roles.FirstOrDefault(r => r.Id == id);
                if (role == null)
                {
                    return GetErrorAction("Could not load role");
                }

                ViewData["Title"] = $"Edit {role.Name}";
            }

            return View(role);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, ApplicationRole roleModel)
        {
            if (ModelState.IsValid)
            {
                var current = await RoleManager.FindByIdAsync(id);
                IdentityResult result;
                if (current == null)
                {
                    result = await RoleManager.CreateAsync(roleModel);

                }
                else
                {
                    Mapper.Map(roleModel, current);
                    result = await RoleManager.UpdateAsync(current);
                }

                if (!result.Succeeded)
                {
                    return GetErrorAction(result.ToString());
                }

                return RedirectToAction(nameof(Index));
            }

            return View(roleModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return GetErrorAction("Invalid ID provided");
            }
            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                return GetErrorAction("Could ID provided");
            }

            IdentityResult result = await RoleManager.DeleteAsync(role);
            if (!result.Succeeded)
            {
                return GetErrorAction(result.ToString());
            }

            return RedirectToAction(nameof(Index));
        }
    }


}
