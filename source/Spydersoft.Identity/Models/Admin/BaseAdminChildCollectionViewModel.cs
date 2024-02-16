using System.Collections.Generic;

using Duende.IdentityServer.EntityFramework.DbContexts;

namespace Spydersoft.Identity.Models.Admin
{
    /// <summary>
    /// Class BaseAdminChildCollectionViewModel.
    /// </summary>
    /// <typeparam name="TChildItemViewModel">The type of the t child item view model.</typeparam>
    /// <typeparam name="TParentItemViewModel">The type of the t parent item view model.</typeparam>
    public abstract class BaseAdminChildCollectionViewModel<TChildItemViewModel, TParentItemViewModel>
        where TChildItemViewModel : BaseAdminChildItemViewModel, new()
        where TParentItemViewModel : BaseAdminViewModel, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseAdminChildCollectionViewModel{TChildItemViewModel, TParentItemViewModel}"/> class.
        /// </summary>
        protected BaseAdminChildCollectionViewModel()
        {
            ItemsList = [];
            NewItem = new TChildItemViewModel();
        }

        /// <summary>
        /// Gets the nav bar.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <returns>Spydersoft.Identity.Models.Admin.BaseAdminNavBar&lt;TParentItemViewModel&gt;.</returns>
        public abstract BaseAdminNavBar<TParentItemViewModel> GetNavBar(TParentItemViewModel parent);

        /// <summary>
        /// Gets the child.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="configDbContext">The configuration database context.</param>
        /// <returns>TChildItemViewModel.</returns>
        public virtual TChildItemViewModel GetChild(TParentItemViewModel parent, ConfigurationDbContext configDbContext)
        {
            var child = new TChildItemViewModel();
            NewItem.ParentId = parent.Id;
            return child;
        }

        /// <summary>
        /// Gets or sets the nav bar.
        /// </summary>
        /// <value>The nav bar.</value>
        public BaseAdminNavBar<TParentItemViewModel> NavBar { get; set; }
        /// <summary>
        /// Gets or sets the items list.
        /// </summary>
        /// <value>The items list.</value>
        public List<TChildItemViewModel> ItemsList { get; set; }
        /// <summary>
        /// Creates new item.
        /// </summary>
        /// <value>The new item.</value>
        public TChildItemViewModel NewItem { get; set; }
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public int Id { get; private set; }

        /// <summary>
        /// Sets the main view model.
        /// </summary>
        /// <param name="parentViewModel">The parent view model.</param>
        /// <param name="configDbContext">The configuration database context.</param>
        public virtual void SetMainViewModel(TParentItemViewModel parentViewModel, ConfigurationDbContext configDbContext)
        {
            NavBar = GetNavBar(parentViewModel);
            NavBar.Id = parentViewModel.Id;
            NewItem = GetChild(parentViewModel, configDbContext);
            Id = parentViewModel.Id;
        }
    }

}