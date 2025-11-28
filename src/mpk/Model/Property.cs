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

    public class Property : IProperty
    {
        public Property() { }

        public Property(string name, PropertyModel model, PropertyType type)
        {
            Name = name;
            Caption = name;
            PropModel = model;
            PropType = type;
        }

        public string Name { get; set; }

        public string Caption { get; set; }

        public bool Visible { get; set; } = true;

        public bool Report { get; set; } = false;

        public bool Saved { get; set; } = false;

        public string TagName { get; set; } = "";

        public PropertyModel PropModel { get; set; }

        public PropertyType PropType { get; set; }

        public object Value { get; set; } = 0;

        public int ChannelId { get; set; } = 0;

        public int Priority { get; set; } = 5;
    }
}
