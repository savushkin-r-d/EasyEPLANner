using StaticHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO.ViewModel
{
    public interface IClamp : IViewItem
    {
        IEplanFunction ClampFunction { get; }

        IIONode Node { get; }

        IIOModule Module { get; }

        void Reset();
    }
}
