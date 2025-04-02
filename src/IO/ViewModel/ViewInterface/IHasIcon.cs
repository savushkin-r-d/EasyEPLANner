using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO.ViewModel
{
    public enum Icon
    {
        None = -1,

        Cab = 0,
        Node,

        BlackModule,
        GrayModule,
        GreenModule,
        LimeModule,
        OrangeModule,
        RedModule,
        VioletModule,
        YellowModule,
        
        Clamp,
        Cable,
    }


    public interface IHasIcon
    {
        Icon Icon { get; }
    }

    public interface IHasDescriptionIcon
    {
        Icon Icon { get; }
    }
}
