using System.Linq;

namespace Spydersoft.Identity.Core.Models.Identity
{
    /// <summary>
    /// Class UserViewModel.
    /// </summary>
    public class UserViewModel
    {
        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>The user.</value>
        public ApplicationUser User { get; set; } = new();
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        public string Password { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is new user.
        /// </summary>
        /// <value><c>true</c> if this instance is new user; otherwise, <c>false</c>.</value>
        public bool IsNewUser { get; set; } = false;

        /// <summary>
        /// Gets or sets the roles.
        /// </summary>
        /// <value>The roles.</value>
        public IQueryable<string> Roles { get; set; } = Enumerable.Empty<string>().AsQueryable();
        /// <summary>
        /// Gets or sets the available roles.
        /// </summary>
        /// <value>The available roles.</value>
        public IQueryable<string> AvailableRoles { get; set; } = Enumerable.Empty<string>().AsQueryable();
        /// <summary>
        /// Gets or sets the selected available role.
        /// </summary>
        /// <value>The selected available role.</value>
        public string SelectedAvailableRole { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the claims.
        /// </summary>
        /// <value>The claims.</value>
        public IQueryable<ClaimModel> Claims { get; set; } = Enumerable.Empty<ClaimModel>().AsQueryable();
        /// <summary>
        /// Creates new claim.
        /// </summary>
        /// <value>The new claim.</value>
        public ClaimModel NewClaim { get; set; } = new();
    }
}
