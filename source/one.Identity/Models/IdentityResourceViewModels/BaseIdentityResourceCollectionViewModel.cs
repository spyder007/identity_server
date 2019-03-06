using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using one.Identity.Models.ClientViewModels;

namespace one.Identity.Models.IdentityResourceViewModels
{
    public class BaseIdentityResourceCollectionViewModel<T> where T : BaseViewModel, IIdentityResourceCollectionViewModel, new()
    {
        public BaseIdentityResourceCollectionViewModel()
        {
            ItemsList = new List<T>();
            NavBar = new IdentityResourceNavBarViewModel();
            NavBar.SetActive(this);
            NewItem = new T();
        }

        public IdentityResourceNavBarViewModel NavBar { get; set; }
        public List<T> ItemsList { get; set; }
        public T NewItem { get; set; }
        public int Id { get; private set; }

        public virtual void SetId(int id)
        {
            NavBar.Id = id;
            NewItem.IdentityResourceId = id;
            Id = id;
        }
    }
}
