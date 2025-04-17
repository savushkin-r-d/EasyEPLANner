using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner.ModbusExchange.Model
{
    public interface IExchange
    {
        IEnumerable<IGateway> Models { get; }

        IGateway SelectedModel { get; }

        void AddModel(string modelName);

        void SelectModel(string modelName);
    }
}
