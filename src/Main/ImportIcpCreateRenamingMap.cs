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

namespace EasyEPlanner
{
    [ExcludeFromCodeCoverage]
    public class ImportIcpCreateRenamingMap : IEplAction
    {
        ~ImportIcpCreateRenamingMap() { }

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

                var openFileDialog = new OpenFileDialog
                {
                    Title = "Открытие main.wago.plua",
                    Filter = "main.wago.plua|main.wago.plua",
                    Multiselect = false
                };

                if (openFileDialog.ShowDialog() == DialogResult.Cancel)
                {
                    return true;
                }

                var data = "";
                using (var reader = new StreamReader(openFileDialog.FileName, EncodingDetector.DetectFileEncoding(openFileDialog.FileName), true))
                {
                    // read main.wago.plua file data
                    data = reader.ReadToEnd();
                }

                var devicesImporter = new DevicesImporter(currentProject, data, false);
                devicesImporter.Import();

                var devices = devicesImporter.ImportDevices;

                var outputFileDialog = new SaveFileDialog
                {
                    Title = "Сохранение карты переименования устройств",
                    Filter = "*.txt|*.txt",
                    FileName = $"{currentProject.ProjectName}_devs_renaming",
                    DefaultExt = "txt",
                };

                if (outputFileDialog.ShowDialog() == DialogResult.Cancel)
                {
                    return true;
                }

                using (var writer = new StreamWriter(outputFileDialog.FileName, false))
                {
                    foreach (var dev in devices)
                    {
                        writer.Write($"{dev.WagoType + dev.FullNumber,10} => {dev.Object,10} | {dev.Type, 3} | {dev.Number}\n");
                    }
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
            Name = nameof(ImportIcpCreateRenamingMap);
            Ordinal = 30;

            return true;
        }

        public void GetActionProperties(ref ActionProperties actionProperties)
        {
        }
    }
}
