namespace Spydersoft.Identity.Models.Admin
{
    public abstract class BaseAdminNavBar<TMainViewModel> where TMainViewModel : BaseAdminViewModel
    {
        protected BaseAdminNavBar(TMainViewModel parent)
        {
            Parent = parent;
        }

        public int Id { get; set; }
        protected TMainViewModel Parent { get; }

        public abstract string Name { get; }
    }
}