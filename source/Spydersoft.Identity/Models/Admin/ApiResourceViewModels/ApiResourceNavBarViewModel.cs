namespace Spydersoft.Identity.Models.Admin.ApiResourceViewModels
{
    /// <summary>
    /// Class ApiResourceNavBarViewModel.
    /// Implements the <see cref="BaseAdminNavBar{TMainViewModel}" />
    /// </summary>
    /// <seealso cref="BaseAdminNavBar{TMainViewModel}" />
    public class ApiResourceNavBarViewModel(ApiResourceViewModel parent) : BaseAdminNavBar<ApiResourceViewModel>(parent)
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public override string Name => Parent.Name;
    }
}