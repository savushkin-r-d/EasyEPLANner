using Aga.Controls.Tree;
using EplanDevice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace EasyEPlanner
{
    public abstract class ColumnNode(string name, string value) : Node(name)
    {
        protected string value = value;

        public virtual string Value
        {
            get => value;
            set => this.value = value;
        }

        public string Name => Text;

        public abstract void Update(IODevice device, string newValue, string oldValue);
    }

    public class DeviceSubTypeNode(string value) : ColumnNode("Подтип", value)
    {
        public override void Update(IODevice device, string newValue, string oldValue)
        {
            if (device.DeviceType.SubTypeNames().Contains(newValue))
            {
                device.Function.SubType = newValue;

                value = "Обновите проект";
            }
            else Value = oldValue;
        }
    }

    public class DeviceDescriptionNode(string value) : ColumnNode("Описание", value)
    {
        public override void Update(IODevice device, string newValue, string oldValue)
        {
            device.Function.Description = newValue;
        }
    }

    public class DeviceArticleNode(string value) : ColumnNode("Изделие", value)
    {
        public override void Update(IODevice device, string newValue, string oldValue)
        {
            Value = oldValue;
        }
    }

    public class ParameterNode(string name, string value) : ColumnNode(name, value)
    {
        public override void Update(IODevice device, string newValue, string oldValue)
        {
            if (double.TryParse(newValue, out double parValue))
            {
                var parameter = (IODevice.Parameter)Tag;
                device.SetParameter(parameter.Name, parValue);
                device.UpdateParameters();
            }
            else Value = oldValue;
        }
    }

    public class PropertyNode(string name, string value) : ColumnNode(name, value)
    {
        public override void Update(IODevice device, string newValue, string oldValue)
        {
            if (device.MultipleProperties().Contains(Name) &&
                newValue.Contains(","))
            {
                device.SetProperty(Name, newValue);
                device.UpdateProperties();
            }
            else Value = oldValue;
        }
    }

    public class RuntimeParameterNode(string name, string value) : ColumnNode(name, value)
    {
        public override void Update(IODevice device, string newValue, string oldValue)
        {
            if (int.TryParse(newValue, out int parValue))
            {
                device.SetRuntimeParameter(Name, parValue);
                device.UpdateRuntimeParameters();
            }
            else Value = oldValue;
        }
    }
}
