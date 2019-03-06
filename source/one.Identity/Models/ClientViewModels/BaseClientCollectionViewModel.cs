using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace one.Identity.Models.ClientViewModels
{
    public class BaseClientCollectionViewModel<T> where T : BaseViewModel, IClientCollectionViewModel, new()
    {
        public BaseClientCollectionViewModel()
        {
            ItemsList = new List<T>();
            NavBar = new NavBarViewModel();
            NavBar.SetActive(this);
            NewItem = new T();
        }

        public NavBarViewModel NavBar { get; set; }
        public List<T> ItemsList { get; set; }
        public T NewItem { get; set; }
        public int Id { get; private set; }

        public virtual void SetId(int id)
        {
            NavBar.Id = id;
            NewItem.ClientId = id;
            Id = id;
        }
    }
}
