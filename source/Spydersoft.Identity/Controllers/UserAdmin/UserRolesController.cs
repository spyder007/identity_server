using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using AutoMapper;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using Spydersoft.Identity.Models.Identity;

namespace Spydersoft.Identity.Controllers.UserAdmin
{
    public class UserRolesController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IMapper mapper) : BaseUserAdminController(userManager, roleManager, mapper)
    {

        #region Role List

        public IActionResult Index()
        {
            var viewModel = new ApplicationRolesViewModel
            {
                Roles = RoleManager.Roles
            };
            ViewData["Title"] = "Roles";
            return View(viewModel);
        }

        #endregion Role List

        #region Role Editing and Deletion

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var viewModel = new ApplicationRoleViewModel();
            if (string.IsNullOrEmpty(id))
            {
                viewModel.Role = new ApplicationRole();
                ViewData["Title"] = "New Role";
            }
            else
            {
                viewModel.Role = await RoleManager.FindByIdAsync(id);
                if (viewModel.Role == null)
                {
                    return GetErrorAction("Could not load role");
                }

                System.Collections.Generic.IList<Claim> claims = await RoleManager.GetClaimsAsync(viewModel.Role);
                viewModel.Claims = claims.AsQueryable();
                viewModel.NewClaim = new ClaimModel();
                ViewData["Title"] = $"Edit {viewModel.Role.Name}";
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, ApplicationRoleViewModel roleModel)
        {
            if (ModelState.IsValid)
            {
                ApplicationRole current = await RoleManager.FindByIdAsync(id);
                IdentityResult result;
                if (current == null)
                {
                    result = await RoleManager.CreateAsync(roleModel.Role);
                }
                else
                {
                    _ = Mapper.Map(roleModel.Role, current);
                    result = await RoleManager.UpdateAsync(current);
                }

                return !result.Succeeded ? GetErrorAction(result.ToString()) : RedirectToAction(nameof(Index));
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
            ApplicationRole role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                return GetErrorAction("Could ID provided");
            }

            IdentityResult result = await RoleManager.DeleteAsync(role);
            return !result.Succeeded ? GetErrorAction(result.ToString()) : RedirectToAction(nameof(Index));
        }

        #endregion Role Editing and Deletion

        #region Role Claim Add / Deletion

        [HttpPost]
        public async Task<IActionResult> AddClaim(string roleid, ApplicationRoleViewModel viewModel)
        {
            ApplicationRole current = await RoleManager.FindByIdAsync(roleid);
            if (current == null)
            {
                return GetErrorAction("Invalid role");
            }

            IdentityResult result = await RoleManager.AddClaimAsync(current, new Claim(viewModel.NewClaim.Type, viewModel.NewClaim.Value));
            return !result.Succeeded ? GetErrorAction(result.ToString()) : RedirectToAction(nameof(Edit), new { id = roleid });
        }

        [HttpGet]
        public async Task<IActionResult> DeleteClaim(string roleid, string claimtype)
        {
            ApplicationRole current = await RoleManager.FindByIdAsync(roleid);
            if (current == null)
            {
                return GetErrorAction("Invalid role");
            }

            System.Collections.Generic.IList<Claim> claims = await RoleManager.GetClaimsAsync(current);

            Claim claim = claims.FirstOrDefault(c => c.Type == claimtype);

            IdentityResult result = await RoleManager.RemoveClaimAsync(current, claim);
            return !result.Succeeded ? GetErrorAction(result.ToString()) : RedirectToAction(nameof(Edit), new { id = roleid });
        }

        #endregion Role Claim Add / Deletion
    }
}