using System.Text;
using System.IO;
using EasyEPlanner;
using System.Threading.Tasks;

namespace InterprojectExchange
{
    /// <summary>
    /// Класс сохраняющий в LUA обмен между контроллерами
    /// </summary>
    public class InterprojectExchangeSaver
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="owner">Класс-владелец</param>
        /// <param name="fileWithSignals">Имя файла, межконтроллерный обмен
        /// </param>
        public InterprojectExchangeSaver(InterprojectExchange owner, 
            string fileWithSignals)
        {
            this.owner = owner;
            SharedFile = fileWithSignals;
        }

        /// <summary>
        /// Сохранить межконтроллерный обмен
        /// </summary>
        public async void Save()
        {
            await Task.Run(() => WriteMainProject());
            WriteAdvancedProjects();
        }

        /// <summary>
        /// Запись текущего проекта
        /// </summary>
        private void WriteMainProject()
        {
            string res = SaveMainProject();
            res = res.Replace("\t", "    ");

            string path = Path.Combine(ProjectManager.GetInstance()
                .GetPtusaProjectsPath(""), owner.CurrentProjectName,
                SharedFile);
            using (var writer = new StreamWriter(path, false,
                Encoding.GetEncoding(1251)))
            {
                writer.WriteLine(res);
            }
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
                bool validModel = model.ProjectName != mainModel.ProjectName &&
                    model.MarkedForDelete == false;
                if (validModel)
                {
                    var currentModel = mainModel as CurrentProjectModel;
                    string projectName = model.ProjectName;
                    // SelectedAdvancedProject - с каким проектом работаем,
                    // влияет на список сигналов с currentModel
                    currentModel.SelectedAdvancedProject = projectName;

                    remoteGateWays += SaveMainProjectRemoteGateWays(projectName,
                        model.PacInfo, currentModel.ReceiverSignals);
                    sharedDevices += SaveMainProjectSharedDevices(projectName, 
                        model.PacInfo.Station, currentModel.SourceSignals);
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
        /// <param name="pacInfo">Данные о ПЛК сохраняемого проекта</param>
        /// <param name="projectName">Имя сохраняемого проекта</param>
        /// <returns></returns>
        private string SaveMainProjectRemoteGateWays(string projectName, 
            PacDTO pacInfo, DeviceSignalsDTO signals)
        {
            var res = "";
            if (signals.Count <= 0)
            {
                return "";
            }

            const string prefix = "\t\t";
            res += SavePACInfo(pacInfo, projectName, prefix);
            res += SaveSignals(signals, prefix);
            res += "\t},\n\n";

            return res;
        }

        /// <summary>
        /// Генерация shared_devices по текущему проекту
        /// </summary>
        /// <param name="projectName">Имя сохраняемого проекта</param>
        /// <param name="stationNum">Номер станции PAC</param>
        /// <returns></returns>
        private string SaveMainProjectSharedDevices(string projectName, 
            int stationNum, DeviceSignalsDTO signals)
        {
            var res = "";
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
        /// Сохранение информации о PAC для remote_gateways
        /// </summary>
        /// <param name="pacInfo"></param>
        /// <param name="projectName"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        private string SavePACInfo(PacDTO pacInfo, string projectName, 
            string prefix)
        {
            var res = "";

            string ipComment = "-- адрес удаленного контроллера";
            string ipEmulatorComment = "-- адрес удаленного контроллера при " +
                "эмуляции на столе";
            string emulationComment = "-- включение эмуляции";
            string cycleTimeComment = "-- время ожидания между опросами " +
                "контроллера";
            string timeoutComment = "-- таймаут для modbus клиента";
            string portComment = "-- modbus - порт удаленного контроллера";
            string enabledComment = "-- включить/выключить шлюз";
            string stationComment = "-- номер станции modbus удаленного " +
                "клиента";

            res += $"\t[\'{projectName}\'] =\n\t{{\n";
            res += prefix + $"ip = \'{pacInfo.IP}\',\t{ipComment}\n";
            res += prefix + $"ipemulator = \'{pacInfo.IPEmulator}\',\t" +
                $"{ipEmulatorComment}\n";
            res += prefix + $"emulation = " +
                $"{pacInfo.EmulationEnabled.ToString().ToLower()},\t" +
                $"{emulationComment}\n";
            res += prefix + $"cycletime = {pacInfo.CycleTime},\t" +
                $"{cycleTimeComment}\n";
            res += prefix + $"timeout = {pacInfo.TimeOut},\t{timeoutComment}\n";
            res += prefix + $"port = {pacInfo.Port},\t{portComment}\n";
            res += prefix + $"enabled = " +
                $"{pacInfo.GateEnabled.ToString().ToLower()},\t" +
                $"{enabledComment}\n";
            res += prefix + $"station = {pacInfo.Station},\t" +
                $"{stationComment}\n";

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

        /// <summary>
        /// Запись альтернативных проектов
        /// </summary>
        private void WriteAdvancedProjects()
        {
            //TODO: Запись альтернативных проектов
            //TODO: Каждый проект, отдельный поток
        }

        /// <summary>
        /// Имя файла межконтроллерного обмена
        /// </summary>
        private string SharedFile { get; set; }

        private InterprojectExchange owner;
    }
}
