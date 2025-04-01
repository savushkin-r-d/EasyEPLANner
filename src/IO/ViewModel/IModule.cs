using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO.ViewModel
{
    public interface IModule : IViewItem, IExpandable
    {
        IIOModule IOModule { get; }

        IIONode IONode { get; }

    }
}
