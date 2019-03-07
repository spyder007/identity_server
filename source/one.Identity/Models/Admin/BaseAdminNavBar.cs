namespace one.Identity.Models.Admin
{
    public abstract class BaseAdminNavBar
    {
        public int Id { get; set; }

        public abstract void SetActive(object model);
    }
}
