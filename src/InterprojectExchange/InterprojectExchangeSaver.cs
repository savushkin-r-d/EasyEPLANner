using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

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
        /// <param name="signalsFile">Имя файла, межконтроллерного обмена
        /// </param>
        public InterprojectExchangeSaver(InterprojectExchange owner,
            string signalsFile)
        {
            interprojectExchange = owner;
            SharedFile = signalsFile;
        }

        /// <summary>
        /// Сохранить межконтроллерный обмен
        /// </summary>
        public async void SaveAsync()
        {
            await Task.Run(() => WriteMainProject());
            WriteAdvancedProjectsAsync();
        }

        /// <summary>
        /// Запись текущего проекта
        /// </summary>
        private void WriteMainProject()
        {
            var advancedModels = interprojectExchange.Models
                .Where(x => x.ProjectName !=
                interprojectExchange.MainModel.ProjectName);

            IProjectModel mainModel = interprojectExchange.MainModel;
            bool invertSignals = false;
            foreach (var model in advancedModels)
            {
                // SelectModel - с каким проектом работаем,
                // влияет на список сигналов с mainModel
                interprojectExchange.SelectModel(model);
                UpdateModelRemoteGateWays(mainModel, model, invertSignals);
                UpdateModelSharedDevices(mainModel, model, invertSignals);
            }

            WriteSharedFile(mainModel.ProjectName,
                mainModel.SharedFileAsStringList);
        }

        /// <summary>
        /// Запись альтернативных проектов
        /// </summary>
        private async void WriteAdvancedProjectsAsync()
        {
            string mainProjectName = interprojectExchange.MainProjectName;
            foreach (var model in interprojectExchange.Models)
            {
                if (model.ProjectName != mainProjectName)
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
            bool invertSignals = true;
            IProjectModel mainModel = interprojectExchange.MainModel;
            interprojectExchange.SelectModel(model);
            UpdateModelRemoteGateWays(model, mainModel, invertSignals);
            UpdateModelSharedDevices(model, mainModel, invertSignals);
            WriteSharedFile(model.ProjectName, model.SharedFileAsStringList);
        }

        /// <summary>
        /// Запись удаленных узлов модели
        /// </summary>
        /// <param name="model"></param>
        /// <param name="mainModel"></param>
        /// <param name="invertSignals">Инвертировать сигналы</param>
        private void UpdateModelRemoteGateWays(IProjectModel model,
            IProjectModel mainModel, bool invertSignals)
        {
            List<string> sharedFileData = model.SharedFileAsStringList;
            string searchPattern = $"['{mainModel.ProjectName}'] =";
            int startIndex = FindModelDescriptionStartIndex(searchPattern,
                sharedFileData);

            if (startIndex != 0)
            {
                int finishIndex = FindModelDescriptionFinishIndex(
                    startIndex, sharedFileData);
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

            if (model.MarkedForDelete || mainModel.MarkedForDelete)
            {
                return;
            }

            string remoteGateWay = SaveProjectRemoteGateWays(
                mainModel.ProjectName, model.PacInfo,
                model.ReceiverSignals, invertSignals);
            if (!string.IsNullOrEmpty(remoteGateWay))
            {
                sharedFileData.Insert(startIndex, remoteGateWay);
            }
        }

        /// <summary>
        /// Запись сигналов-источников модели
        /// </summary>
        /// <param name="model"></param>
        /// <param name="mainModel"></param>
        /// <param name="invertSignals">Инвертировать сигналы</param>
        private void UpdateModelSharedDevices(IProjectModel model,
            IProjectModel mainModel, bool invertSignals)
        {
            List<string> sharedFileData = model.SharedFileAsStringList;
            string searchPattern = $"projectName = " +
                $"\"{mainModel.ProjectName}\",";
            int startIndex = FindModelDescriptionStartIndex(searchPattern,
                sharedFileData);

            int offset = 2;
            if (startIndex != 0)
            {
                startIndex -= offset;
                int finishIndex = FindModelDescriptionFinishIndex(
                    startIndex, sharedFileData);
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


            if (model.MarkedForDelete || mainModel.MarkedForDelete)
            {
                return;
            }

            string sharedDevices = SaveProjectSharedDevices(
                mainModel.ProjectName, mainModel.PacInfo.Station,
                model.SourceSignals, invertSignals);
            if (!string.IsNullOrEmpty(sharedDevices))
            {
                sharedFileData.Insert(startIndex, sharedDevices);
            }
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
            PacInfo pacInfo, DeviceSignalsInfo signals, bool invertSignals)
        {
            if (signals.Count <= 0)
            {
                return string.Empty;
            }

            const string prefix = "\t\t";
            var res = string.Empty;

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
            int stationNum, DeviceSignalsInfo signals, bool invertSignals)
        {
            if (signals.Count <= 0)
            {
                return string.Empty;
            }

            const string prefix = "\t\t";
            var res = string.Empty;

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
        private string SavePACInfo(PacInfo pacInfo, string projectName,
            string prefix)
        {
            var res = string.Empty;

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
        private string SaveSignals(DeviceSignalsInfo signals, string prefix,
            bool invertSignals)
        {
            var res = string.Empty;
            var signalsList = new Dictionary<string, string>();

            SaveDISignals(ref signalsList, signals.DI, prefix, invertSignals);
            SaveDOSignals(ref signalsList, signals.DO, prefix, invertSignals);
            SaveAISignals(ref signalsList, signals.AI, prefix, invertSignals);
            SaveAOSignals(ref signalsList, signals.AO, prefix, invertSignals);

            var signalsKeys = signalsList.Keys.ToList();
            signalsKeys.Sort();
            foreach(var key in signalsKeys)
            {
                res += signalsList[key];
            }
            return res;
        }

        /// <summary>
        /// Сохранение DI сигналов
        /// </summary>
        /// <param name="signalsList">Словарь сигналов для сохранения</param>
        /// <param name="signals">Сигналы</param>
        /// <param name="prefix">Префикс</param>
        /// <param name="invertSignals">Инвертировать сигналы</param>
        /// <returns></returns>
        private void SaveDISignals(ref Dictionary<string, string> signalsList,
            List<string> signals, string prefix,
            bool invertSignals)
        {
            var res = string.Empty;
            var digIn = string.Empty;

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

            if (!string.IsNullOrEmpty(digIn))
            {
                if (invertSignals)
                {
                    res += prefix + 
                        $"{DOSignal} =\n{prefix}{{\n{digIn}{prefix}}},\n";
                    signalsList.Add(DOSignal, res);
                }
                else
                {
                    res += prefix + 
                        $"{DISignal} =\n{prefix}{{\n{digIn}{prefix}}},\n";
                    signalsList.Add(DISignal, res);
                }
            }
        }

        /// <summary>
        /// Сохранение DO сигналов
        /// </summary>
        /// <param name="signalsList">Словарь сигналов для сохранения</param>
        /// <param name="signals">Сигналы</param>
        /// <param name="prefix">Префикс</param>
        /// <param name="invertSignals">Инвертировать сигналы</param>
        /// <returns></returns>
        private void SaveDOSignals(ref Dictionary<string, string> signalsList, 
            List<string> signals, string prefix,
            bool invertSignals)
        {
            var res = string.Empty;
            var digOut = string.Empty;

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

            if (!string.IsNullOrEmpty(digOut))
            {
                if (invertSignals)
                {
                    res += prefix + 
                        $"{DISignal} =\n{prefix}{{\n{digOut}{prefix}}},\n";
                    signalsList.Add(DISignal, res);
                }
                else
                {
                    res += prefix + 
                        $"{DOSignal} =\n{prefix}{{\n{digOut}{prefix}}},\n";
                    signalsList.Add(DOSignal, res);
                }
            }
        }

        /// <summary>
        /// Сохранение AI сигналов
        /// </summary>
        /// <param name="signalsList">Словарь сигналов для сохранения</param>
        /// <param name="signals">Сигналы</param>
        /// <param name="prefix">Префикс</param>
        /// <param name="invertSignals">Инвертировать сигналы</param>
        /// <returns></returns>
        private void SaveAISignals(ref Dictionary<string, string> signalsList, 
            List<string> signals, string prefix,
            bool invertSignals)
        {
            var res = string.Empty;
            var analogIn = string.Empty;

            foreach (var signal in signals)
            {
                if (invertSignals)
                {
                    analogIn += prefix + $"{signal},\n";
                }
                else
                {
                    analogIn += prefix + $"__{signal},\n";
                }
            }

            if (!string.IsNullOrEmpty(analogIn))
            {
                if (invertSignals)
                {
                    res += prefix + 
                        $"{AOSignal} =\n{prefix}{{\n{analogIn}{prefix}}},\n";
                    signalsList.Add(AOSignal, res);
                }
                else
                {
                    res += prefix + 
                        $"{AISignal} =\n{prefix}{{\n{analogIn}{prefix}}},\n";
                    signalsList.Add(AISignal, res);
                }
            }
        }

        /// <summary>
        /// Сохранение AO сигналов
        /// </summary>
        /// <param name="signalsList">Словарь сигналов для сохранения</param>
        /// <param name="signals">Сигналы</param>
        /// <param name="prefix">Префикс</param>
        /// <param name="invertSignals">Инвертировать сигналы</param>
        /// <returns></returns>
        private void SaveAOSignals(ref Dictionary<string, string> signalsList, 
            List<string> signals, string prefix, bool invertSignals)
        {
            var res = string.Empty;
            var analogOut = string.Empty;

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

            if (!string.IsNullOrEmpty(analogOut))
            {
                if (invertSignals)
                {
                    res += prefix + $"{AISignal} =\n{prefix}{{\n{analogOut}" +
                        $"{prefix}}},\n";
                    signalsList.Add(AISignal, res);
                }
                else
                {
                    res += prefix + $"{AOSignal} =\n{prefix}{{\n{analogOut}" +
                        $"{prefix}}},\n";
                    signalsList.Add(AOSignal, res);
                }
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
            bool containVarInList = false;
            foreach (var lines in model.SharedFileAsStringList)
            {
                if (lines.Contains(variableName))
                {
                    containVarInList = true;
                }
            }

            bool emptyFile = model.SharedFileAsStringList.Count == 0;
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
        /// <param name="searchPattern">Шаблон поиска</param>
        /// <param name="sharedFileData">Описание shared.lua по строкам</param>
        /// <returns>Индекс начала описания модели в файле</returns>
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
        /// <returns>Индекс в файле, где кончается описание модели</returns>
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
            var model = interprojectExchange.GetModel(projectName);
            string path = Path.Combine(model.PathToProject, 
                projectName, SharedFile);
            using (var writer = new StreamWriter(path, false,
                EasyEPlanner.EncodingDetector.DetectFileEncoding(path)))
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

        private InterprojectExchange interprojectExchange;

        private const string DISignal = "DI";
        private const string DOSignal = "DO";
        private const string AISignal = "AI";
        private const string AOSignal = "AO";
    }
}