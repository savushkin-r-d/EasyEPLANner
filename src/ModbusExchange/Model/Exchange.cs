using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        public void ImportCSV()
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "CSV структура шлюза",
                Filter = "CSV|*.csv",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() is DialogResult.Cancel)
            {
                return;
            }

            using StreamReader reader = new(openFileDialog.FileName, EncodingDetector.DetectFileEncoding(openFileDialog.FileName), true);
            
            var csvData = reader.ReadToEnd();

            CSVImporter.Import(SelectedModel.Read, SelectedModel.Write, csvData);
        }

        public void SelectModel(string modelName)
        {
            SelectedModel = models.Find(m => m.Name == modelName);
        }
    }
}
