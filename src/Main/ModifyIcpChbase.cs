using EasyEPlanner.ProjectImportICP;
using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.DataModel;
using EplanDevice;
using StaticHelper;
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
    public class ModifyIcpChbase : IEplAction
    {
        ~ModifyIcpChbase() { }

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

                var openNewChbase = new OpenFileDialog
                {
                    Title = "Открытие новой базы каналов",
                    Filter = "*.cbdx|*.cdbx",
                    Multiselect = false
                };

                if (openNewChbase.ShowDialog() == DialogResult.Cancel)
                    return true;

                var openOldChbase = new OpenFileDialog
                {
                    Title = "Открытие старой базы каналов",
                    Filter = "*.cbdx|*.cdbx",
                    Multiselect = false
                };

                if (openOldChbase.ShowDialog() == DialogResult.Cancel)
                    return true;

                var newChbase = "";
                using (var reader = new StreamReader(openNewChbase.FileName, EncodingDetector.DetectFileEncoding(openNewChbase.FileName), true))
                {
                    newChbase = reader.ReadToEnd();
                }

                var oldChbase = "";
                using (var reader = new StreamReader(openOldChbase.FileName, EncodingDetector.DetectFileEncoding(openOldChbase.FileName), true))
                {
                    oldChbase = reader.ReadToEnd();
                }

                Logs.Clear();
                Logs.Show();

                var apiHelper = new ApiHelper();
                var devs = DeviceManager.GetInstance().Devices.Select(d =>
                {
                    var wagoName = apiHelper.GetSupplementaryFieldValue(d.EplanObjectFunction, 10);

                    if (wagoName == string.Empty)
                    {
                        wagoName = ChannelBaseTransformer.ToWagoDevice(d);
                        if (wagoName != string.Empty)
                            Logs.AddMessage($"Старое название тега для устройства {d.Name} не указано, используется название: {wagoName} \n");
                    }
                        

                    return (d.Name, wagoName);
                });

                var res = new ChannelBaseTransformer().TransformID(newChbase, oldChbase, devs);
                using (var writer = new StreamWriter(openNewChbase.FileName + "_reindex", false, Encoding.UTF8))
                {
                    writer.Write(res);
                }

                Logs.SetProgress(100);
                Logs.EnableButtons();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return true;
        }

        public bool OnRegister(ref string Name, ref int Ordinal)
        {
            Name = nameof(ModifyIcpChbase);
            Ordinal = 30;

            return true;
        }

        public void GetActionProperties(ref ActionProperties actionProperties)
        {
        }
    }
}
