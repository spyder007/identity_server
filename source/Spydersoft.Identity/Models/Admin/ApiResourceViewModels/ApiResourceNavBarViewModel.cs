namespace Spydersoft.Identity.Models.Admin.ApiResourceViewModels
{
    /// <summary>
    /// Class ApiResourceNavBarViewModel.
    /// Implements the <see cref="Admin.BaseAdminNavBar{ApiResourceViewModels.ApiResourceViewModel}" />
    /// </summary>
    /// <seealso cref="Admin.BaseAdminNavBar{ApiResourceViewModels.ApiResourceViewModel}" />
    public class ApiResourceNavBarViewModel(ApiResourceViewModel parent) : BaseAdminNavBar<ApiResourceViewModel>(parent)
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public override string Name => Parent.Name;
    }
}