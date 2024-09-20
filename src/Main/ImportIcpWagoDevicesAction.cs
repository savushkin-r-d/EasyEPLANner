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

                if (openWagoFileDialog.ShowDialog() == DialogResult.Cancel)
                {
                    return true;
                }

                var wagoData = "";
                using (var reader = new StreamReader(openWagoFileDialog.FileName, EncodingDetector.DetectFileEncoding(openWagoFileDialog.FileName), true))
                {
                    // read main.wago.plua file data
                    wagoData = reader.ReadToEnd();
                }

                var devicesImporter = new DevicesImporter(currentProject, wagoData);
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
