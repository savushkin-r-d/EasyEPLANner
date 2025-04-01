using IO.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO.ViewModel
{
    public class Property(string name, Func<string> getter, Action<string> setter = null) : IProperty, IViewItem, IEditable
    {
        public string Name => name;

        public string Description => getter.Invoke();

        public bool SetValue(string value)
        {
            if (setter is null)
                return false;

            setter?.Invoke(value);
            return true;
        }
    }
}
