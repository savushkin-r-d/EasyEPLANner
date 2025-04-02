using IO.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO.ViewModel
{
    public interface IEditable : IViewItem
    {
        bool SetValue(string value);
    }
}
