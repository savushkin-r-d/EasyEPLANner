using System.Text;
using System.IO;
using EasyEPlanner;
using System.Threading.Tasks;
using System.Collections.Generic;

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
            WriteAdvancedProjectsAsync();
        }

        /// <summary>
        /// Запись текущего проекта
        /// </summary>
        private void WriteMainProject()
        {
            string res = SaveMainProject();
            if (res.Length > 0)
            {
                res = res.Replace("\t", "    ");
                WriteSharedFile(owner.CurrentProjectName,
                    new List<string> { res });
            }
        }

        /// <summary>
        /// Генерация файла для текущего проекта
        /// </summary>
        /// <returns></returns>
        private string SaveMainProject()
        {
            string res = "";

            IProjectModel mainModel = owner.GetModel(owner.CurrentProjectName);
            string remoteGateWays = "";
            string sharedDevices = "";
            const string doubleNewLine = "\n\n";
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

                    remoteGateWays += SaveProjectRemoteGateWays(projectName,
                        model.PacInfo, currentModel.ReceiverSignals,
                        false);
                    remoteGateWays += doubleNewLine;

                    sharedDevices += SaveProjectSharedDevices(projectName,
                        model.PacInfo.Station, currentModel.SourceSignals,
                        false);
                    sharedDevices += doubleNewLine;
                }
            }

            string remoteGatewaysTemplate = "remote_gateways =\n{\nVALUE}\n";
            string sharedDevicesTemplate = "shared_devices =\n{\nVALUE}";

            if (remoteGateWays.Length > 0)
            {
                res += remoteGatewaysTemplate.Replace("VALUE", remoteGateWays);
            }

            if (sharedDevices.Length > 0)
            {
                res += sharedDevicesTemplate.Replace("VALUE", sharedDevices);
            }

            return res;
        }

        /// <summary>
        /// Генерация remote_gateways по текущему проекту
        /// </summary>
        /// <param name="pacInfo">Данные о ПЛК сохраняемого проекта</param>
        /// <param name="projectName">Имя сохраняемого проекта</param>
        /// <param name="invertSignals">Инвертировать сигналы</param>
        /// <param name="signals">Сигналы модели</param>
        /// <returns></returns>
        private string SaveProjectRemoteGateWays(string projectName,
            PacDTO pacInfo, DeviceSignalsDTO signals, bool invertSignals)
        {
            var res = "";
            if (signals.Count <= 0)
            {
                return "";
            }

            const string prefix = "\t\t";
            res += SavePACInfo(pacInfo, projectName, prefix);
            res += SaveSignals(signals, prefix, invertSignals);
            res += "\t},";

            res = res.Replace("\t", "    ");
            return res;
        }

        /// <summary>
        /// Генерация shared_devices по проекту
        /// </summary>
        /// <param name="signals">Сигналы модели</param>
        /// <param name="projectName">Имя сохраняемого проекта</param>
        /// <param name="stationNum">Номер станции PAC</param>
        /// <param name="invertSignals">Инвертировать сигналы (DI<>DO, AI<>AO)
        /// </param>
        /// <returns></returns>
        private string SaveProjectSharedDevices(string projectName,
            int stationNum, DeviceSignalsDTO signals, bool invertSignals)
        {
            var res = "";
            if (signals.Count <= 0)
            {
                return "";
            }

            const string prefix = "\t\t";
            res += $"\t[{stationNum}] =\n\t{{\n";
            res += prefix + $"projectName = \"{projectName}\",\n";

            res += SaveSignals(signals, prefix, invertSignals);

            res += "\t},";

            res = res.Replace("\t", "    ");
            return res;
        }

        /// <summary>
        /// Сохранение информации о PAC для remote_gateways
        /// </summary>
        /// <param name="pacInfo">Информация о PAC из модели</param>
        /// <param name="projectName">Имя проекта</param>
        /// <param name="prefix">Префикс</param>
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
        /// <param name="invertSignals">Инвертировать сигналы</param>
        /// <returns></returns>
        private string SaveSignals(DeviceSignalsDTO signals, string prefix,
            bool invertSignals)
        {
            var res = "";

            res += SaveDISignals(signals.DI, prefix, invertSignals);
            res += SaveDOSignals(signals.DO, prefix, invertSignals);
            res += SaveAISignals(signals.AI, prefix, invertSignals);
            res += SaveAOSignals(signals.AO, prefix, invertSignals);

            return res;
        }

        /// <summary>
        /// Сохранение DI сигналов
        /// </summary>
        /// <param name="signals">Сигналы</param>
        /// <param name="prefix">Префикс</param>
        /// <param name="invertSignals">Инвертировать сигналы</param>
        /// <returns></returns>
        private string SaveDISignals(List<string> signals, string prefix,
            bool invertSignals)
        {
            var res = "";

            string digIn = "";
            foreach (var signal in signals)
            {
                if (invertSignals)
                {
                    digIn += prefix + $"{signal},\n";
                }
                else
                {
                    digIn += prefix + $"__{signal},\n";
                }
            }
            if (digIn.Length > 0)
            {
                if (invertSignals)
                {
                    res += prefix + $"DO =\n{prefix}{{\n{digIn}{prefix}}},\n";
                }
                else
                {
                    res += prefix + $"DI =\n{prefix}{{\n{digIn}{prefix}}},\n";
                }
            }

            return res;
        }

        /// <summary>
        /// Сохранение DO сигналов
        /// </summary>
        /// <param name="signals">Сигналы</param>
        /// <param name="prefix">Префикс</param>
        /// <param name="invertSignals">Инвертировать сигналы</param>
        /// <returns></returns>
        private string SaveDOSignals(List<string> signals, string prefix,
            bool invertSignals)
        {
            var res = "";

            string digOut = "";
            foreach (var signal in signals)
            {
                if (invertSignals)
                {
                    digOut += prefix + $"__{signal},\n";

                }
                else
                {
                    digOut += prefix + $"{signal},\n";
                }
            }
            if (digOut.Length > 0)
            {
                if (invertSignals)
                {
                    res += prefix + $"DI =\n{prefix}{{\n{digOut}{prefix}}},\n";
                }
                else
                {
                    res += prefix + $"DO =\n{prefix}{{\n{digOut}{prefix}}},\n";
                }
            }

            return res;
        }

        /// <summary>
        /// Сохранение AI сигналов
        /// </summary>
        /// <param name="signals">Сигналы</param>
        /// <param name="prefix">Префикс</param>
        /// <param name="invertSignals">Инвертировать сигналы</param>
        /// <returns></returns>
        private string SaveAISignals(List<string> signals, string prefix,
            bool invertSignals)
        {
            var res = "";

            string ai = "";
            foreach (var signal in signals)
            {
                if (invertSignals)
                {
                    ai += prefix + $"{signal},\n";
                }
                else
                {
                    ai += prefix + $"__{signal},\n";
                }
            }
            if (ai.Length > 0)
            {
                if (invertSignals)
                {
                    res += prefix + $"AO =\n{prefix}{{\n{ai}{prefix}}},\n";
                }
                else
                {
                    res += prefix + $"AI =\n{prefix}{{\n{ai}{prefix}}},\n";
                }
            }

            return res;
        }

        /// <summary>
        /// Сохранение AO сигналов
        /// </summary>
        /// <param name="signals">Сигналы</param>
        /// <param name="prefix">Префикс</param>
        /// <param name="invertSignals">Инвертировать сигналы</param>
        /// <returns></returns>
        private string SaveAOSignals(List<string> signals, string prefix,
            bool invertSignals)
        {
            var res = "";

            string analogOut = "";
            foreach (var signal in signals)
            {
                if (invertSignals)
                {
                    analogOut += prefix + $"__{signal},\n";
                }
                else
                {
                    analogOut += prefix + $"{signal},\n";
                }
            }
            if (analogOut.Length > 0)
            {
                if (invertSignals)
                {
                    res += prefix + $"AI =\n{prefix}{{\n{analogOut}" +
                        $"{prefix}}},\n";
                }
                else
                {
                    res += prefix + $"AO =\n{prefix}{{\n{analogOut}" +
                        $"{prefix}}},\n";
                }
            }

            return res;
        }

        /// <summary>
        /// Запись альтернативных проектов
        /// </summary>
        private async void WriteAdvancedProjectsAsync()
        {
            foreach (var model in owner.Models)
            {
                if (model.ProjectName != owner.CurrentProjectName)
                {
                    await Task.Run(() => WriteAdvancedModel(model));
                }
            }
        }

        /// <summary>
        /// Запись модели альтернативного проекта
        /// </summary>
        /// <param name="model">Модель</param>
        private void WriteAdvancedModel(IProjectModel model)
        {
            bool invalidModel = model.MarkedForDelete || 
                (model.ReceiverSignals.Count == 0
                && model.SourceSignals.Count == 0);
            if (!invalidModel)
            {
                WriteAdvancedModelRemoteGateWays(model);
                WriteAdvancedModelSharedDevices(model);
            }
        }

        /// <summary>
        /// Запись удаленных узлов альтернативной модели
        /// </summary>
        /// <param name="model">Модель с данными</param>
        private void WriteAdvancedModelRemoteGateWays(IProjectModel model)
        {
            List<string> sharedFileData = model.SharedFileAsStringList;
            string searchPattern = $"['{owner.CurrentProjectName}'] =";
            int startIndex = FindModelDescriptionStartIndex(searchPattern,
                sharedFileData);

            if (startIndex != 0)
            {
                int finishIndex = FindModelDescriptionFinishIndex(startIndex,
                    sharedFileData);
                sharedFileData.RemoveRange(startIndex,
                    (finishIndex - startIndex));
            }
            else
            {
                string valuePattern = $"remote_gateways =";
                FillDefaultSharedData(valuePattern, model);

                startIndex = FindModelDescriptionStartIndex(valuePattern,
                    sharedFileData);
                int offset = 2;
                startIndex += offset;
            }

            IProjectModel mainModel = owner.GetModel(
                owner.CurrentProjectName);

            mainModel.PacInfo.Station = model.PacInfo.Station;
            string remoteGateWay = SaveProjectRemoteGateWays(
                mainModel.ProjectName, mainModel.PacInfo,
                model.ReceiverSignals, true);
            if (remoteGateWay.Length > 0)
            {
                sharedFileData.Insert(startIndex, remoteGateWay);
                WriteSharedFile(model.ProjectName, sharedFileData);
            }
        }

        /// <summary>
        /// Запись сигналов-источников альтернативной модели
        /// </summary>
        /// <param name="model">Модель с данными</param>
        private void WriteAdvancedModelSharedDevices(IProjectModel model)
        {
            List<string> sharedFileData = model.SharedFileAsStringList;
            string searchPattern = $"projectName = " +
                $"\"{owner.CurrentProjectName}\",";
            int startIndex = FindModelDescriptionStartIndex(searchPattern,
                sharedFileData);

            int offset = 2;
            if (startIndex != 0)
            {
                startIndex -= offset;
                int finishIndex = FindModelDescriptionFinishIndex(startIndex,
                    sharedFileData);
                sharedFileData.RemoveRange(startIndex,
                    (finishIndex - startIndex));
            }
            else
            {
                string valuePattern = $"shared_devices =";
                FillDefaultSharedData(valuePattern, model);

                startIndex = FindModelDescriptionStartIndex(valuePattern,
                    sharedFileData);
                startIndex += offset;
            }

            IProjectModel mainModel = owner.GetModel(
                owner.CurrentProjectName);

            mainModel.PacInfo.Station = model.PacInfo.Station;
            string sharedDevices = SaveProjectSharedDevices(
                mainModel.ProjectName, model.PacInfo.Station,
                model.SourceSignals, true);
            if (sharedDevices.Length != 0)
            {
                sharedFileData.Insert(startIndex, sharedDevices);
                WriteSharedFile(model.ProjectName, sharedFileData);
            }
        }

        /// <summary>
        /// Заполнение SharedFileData стандартным значением
        /// </summary>
        /// <param name="variableName">Имя переменной и знаком равно ("var1 =")
        /// </param>
        /// <param name="model">Модель, которая обрабатывается</param>
        private void FillDefaultSharedData(string variableName, 
            IProjectModel model)
        {
            bool emptyFile = model.SharedFileAsStringList.Count == 0;

            bool containVarInList = false;
            foreach (var lines in model.SharedFileAsStringList)
            {
                if (lines.Contains(variableName))
                {
                    containVarInList = true;
                }
            }

            if (emptyFile || !containVarInList)
            {
                model.SharedFileAsStringList.Add(variableName);
                model.SharedFileAsStringList.Add("{");
                model.SharedFileAsStringList.Add("}");
            }
        }

        /// <summary>
        /// Поиск вхождения шаблона в файле shared.lua
        /// </summary>
        /// <returns></returns>
        private int FindModelDescriptionStartIndex(string searchPattern,
            List<string> sharedFileData)
        {
            int res = 0;

            for (int i = 0; i < sharedFileData.Count; i++)
            {
                if (sharedFileData[i].Contains(searchPattern))
                {
                    res = i;
                }
            }

            return res;
        }

        /// <summary>
        /// Поиск окончания описания модели в shared.lua
        /// </summary>
        /// <param name="startIndex">Стартовый индекс для поиска</param>
        /// <param name="sharedFileData">Описание shared.lua по строкам</param>
        /// <returns></returns>
        private int FindModelDescriptionFinishIndex(int startIndex,
            List<string> sharedFileData)
        {
            bool foundFinish = false;
            int finishIndex = startIndex + 1;
            int countOfBrackets = 0;
            while (foundFinish == false)
            {
                if (sharedFileData[finishIndex].Contains("{"))
                {
                    countOfBrackets++;
                }

                if (sharedFileData[finishIndex].Contains("}"))
                {
                    countOfBrackets--;
                }

                if (countOfBrackets == 0)
                {
                    foundFinish = true;
                }

                finishIndex++;
            }

            return finishIndex;
        }

        /// <summary>
        /// Запись shared.lua
        /// </summary>
        /// <param name="projectName">Имя проекта</param>
        /// <param name="sharedFileData">Данные файла для записи в виде списка
        /// строк</param>
        private void WriteSharedFile(string projectName,
            List<string> sharedFileData)
        {
            string path = Path.Combine(ProjectManager.GetInstance()
                .GetPtusaProjectsPath(""), projectName, SharedFile);
            using (var writer = new StreamWriter(path, false,
                Encoding.GetEncoding(1251)))
            {
                foreach (string line in sharedFileData)
                {
                    writer.WriteLine(line);
                }
            }
        }

        /// <summary>
        /// Имя файла межконтроллерного обмена
        /// </summary>
        private string SharedFile { get; set; }

        private InterprojectExchange owner;
    }
}
