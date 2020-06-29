using System.Text;
using System.IO;
using EasyEPlanner;

namespace InterprojectExchange
{
    /// <summary>
    /// Класс сохраняющий в LUA обмен между контроллерами
    /// </summary>
    public class InterprojectExchangeSaver
    {
        public InterprojectExchangeSaver(InterprojectExchange owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Сохранить межконтроллерный обмен
        /// </summary>
        public void Save()
        {
            WriteCurrentProject();
            //TODO: Запись остальных проектов
        }

        /// <summary>
        /// Запись текущего проекта
        /// </summary>
        private void WriteCurrentProject()
        {
            string res = SaveMainProject();
            res = res.Replace("\t", "    ");

            string path = Path.Combine(ProjectManager.GetInstance()
                .GetPtusaProjectsPath(""), owner.CurrentProjectName,
                "shared.lua");
            var writer = new StreamWriter(path, false,
                Encoding.GetEncoding(1251));
            writer.WriteLine(res);
            writer.Flush();
            writer.Close();
        }

        /// <summary>
        /// Генерация файла для текущего проекта
        /// </summary>
        /// <returns></returns>
        private string SaveMainProject()
        {
            string res = "";

            var mainModel = owner.GetModel(owner.CurrentProjectName);
            string remoteGateWays = "";
            string sharedDevices = "";
            foreach (var model in owner.Models)
            {
                if (model.ProjectName != mainModel.ProjectName)
                {
                    remoteGateWays += SaveMainProjectRemoteGateWays(mainModel,
                        model.ProjectName, model.PacInfo, 
                        model.ReceiverSignals);
                    sharedDevices += SaveMainProjectSharedDevices(mainModel,
                        model.ProjectName, model.PacInfo.Station, 
                        model.SourceSignals);
                }
            }

            string remoteGatewaysTemplate = "remote_gateways =\n{\nVALUE}\n";
            string sharedDevicesTemplate = "shared_devices =\n{\nVALUE}";

            res += remoteGatewaysTemplate.Replace("VALUE", remoteGateWays);
            res += sharedDevicesTemplate.Replace("VALUE", sharedDevices);

            return res;
        }

        /// <summary>
        /// Генерация remote_gateways по текущему проекту
        /// </summary>
        /// <param name="mainModel">Главная модель текущего проекта</param>
        /// <param name="pacInfo">Данные о ПЛК сохраняемого проекта</param>
        /// <param name="projectName">Имя сохраняемого проекта</param>
        /// <returns></returns>
        private string SaveMainProjectRemoteGateWays(IProjectModel mainModel,
            string projectName, PacDTO pacInfo, DeviceSignalsDTO signals)
        {
            var res = "";

            var model = mainModel as CurrentProjectModel;
            model.SelectedAdvancedProject = projectName;
            if (signals.Count <= 0)
            {
                return "";
            }

            const string prefix = "\t\t";
            res += $"\t[\'{projectName}\'] =\n\t{{\n";
            res += prefix + $"ip = \'{pacInfo.IP}\',\n";
            res += prefix + $"ipemulator = \'{pacInfo.IPEmulator}\',\n";
            res += prefix + $"emulation = " +
                $"{pacInfo.EmulationEnabled.ToString().ToLower()},\n";
            res += prefix + $"cycletime = {pacInfo.CycleTime},\n";
            res += prefix + $"timeout = {pacInfo.TimeOut},\n";
            res += prefix + $"port = {pacInfo.Port},\n";
            res += prefix + $"enabled = " +
                $"{pacInfo.GateEnabled.ToString().ToLower()},\n";
            res += prefix + $"station = {pacInfo.Station},\n";
            
            res += SaveSignals(signals, prefix);

            res += "\t},\n\n";
            return res;
        }

        /// <summary>
        /// Генерация shared_devices по текущему проекту
        /// </summary>
        /// <param name="mainModel">Главная модель проекта</param>
        /// <param name="projectName">Имя сохраняемого проекта</param>
        /// <param name="stationNum">Номер станции PAC</param>
        /// <returns></returns>
        private string SaveMainProjectSharedDevices(IProjectModel mainModel,
            string projectName, int stationNum, DeviceSignalsDTO signals)
        {
            var res = "";

            var model = mainModel as CurrentProjectModel;
            model.SelectedAdvancedProject = projectName;
            if (signals.Count <= 0)
            {
                return "";
            }

            const string prefix = "\t\t";
            res += $"\t[{stationNum}] =\n\t{{\n";
            res += prefix + $"projectName = \"{projectName}\",\n";
            
            res += SaveSignals(signals, prefix);
            
            res += "\t},\n\n";
            return res;
        }

        /// <summary>
        /// Сохранение сигналов в нужном виде
        /// </summary>
        /// <param name="signals">Модель сигналов</param>
        /// <param name="prefix">Префикс</param>
        /// <returns></returns>
        private string SaveSignals(DeviceSignalsDTO signals, string prefix)
        {
            var res = "";

            string digIn = "";
            foreach (var signalDI in signals.DI)
            {
                digIn += prefix + $"__{signalDI},\n";
            }
            if (digIn.Length > 0)
            {
                res += "\t" + $"DI =\n{prefix}{{\n{digIn}{prefix}}},\n";
            }

            string digOut = "";
            foreach (var signalDO in signals.DO)
            {
                digOut += prefix + $"{signalDO},\n";
            }
            if (digOut.Length > 0)
            {
                res += "\t" + $"DO =\n{prefix}{{\n{digOut}{prefix}}},\n";
            }

            string analogIn = "";
            foreach (var signalAI in signals.AI)
            {
                analogIn += prefix + $"__{signalAI},\n";
            }
            if (analogIn.Length > 0)
            {
                res += "\t" + $"AI =\n{prefix}{{\n{analogIn}{prefix}}},\n";
            }

            string analogOut = "";
            foreach (var signalAO in signals.AO)
            {
                analogOut += prefix + $"{signalAO},\n";
            }
            if (analogOut.Length > 0)
            {
                res += "\t" + $"AO =\n{prefix}{{\n{analogOut}{prefix}}},\n";
            }

            return res;
        }

        private InterprojectExchange owner;
    }
}
