using BrightIdeasSoftware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO.ViewModel
{
    public class Node : INode, IExpandable, IHasIcon
    {
        private readonly List<IModule> modules = [];

        private readonly List<IViewItem> items = [];

        public Node(IIONode node, ILocation owner)
        {
            IONode = node;

            if (node.Type is IO.IONode.TYPES.T_EMPTY)
                return;

            var ip = new Property("IP-адрес",
                () => IONode.Function.IP,
                ip => IONode.Function.IP = ip);

            var SubnetMask = new Property("Маска подсети",
                () => IONode.Function.SubnetMask,
                mask => IONode.Function.SubnetMask = mask);

            var Gateway = new Property("Сетевой шлюз", 
                () => IONode.Function.Gateway, 
                gw => IONode.Function.Gateway = gw);

            modules.AddRange(node.IOModules.Select(m => new Module(m, this)));
            
            items.Add(ip);
            items.Add(SubnetMask);
            items.Add(Gateway);
            items.AddRange(modules);
        }

        public IIONode IONode { get; private set; }

        public IEnumerable<IViewItem> Items => items;

        public string Name => IONode.Type is IO.IONode.TYPES.T_EMPTY ? $"{IONode.N}. Заглушка" : $"{IONode.N}. {IONode.Name}";

        public string Description => IONode.TypeStr;

        public bool Expanded 
        {
            get => IONode.Function?.Expanded ?? false;
            set => IONode.Function.Expanded = value;
        }

        Icon IHasIcon.Icon => Icon.Node;
    }
}
