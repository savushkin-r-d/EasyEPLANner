using EasyEPlanner.mpk.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace EasyEPlanner.mpk.Saver
{
    public class XmlSaver
    {
        private Container container;

        public XmlSaver(Container container) 
        {
            this.container = container;
        }

        public void Save()
        {
            
        }


        public void SerializeComponent()
        {

        }
    }
}
