using System.ComponentModel;
using System.Linq;


namespace Spydersoft.Identity.Models
{
    public abstract class BaseViewModel
    {
        protected BaseViewModel()
        {
            // apply any DefaultValueAttribute settings to their properties
            System.Reflection.PropertyInfo[] propertyInfos = GetType().GetProperties();
            foreach (System.Reflection.PropertyInfo propertyInfo in propertyInfos)
            {
                var attributes = propertyInfo.GetCustomAttributes(typeof(DefaultValueAttribute), true);
                if (attributes.Any())
                {
                    var attribute = (DefaultValueAttribute)attributes[0];
                    propertyInfo.SetValue(this, attribute.Value, null);
                }
            }
        }
    }
}