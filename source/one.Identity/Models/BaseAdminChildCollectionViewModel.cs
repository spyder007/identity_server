using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace one.Identity.Models
{
    public abstract class BaseAdminChildCollectionViewModel<T> where T : BaseAdminChildItemViewModel, new()
    {
        protected BaseAdminChildCollectionViewModel()
        {
            ItemsList = new List<T>();
            NewItem = new T();
        }

        public abstract BaseAdminNavBar GetNavBar();

        public BaseAdminNavBar NavBar { get; set; }
        public List<T> ItemsList { get; set; }
        public T NewItem { get; set; }
        public int Id { get; private set; }

        public virtual void SetId(int id)
        {
            NavBar = GetNavBar();
            NavBar.SetActive(this);
            NavBar.Id = id;
            NewItem.ParentId = id;
            Id = id;
        }
    }

}
