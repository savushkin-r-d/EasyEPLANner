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
        private IIOManager manager;

        public IOViewModel(IIOManager manager) 
        {
            this.manager = manager;

            Root = new Root(this);
        }

        public IEnumerable<IRoot> Roots => [ Root ]; 

        public IRoot Root { get; private set; }

        public IIOManager IOManager => manager;

        public IEplanFunction SelectedClampFunction { get; set; }

        public IClamp SelectedClamp { get; set; }

        public void RebuildTree()
        {
            Root = new Root(this);
        }
    }
}
