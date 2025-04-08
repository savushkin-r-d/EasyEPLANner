using StaticHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO.ViewModel
{
    public class IOViewModel : IIOViewModel
    {
        public IOViewModel(IIOManager manager) 
        {
            IOManager = manager;

            Root = new Root(this);
        }

        public IEnumerable<IRoot> Roots => [ Root ]; 

        public IRoot Root { get; private set; }

        public IIOManager IOManager {get; private set;}

        public IEplanFunction SelectedClampFunction { get; set; }

        public IClamp SelectedClamp { get; set; }

        public void RebuildTree()
        {
            Root = new Root(this);
        }
    }
}
