using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aga.Controls.Tree;

namespace EasyEPlanner
{
    public class ColumnNode : Node
    {
        public string Value = "";
        public ColumnNode(string Text, string Value)
            : base(Text) 
        {
            this.Value = Value;
        }
    }
}
