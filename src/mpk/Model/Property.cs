using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner.mpk.Model
{
    public enum PropertyModel
    {
        Local = 0,
        Global,
        Tag,
        Calculated,
        Interserver,
        OPC,
    }

    public enum PropertyType
    {
        String = 0,
        Integer,
        Float,
        Boolean,
        Date,
    }

    public class Property
    {
        public string Name { get; set; }

        public string Caption { get; set; }

        public bool Visible { get; set; }

        public bool Report {  get; set; }

        public bool Saved { get; set; }

        public string TagName { get; set; }

        public PropertyModel PropModel { get; set; }

        public PropertyType PropType { get; set; }

        public int Value { get; set; }

        public int ChannelId { get; set; }

        public int Priority { get; set; }
    }
}
