using EasyEPlanner.ProjectImportICP;
using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.DataModel;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyEPlanner.Main
{
    /// <summary>
    /// Действие "Импорт устройств ICP-CON проекта"
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ImportIcpWagoDevicesAction : IEplAction
    {
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

                var openWagoFileDialog = new OpenFileDialog
                {
                    Title = "Открытие main.wago.plua",
                    Filter = "main.wago.plua|main.wago.plua",
                    Multiselect = false
                };

                var openIcpProjectFileDialog = new OpenFileDialog
                {
                    Title = "Открытие файла проекта ds4",
                    Filter = ".ds4|*.ds4",
                    Multiselect = false
                };

                if (openWagoFileDialog.ShowDialog() == DialogResult.Cancel ||
                    openIcpProjectFileDialog.ShowDialog() == DialogResult.Cancel)
                {
                    return true;
                }

                // read main.wago.plua file data
                var wagoData = new StreamReader(openWagoFileDialog.FileName, EncodingDetector.DetectFileEncoding(openWagoFileDialog.FileName), true).ReadToEnd();
                // Fix main.wago.plua '}'
                wagoData = Regex.Replace(wagoData, @"}(?=(\r|\n|\r\n)\t+group_dev_ex)", "},", RegexOptions.None, TimeSpan.FromMilliseconds(100));
                // Remove unnecessary lua-module
                wagoData = Regex.Replace(wagoData, "require 'sys_wago'", "", RegexOptions.None, TimeSpan.FromMilliseconds(100));

                // read *.ds4 file data
                var icpProjectData = new StreamReader(openIcpProjectFileDialog.FileName, EncodingDetector.DetectFileEncoding(openIcpProjectFileDialog.FileName), true).ReadToEnd();

                var devicesImporter = new DevicesImporter(currentProject, wagoData, icpProjectData);
                devicesImporter.Import();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return true;
        }


        public bool OnRegister(ref string Name, ref int Ordinal)
        {
            Name = "ImportIcpWagoDevices";
            Ordinal = 30;

            return true;
        }

        public void GetActionProperties(ref ActionProperties actionProperties)
        {
        }
    }
}
