using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using EasyEPlanner;
using System.Text.RegularExpressions;
using System.Text;
using System;

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
            await WriteAdvancedProjectsAsync();
        }

        /// <summary>
        /// Запись текущего проекта
        /// </summary>
        private void WriteMainProject()
        {
            var alternativeModels = interprojectExchange.Models
                .Where(x => x.ProjectName !=
                interprojectExchange.MainModel.ProjectName);

            IProjectModel mainModel = interprojectExchange.MainModel;
            bool invertSignals = false;
            foreach (var altModel in alternativeModels)
            {
                // SelectModel - с каким проектом работаем,
                // влияет на список сигналов с mainModel
                interprojectExchange.SelectModel(altModel);
                UpdateModelRemoteGateWays(mainModel, altModel, invertSignals);
                UpdateModelSharedDevices(mainModel, altModel, invertSignals);
            }

            AddOrUpdateEplannerVersionInfo(mainModel);

            WriteSharedFile(mainModel.ProjectName,
                mainModel.SharedFileAsStringList);
        }

        /// <summary>
        /// Запись альтернативных проектов
        /// </summary>
        private async Task WriteAdvancedProjectsAsync()
        {
            string mainProjectName = interprojectExchange.MainProjectName;
            foreach (var model in interprojectExchange.Models)
            {
                if (model.ProjectName != mainProjectName)
                {
                    await Task.Run(() => WriteAlternativeModel(model));
                }
            }
        }

        /// <summary>
        /// Запись модели альтернативного проекта
        /// </summary>
        /// <param name="altModel">Модель</param>
        private void WriteAlternativeModel(IProjectModel altModel)
        {
            bool invertSignals = true;
            IProjectModel mainModel = interprojectExchange.MainModel;
            // SelectModel - с каким проектом работаем,
            // влияет на список сигналов с mainModel
            interprojectExchange.SelectModel(altModel);
            UpdateModelRemoteGateWays(altModel, mainModel, invertSignals);
            UpdateModelSharedDevices(altModel, mainModel, invertSignals);

            AddOrUpdateEplannerVersionInfo(altModel);

            WriteSharedFile(altModel.ProjectName,
                altModel.SharedFileAsStringList);
        }

        /// <summary>
        /// Update or Add info about EasyEplanner version to shared.lua file 
        /// </summary>
        /// <param name="model">Project model</param>
        private void AddOrUpdateEplannerVersionInfo(IProjectModel model)
        {
            var fileAsList = model.SharedFileAsStringList;
            bool hasData = fileAsList.Count > 0;
            if (hasData)
            {
                bool hasVersionInfo = fileAsList[0]
                    .Contains(AssemblyVersion.StrForFilePattern);
                if (hasVersionInfo)
                {
                    fileAsList.RemoveAt(0);
                }

                fileAsList.Insert(0,
                        $"--{AssemblyVersion.GetStringForFileWithVersion()}");
            }
        }

        /// <summary>
        /// Запись удаленных узлов модели
        /// </summary>
        /// <param name="savingModel">Сохраняемая модель</param>
        /// <param name="oppositeModel">Противоположная сохраняемой модель
        /// </param>
        /// <param name="invertSignals">Инвертировать сигналы</param>
        private void UpdateModelRemoteGateWays(IProjectModel savingModel,
            IProjectModel oppositeModel, bool invertSignals)
        {
            List<string> sharedFileData = savingModel.SharedFileAsStringList;
            
            var searchPattern = new Regex($@"\[\s*('|""){oppositeModel.ProjectName}\1\s*\]\s*=",
                RegexOptions.None, TimeSpan.FromMilliseconds(100)); 

            int startIndex = FindModelDescriptionStartIndex(searchPattern,
                sharedFileData);

            if (startIndex >= 0)
            {
                int finishIndex = FindModelDescriptionFinishIndex(
                    startIndex, sharedFileData);
                sharedFileData.RemoveRange(startIndex,
                    (finishIndex - startIndex));
            }
            else
            {
                string valuePattern = $"remote_gateways =";
                searchPattern = new Regex(@"remote_gateways\s*=",
                    RegexOptions.None, TimeSpan.FromMilliseconds(100));

                FillDefaultSharedData(valuePattern, savingModel);
                startIndex = FindModelDescriptionStartIndex(searchPattern,
                    sharedFileData);
                int offset = 2;
                startIndex += offset;
            }

            if (savingModel.MarkedForDelete || oppositeModel.MarkedForDelete)
            {
                return;
            }

            string remoteGateWay = SaveProjectRemoteGateWays(
                oppositeModel.ProjectName, savingModel.PacInfo,
                savingModel.ReceiverSignals, invertSignals);
            if (!string.IsNullOrEmpty(remoteGateWay))
            {
                sharedFileData.Insert(startIndex, remoteGateWay);
            }
        }

        /// <summary>
        /// Запись сигналов-источников модели
        /// </summary>
        /// <param name="savingModel">Сохраняемая модель</param>
        /// <param name="oppositeModel">Противоположная сохраняемой модель
        /// </param>
        /// <param name="invertSignals">Инвертировать сигналы</param>
        private void UpdateModelSharedDevices(IProjectModel savingModel,
            IProjectModel oppositeModel, bool invertSignals)
        {
            List<string> sharedFileData = savingModel.SharedFileAsStringList;

            var searchPattern = new Regex($@"projectName\s*=\s*('|""){oppositeModel.ProjectName}\1",
                RegexOptions.None, TimeSpan.FromMilliseconds(100));
            
            int startIndex = FindModelDescriptionStartIndex(searchPattern,
                sharedFileData);

            int offset = 2;
            if (startIndex >= 0)
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
                searchPattern = new Regex(@"shared_devices\s*=",
                    RegexOptions.None, TimeSpan.FromMilliseconds(100));
                FillDefaultSharedData(valuePattern, savingModel);
                startIndex = FindModelDescriptionStartIndex(searchPattern,
                    sharedFileData);
                startIndex += offset;
            }


            if (savingModel.MarkedForDelete || oppositeModel.MarkedForDelete)
            {
                return;
            }

            string sharedDevices = SaveProjectSharedDevices(
                oppositeModel.ProjectName, oppositeModel.PacInfo.Station,
                savingModel.SourceSignals, invertSignals);
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

            return new StringBuilder()
                .Append($"\t['{projectName}'] =\n\t{{\n")
                .Append(SavePACInfo(pacInfo, prefix))
                .Append(SaveSignals(signals, prefix, invertSignals))
                .Append("\t},")
                .ToString()
                .Replace("\t", "    ");
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

            return new StringBuilder()
                .Append($"\t[{stationNum}] =\n")
                .Append("\t{\n")
                .Append($"{prefix}projectName = \"{projectName}\",\n")
                .Append(SaveSignals(signals, prefix, invertSignals))
                .Append("\t},")
                .ToString()
                .Replace("\t", "    ");
        }

        /// <summary>
        /// Сохранение информации о PAC для remote_gateways
        /// </summary>
        /// <param name="pacInfo">Информация о PAC из модели</param>
        /// <param name="projectName">Имя проекта</param>
        /// <param name="prefix">Префикс</param>
        /// <returns></returns>
        private string SavePACInfo(PacInfo pacInfo, string prefix)
        {
            string ipComment = "-- адрес удаленного контроллера";
            string ipEmulatorComment = "-- адрес удаленного контроллера при эмуляции на столе";
            string emulationComment = "-- включение эмуляции";
            string cycleTimeComment = "-- время ожидания между опросами контроллера";
            string timeoutComment = "-- таймаут для modbus клиента";
            string portComment = "-- modbus - порт удаленного контроллера";
            string enabledComment = "-- включить/выключить шлюз";
            string stationComment = "-- номер станции modbus удаленного клиента";

            var ip = $"'{pacInfo.IP}',";
            var ipemulator = $"'{pacInfo.IPEmulator}',";
            var emulation = $"{pacInfo.EmulationEnabled.ToString().ToLower()},";
            var cycletime = $"{pacInfo.CycleTime},";
            var timeout = $"{pacInfo.TimeOut},";
            var port = $"{pacInfo.Port},";
            var enabled = $"{pacInfo.GateEnabled.ToString().ToLower()},";
            var station = $"{pacInfo.Station},";

            int width = Math.Max(ip.Length, ipemulator.Length);

            return new StringBuilder()
                .Append($"{prefix}ip         = {ip.PadRight(width)} {ipComment}\n")
                .Append($"{prefix}ipemulator = {ipemulator.PadRight(width)} {ipEmulatorComment}\n")
                .Append($"{prefix}emulation  = {emulation.PadRight(width)} {emulationComment}\n")
                .Append($"{prefix}cycletime  = {cycletime.PadRight(width)} {cycleTimeComment}\n")
                .Append($"{prefix}timeout    = {timeout.PadRight(width)} {timeoutComment}\n")
                .Append($"{prefix}port       = {port.PadRight(width)} {portComment}\n")
                .Append($"{prefix}enabled    = {enabled.PadRight(width)} {enabledComment}\n")
                .Append($"{prefix}station    = {station.PadRight(width)} {stationComment}\n")
                .ToString();
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
        /// <returns>Индекс начала описания модели в файле, -1, если совпадений нет</returns>
        private int FindModelDescriptionStartIndex(Regex searchPattern,
            List<string> sharedFileData)
        {
            return sharedFileData.FindIndex(x => searchPattern.IsMatch(x));
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

            if(!File.Exists(path))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                var openedFileStream = File.Create(path);
                openedFileStream.Close();
            }

            System.Text.Encoding encoding = EasyEPlanner.EncodingDetector
                    .DetectFileEncoding(path);
            using (var writer = new StreamWriter(path, false, encoding))
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