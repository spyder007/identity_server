using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using spydersoft.Identity.Models.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace spydersoft.Identity.Controllers.UserAdmin
{
    public class UsersController : BaseUserAdminController
    {
        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager) : base(userManager, roleManager)
        {
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new UsersViewModel { Users = UserManager.Users };
            return View(model);
        }

        #region Edit User

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var viewModel = new UserViewModel();
            if (string.IsNullOrEmpty(id))
            {
                viewModel.User = new ApplicationUser();
                viewModel.IsNewUser = true;

                ViewData["Title"] = "New User";
            }
            else
            {
                viewModel.User = await UserManager.FindByIdAsync(id);
                if (viewModel.User == null)
                {
                    return GetErrorAction("Could not load user");
                }

                viewModel.IsNewUser = false;
                var roles = await UserManager.GetRolesAsync(viewModel.User);
                var allRoles = RoleManager.Roles.Select(r => r.Name);
                viewModel.Roles = roles.AsQueryable();
                viewModel.AvailableRoles = allRoles.Where(r => !viewModel.Roles.Any(ur => ur == r));
                ViewData["Title"] = $"Edit {viewModel.User.UserName}";
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, UserViewModel userModel)
        {
            if (ModelState.IsValid)
            {
                var current = await UserManager.FindByIdAsync(id);
                IdentityResult result;
                if (current == null)
                {
                    result = await UserManager.CreateAsync(userModel.User, userModel.Password);
                }
                else
                {
                    Mapper.Map(userModel.User, current);
                    result = await UserManager.UpdateAsync(current);
                }

                if (!result.Succeeded)
                {
                    return GetErrorAction(result.ToString());
                }

                return RedirectToAction(nameof(Index));
            }

            return View(userModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return GetErrorAction("Invalid ID provided");
            }
            var applicationUser = await UserManager.FindByIdAsync(id);
            if (applicationUser == null)
            {
                return GetErrorAction("Could ID provided");
            }

            IdentityResult result = await UserManager.DeleteAsync(applicationUser);
            if (!result.Succeeded)
            {
                return GetErrorAction(result.ToString());
            }

            return RedirectToAction(nameof(Index));
        }

        #endregion Edit User

        #region Add / Delete Roles

        [HttpPost]
        public async Task<IActionResult> AddRole(string userid, UserViewModel model)
        {
            var current = await UserManager.FindByIdAsync(userid);
            if (current == null)
            {
                return GetErrorAction("Invalid user");
            }

            IdentityResult result = await UserManager.AddToRoleAsync(current, model.SelectedAvailableRole);
            if (!result.Succeeded)
            {
                return GetErrorAction(result.ToString());
            }

            return RedirectToAction(nameof(Edit), new { id = userid });
        }

        [HttpGet]
        public async Task<IActionResult> DeleteRole(string userid, string role)
        {
            var current = await UserManager.FindByIdAsync(userid);
            if (current == null)
            {
                return GetErrorAction("Invalid role");
            }

            IdentityResult result = await UserManager.RemoveFromRoleAsync(current, role);
            if (!result.Succeeded)
            {
                return GetErrorAction(result.ToString());
            }

            return RedirectToAction(nameof(Edit), new { id = userid });
        }

        #endregion Add / Delete Roles
    }
}