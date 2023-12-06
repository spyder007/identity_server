using System.Collections.Generic;

using Duende.IdentityServer.EntityFramework.DbContexts;

namespace Spydersoft.Identity.Models.Admin
{
    public abstract class BaseAdminChildCollectionViewModel<TChildItemViewModel, TParentItemViewModel>
        where TChildItemViewModel : BaseAdminChildItemViewModel, new()
        where TParentItemViewModel : BaseAdminViewModel, new()
    {
        protected BaseAdminChildCollectionViewModel()
        {
            ItemsList = new List<TChildItemViewModel>();
            NewItem = new TChildItemViewModel();
        }

        public abstract BaseAdminNavBar<TParentItemViewModel> GetNavBar(TParentItemViewModel parent);

        public virtual TChildItemViewModel GetChild(TParentItemViewModel parent, ConfigurationDbContext configDbContext)
        {
            var child = new TChildItemViewModel();
            NewItem.ParentId = parent.Id;
            return child;
        }

        public BaseAdminNavBar<TParentItemViewModel> NavBar { get; set; }
        public List<TChildItemViewModel> ItemsList { get; set; }
        public TChildItemViewModel NewItem { get; set; }
        public int Id { get; private set; }

        public virtual void SetMainViewModel(TParentItemViewModel parentViewModel, ConfigurationDbContext configDbContext)
        {
            NavBar = GetNavBar(parentViewModel);
            NavBar.Id = parentViewModel.Id;
            NewItem = GetChild(parentViewModel, configDbContext);
            Id = parentViewModel.Id;
        }
    }

}