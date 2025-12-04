using EasyEPlanner.ModbusExchange.View;
using EasyEPlanner.mpk.ModelBuilder;
using EasyEPlanner.mpk.Saver;
using EasyEPlanner.mpk.View;
using EasyEPlanner.mpk.ViewModel;
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

                var saverDialog = new MpkSaverForm(new MpkSaverContext(currentProject.ProjectName));
                if (saverDialog.ShowDialog() != DialogResult.Cancel)
                {
                    new MpkxSaver().Save(saverDialog.Context);
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
