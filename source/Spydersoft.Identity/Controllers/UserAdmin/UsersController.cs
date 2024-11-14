using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using AutoMapper;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using Spydersoft.Identity.Constants;
using Spydersoft.Identity.Models.Identity;

namespace Spydersoft.Identity.Controllers.UserAdmin
{
    /// <summary>
    /// Class UsersController.
    /// Implements the <see cref="BaseUserAdminController" />
    /// </summary>
    /// <seealso cref="BaseUserAdminController" />
    public class UsersController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IMapper mapper) : BaseUserAdminController(userManager, roleManager, mapper)
    {
        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        public IActionResult Index()
        {
            var model = new UsersViewModel { Users = UserManager.Users };
            ViewData["Title"] = "Registered Users";
            return View(model);
        }

        #region Edit User

        /// <summary>
        /// Edits the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var viewModel = new UserViewModel();

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

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
                System.Collections.Generic.IList<string> roles = await UserManager.GetRolesAsync(viewModel.User);
                var allRoles = RoleManager.Roles.Select(r => r.Name).ToList();
                viewModel.Roles = roles.AsQueryable();
                var userRoles = roles.ToList();
                viewModel.AvailableRoles = allRoles.Where(allRole => userRoles.TrueForAll(ur => ur != allRole)).AsQueryable();

                System.Collections.Generic.IList<Claim> userClaims = await UserManager.GetClaimsAsync(viewModel.User);
                viewModel.Claims = Mapper.ProjectTo<ClaimModel>(userClaims.AsQueryable());

                ViewData["Title"] = $"Edit {viewModel.User.UserName}";
            }

            return View(viewModel);
        }

        /// <summary>
        /// Edits the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userModel">The user model.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        public async Task<IActionResult> Edit(string id, UserViewModel userModel)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser current = await UserManager.FindByIdAsync(id);
                IdentityResult result;
                if (current == null)
                {
                    result = await UserManager.CreateAsync(userModel.User, userModel.Password);
                }
                else
                {
                    _ = Mapper.Map(userModel.User, current);
                    result = await UserManager.UpdateAsync(current);
                }

                return !result.Succeeded ? GetErrorAction(result.ToString()) : RedirectToAction(nameof(Index));
            }

            return View(userModel);
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (!ModelState.IsValid)
            {
                return GetErrorAction("Invalid ID provided");
            }

            if (string.IsNullOrEmpty(id))
            {
                return GetErrorAction("Invalid ID provided");
            }
            ApplicationUser applicationUser = await UserManager.FindByIdAsync(id);
            if (applicationUser == null)
            {
                return GetErrorAction("Could ID provided");
            }

            IdentityResult result = await UserManager.DeleteAsync(applicationUser);
            return !result.Succeeded ? GetErrorAction(result.ToString()) : RedirectToAction(nameof(Index));
        }

        #endregion Edit User

        #region Add / Delete Roles

        /// <summary>
        /// Adds the role.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <param name="model">The model.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        public async Task<IActionResult> AddRole(string userid, UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return GetErrorAction(Messages.InvalidUser);
            }

            ApplicationUser current = await UserManager.FindByIdAsync(userid);
            if (current == null)
            {
                return GetErrorAction(Messages.InvalidUser);
            }

            IdentityResult result = await UserManager.AddToRoleAsync(current, model.SelectedAvailableRole);
            return !result.Succeeded ? GetErrorAction(result.ToString()) : RedirectToAction(nameof(Edit), new { id = userid });
        }

        /// <summary>
        /// Deletes the role.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <param name="role">The role.</param>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        public async Task<IActionResult> DeleteRole(string userid, string role)
        {
            if (!ModelState.IsValid)
            {
                return GetErrorAction(Messages.InvalidRole);
            }
            ApplicationUser current = await UserManager.FindByIdAsync(userid);
            if (current == null)
            {
                return GetErrorAction(Messages.InvalidRole);
            }

            IdentityResult result = await UserManager.RemoveFromRoleAsync(current, role);
            return !result.Succeeded ? GetErrorAction(result.ToString()) : RedirectToAction(nameof(Edit), new { id = userid });
        }

        #endregion Add / Delete Roles

        #region Add / Delete Claims

        /// <summary>
        /// Adds the claim.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <param name="model">The model.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        public async Task<IActionResult> AddClaim(string userid, UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return GetErrorAction(Messages.InvalidUser);
            }

            ApplicationUser current = await UserManager.FindByIdAsync(userid);
            if (current == null)
            {
                return GetErrorAction(Messages.InvalidUser);
            }

            IdentityResult result = await UserManager.AddClaimAsync(current, Mapper.Map<Claim>(model.NewClaim));
            return !result.Succeeded ? GetErrorAction(result.ToString()) : RedirectToAction(nameof(Edit), new { id = userid });
        }

        /// <summary>
        /// Deletes the claim.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <param name="claimtype">The claimtype.</param>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        public async Task<IActionResult> DeleteClaim(string userid, string claimtype)
        {
            if (!ModelState.IsValid)
            {
                return GetErrorAction(Messages.InvalidRole);
            }

            ApplicationUser current = await UserManager.FindByIdAsync(userid);
            if (current == null)
            {
                return GetErrorAction(Messages.InvalidRole);
            }

            System.Collections.Generic.IList<Claim> claims = await UserManager.GetClaimsAsync(current);
            Claim claim = claims.FirstOrDefault(c => c.Type == claimtype);

            IdentityResult result = await UserManager.RemoveClaimAsync(current, claim);
            return !result.Succeeded ? GetErrorAction(result.ToString()) : RedirectToAction(nameof(Edit), new { id = userid });
        }

        #endregion Add / Delete Roles
    }
}