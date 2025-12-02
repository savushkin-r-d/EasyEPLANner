using EasyEPlanner.ModbusExchange.View;
using EasyEPlanner.mpk.ModelBuilder;
using EasyEPlanner.mpk.Saver;
using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.DataModel;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TechObject;

namespace EasyEPlanner.Main
{
    [ExcludeFromCodeCoverage]
    internal class SaveAsMpkx : IEplAction
    {
        ~SaveAsMpkx()
        {
        }

        public bool Execute(ActionCallingContext oActionCallingContext)
        {
            try
            {
                Project currentProject = EProjectManager.GetInstance().
                    GetCurrentPrj();
                if (currentProject == null)
                {
                    MessageBox.Show("Нет открытого проекта!", "EPlaner",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                using var folderDialog = new FolderBrowserDialog();
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFolderPath = folderDialog.SelectedPath;

                    var container = new TechObjectMpkBuilder(TechObjectManager.GetInstance(), currentProject.ProjectName).Build();
                    var exchange = new MpkxSaver(container);
                    exchange.Save(selectedFolderPath);
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
            Name = nameof(SaveAsMpkx);
            Ordinal = 30;

            return true;
        }

        public void GetActionProperties(ref ActionProperties actionProperties)
        {
        }
    }
}
