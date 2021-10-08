using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using spydersoft.Identity.Models.Identity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;

namespace spydersoft.Identity.Controllers.UserAdmin
{
    public class UsersController : BaseUserAdminController
    {
        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IMapper mapper) 
            : base(userManager, roleManager, mapper)
        {
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new UsersViewModel { Users = UserManager.Users };
            ViewData["Title"] = "Registered Users";
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
                var userRoles = roles.ToList();
                viewModel.AvailableRoles = allRoles.Where(r => userRoles.All(ur => ur != r));

                var userClaims = await UserManager.GetClaimsAsync(viewModel.User);
                viewModel.Claims = Mapper.ProjectTo<ClaimModel>(userClaims.ToList().AsQueryable());

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

        #region Add / Delete Claims

        [HttpPost]
        public async Task<IActionResult> AddClaim(string userid, UserViewModel model)
        {
            var current = await UserManager.FindByIdAsync(userid);
            if (current == null)
            {
                return GetErrorAction("Invalid user");
            }

            IdentityResult result = await UserManager.AddClaimAsync(current, Mapper.Map<Claim>(model.NewClaim));
            if (!result.Succeeded)
            {
                return GetErrorAction(result.ToString());
            }

            return RedirectToAction(nameof(Edit), new { id = userid });
        }

        [HttpGet]
        public async Task<IActionResult> DeleteClaim(string userid, string claimtype)
        {
            var current = await UserManager.FindByIdAsync(userid);
            if (current == null)
            {
                return GetErrorAction("Invalid role");
            }

            var claims = await UserManager.GetClaimsAsync(current);
            var claim = claims.FirstOrDefault(c => c.Type == claimtype);

            IdentityResult result = await UserManager.RemoveClaimAsync(current, claim);
            if (!result.Succeeded)
            {
                return GetErrorAction(result.ToString());
            }

            return RedirectToAction(nameof(Edit), new { id = userid });
        }

        #endregion Add / Delete Roles
    }
}