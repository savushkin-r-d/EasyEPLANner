using IO.ViewModel;
using IO.ViewModel.ViewInterface;
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

        public virtual bool SetValue(string value)
        {
            if (setter is null)
                return false;

            setter.Invoke(value);
            return true;
        }
    }

    /// <summary>
    /// Свойство с выбором значения из списка.
    /// </summary>
    public class ComboBoxProperty : Property, IComboBoxEditable
    {
        public ComboBoxProperty(string name, Func<string> getter,
            Action<string> setter, IEnumerable<string> items)
            : base(name, getter, setter)
        {
            ComboBoxItems = items;
        }

        public IEnumerable<string> ComboBoxItems { get; }

        public override bool SetValue(string value)
        {
            if (!ComboBoxItems.Contains(value))
                return false;

            return base.SetValue(value);
        }
    }
}
