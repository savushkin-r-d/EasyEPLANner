using BrightIdeasSoftware;
using EasyEPlanner.ProjectImportICP;
using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.DataModel;
using Eplan.EplApi.DataModel.EObjects;
using Eplan.EplApi.DataModel.MasterData;
using LuaInterface;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyEPlanner
{
    [ExcludeFromCodeCoverage]
    public class ImportIcpWagoProject : IEplAction
    {
        ~ImportIcpWagoProject() { }

        public static string GetMainWagoPluaData()
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Открытие main.wago.plua",
                Filter = "main.wago.plua|main.wago.plua",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == DialogResult.Cancel)
            {
                return string.Empty;
            }

            var data = "";
            using (var reader = new StreamReader(openFileDialog.FileName, EncodingDetector.DetectFileEncoding(openFileDialog.FileName), true))
            {
                // read main.wago.plua file data
                data = reader.ReadToEnd();
            }

            return data;
        }


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

                var data = GetMainWagoPluaData();
                if (string.IsNullOrEmpty(data))
                    return true;

                Logs.Show();
                Logs.DisableButtons();
                Logs.Clear();
                Logs.SetProgress(0);                

                var modulesImporter = new ModulesImporter(currentProject, data);
                modulesImporter.Import();

                var devicesImporter = new DevicesImporter(currentProject, data);
                devicesImporter.Import();

                new BindingImporter(modulesImporter.ImportModules, devicesImporter.ImportDevices).Bind();

                if (DialogResult.Yes == MessageBox.Show("Сохранить файл соответствия наименования устройств?",
                    "EPlanner", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    var openFileDialog = new SaveFileDialog()
                    {
                        Title = "Сохранение файла соответствия наименования устройств",
                        Filter = "CSV file (*.scv)|*.csv",
                    };

                    if (openFileDialog.ShowDialog() != DialogResult.Cancel)
                    {
                        DeviceNameMatchingSaver.Save(openFileDialog.FileName, devicesImporter.ImportDevices);
                    }
                }

                Logs.EnableButtons();
                Logs.ShowLastLine();
                Logs.SetProgress(100);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return true;
        }

        public bool OnRegister(ref string Name, ref int Ordinal)
        {
            Name = nameof(ImportIcpWagoProject);
            Ordinal = 30;

            return true;
        }

        public void GetActionProperties(ref ActionProperties actionProperties)
        {
        }
    }
}
