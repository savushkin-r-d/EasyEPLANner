using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner.ModbusExchange.Model
{
    public class Exchange : IExchange
    {
        readonly List<IGateway> models = [];

        public IEnumerable<IGateway> Models => models;

        public IGateway SelectedModel {get; private set;}

        public void AddModel(string modelName)
        {
            var model = new Gateway(modelName);
            models.Add(model);
            SelectedModel = model;
        }

        public void SelectModel(string modelName)
        {
            SelectedModel = models.Find(m => m.Name == modelName);
        }
    }
}
