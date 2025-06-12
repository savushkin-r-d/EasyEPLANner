using Aga.Controls.Tree;
using EplanDevice;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EasyEPlanner
{
    public abstract class ColumnNode(string name, string value) : Node(name)
    {
        public virtual string Value { get; set; } = value;

        public string Name => Text;

        public abstract void Update(IODevice device, string newValue, string oldValue);
    }
}
