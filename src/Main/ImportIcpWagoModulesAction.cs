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
    public class ImportIcpWagoModulesAction : IEplAction
    {
        ~ImportIcpWagoModulesAction() { }

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

                var data = new StreamReader(openFileDialog.FileName, EncodingDetector.DetectFileEncoding(openFileDialog.FileName), true).ReadToEnd();

                // Fix main.wago.plua file: add "," to table before "group_dev_ex"
                data = Regex.Replace(data, @"}(?=(\r|\n|\r\n)\t*group_dev_ex)", "},", RegexOptions.None, TimeSpan.FromMilliseconds(100));
                
                // Remove unnecessary lua-module 'sys_wago'
                data = Regex.Replace(data, "require 'sys_wago'", "", RegexOptions.None, TimeSpan.FromMilliseconds(100));

                var modulesImporter = new ModulesImporter(currentProject, data);
                modulesImporter.Import();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return true;
        }

        public bool OnRegister(ref string Name, ref int Ordinal)
        {
            Name = "ImportIcpWagoModules";
            Ordinal = 30;

            return true;
        }

        public void GetActionProperties(ref ActionProperties actionProperties)
        {
        }
    }
}
