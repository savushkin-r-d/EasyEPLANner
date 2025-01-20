using CsvHelper.Configuration;
using CsvHelper;
using EasyEPlanner.ProjectImportICP;
using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.DataModel;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EplanDevice;
using StaticHelper;

namespace EasyEPlanner
{
    [ExcludeFromCodeCoverage]
    public class ImportIcpCreateDeviceNameMathchingFile : IEplAction
    {
        ~ImportIcpCreateDeviceNameMathchingFile() { }

        public bool Execute(ActionCallingContext oActionCallingContext)
        {
            try
            {
                Project currentProject = EProjectManager.GetInstance()
                    .GetCurrentPrj();
                if (currentProject == null)
                {
                    MessageBox.Show("Нет открытого проекта!", "EPlaner",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return true;
                }

                var openFileDialog = new SaveFileDialog()
                {
                    Title = "Сохранение файла соответствия наименований устройств",
                    Filter = "CSV file (*.scv)|*.csv",
                };

                if (openFileDialog.ShowDialog() != DialogResult.Cancel)
                {
                    DeviceNameMatchingSaver.Save(openFileDialog.FileName, DeviceManager.GetInstance().Devices);
                }
            }
            
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return true;
        }

        public bool OnRegister(ref string Name, ref int Ordinal)
        {
            Name = nameof(ImportIcpCreateDeviceNameMathchingFile);
            Ordinal = 30;

            return true;
        }

        public void GetActionProperties(ref ActionProperties actionProperties)
        {
        }
    }
}