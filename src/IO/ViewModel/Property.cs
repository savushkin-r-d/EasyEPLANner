using IO.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO.ViewModel
{
    /// <summary>
    /// Свойство.Привязывются делегаты получения и установки свойства (на ФСА).
    /// </summary>
    /// <param name="name">Название свойства</param>
    /// <param name="getter">Делегат получения значения свойства</param>
    /// <param name="setter">Делегат установки значения свойства</param>
    public class Property(string name, Func<string> getter, Action<string> setter = null) : IProperty, IEditable
    {
        public string Name => name;

        public string Description => getter.Invoke();

        public string Value => Description;

        public bool SetValue(string value)
        {
            if (setter is null)
                return false;

            setter?.Invoke(value);
            return true;
        }
    }
}
