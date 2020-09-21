using System.Text;
using System.IO;
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
            string res = SaveMainProject();
            if (!string.IsNullOrEmpty(res))
            {
                res = res.Replace("\t", "    ");
                WriteSharedFile(interprojectExchange.MainProjectName,
                    new List<string> { res });
            }
            else
            {
                DeleteSharedFile(interprojectExchange.MainProjectName);
            }
        }

        /// <summary>
        /// Генерация файла для текущего проекта
        /// </summary>
        /// <returns>Содержимое файла для сохранения</returns>
        private string SaveMainProject()
        {
            var remoteGateWays = new List<string>();
            var sharedDevices = new List<string>();

            string[] loadedAdvancedModelsNames = interprojectExchange
                .LoadedAdvancedModelNames;
            CurrentProjectModel mainModel = interprojectExchange.MainModel;
            bool invertSignals = false;

            foreach (var modelName in loadedAdvancedModelsNames)
            {
                IProjectModel model = interprojectExchange.GetModel(modelName);
                string projectName = model.ProjectName;
                // SelectModel - с каким проектом работаем,
                // влияет на список сигналов с mainModel
                interprojectExchange.SelectModel(model);

                string remoteGateWay = SaveProjectRemoteGateWays(projectName, 
                    mainModel.PacInfo, mainModel.ReceiverSignals, 
                    invertSignals);
                if (!string.IsNullOrEmpty(remoteGateWay))
                {
                    remoteGateWays.Add(remoteGateWay);
                }

                string sharedDevice = SaveProjectSharedDevices(projectName,
                    model.PacInfo.Station, mainModel.SourceSignals, 
                    invertSignals);
                if (!string.IsNullOrEmpty(sharedDevice))
                {
                    sharedDevices.Add(sharedDevice);
                }
            }

            string res = GenerateMainProjectTextForWriteInFile(remoteGateWays,
                sharedDevices);
            return res;
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
            bool deleteModel = (model.ReceiverSignals.Count == 0 &&
                model.SourceSignals.Count == 0) ||
                model.MarkedForDelete;
            if (deleteModel)
            {
                DeleteSharedFile(model.ProjectName);
            }
            else
            {
                SaveAdvancedModelRemoteGateWays(model);
                SaveAdvancedModelSharedDevices(model);
            }
        }

        /// <summary>
        /// Запись удаленных узлов альтернативной модели
        /// </summary>
        /// <param name="model">Модель с данными</param>
        private void SaveAdvancedModelRemoteGateWays(IProjectModel model)
        {
            List<string> sharedFileData = model.SharedFileAsStringList;
            string searchPattern = $"['{interprojectExchange.MainProjectName}'] =";
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

            bool invertSignals = true;
            IProjectModel mainModel = interprojectExchange.MainModel;
            string remoteGateWay = SaveProjectRemoteGateWays(
                mainModel.ProjectName, model.PacInfo,
                model.ReceiverSignals, invertSignals);
            if (!string.IsNullOrEmpty(remoteGateWay))
            {
                sharedFileData.Insert(startIndex, remoteGateWay);
            }

            WriteSharedFile(model.ProjectName, sharedFileData);
        }

        /// <summary>
        /// Запись сигналов-источников альтернативной модели
        /// </summary>
        /// <param name="model">Модель с данными</param>
        private void SaveAdvancedModelSharedDevices(IProjectModel model)
        {
            List<string> sharedFileData = model.SharedFileAsStringList;
            string searchPattern = $"projectName = " +
                $"\"{interprojectExchange.MainProjectName}\",";
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

            IProjectModel mainModel = interprojectExchange.MainModel;
            interprojectExchange.SelectModel(model);
            bool invertSignals = true;
            string sharedDevices = SaveProjectSharedDevices(
                mainModel.ProjectName, mainModel.PacInfo.Station,
                model.SourceSignals, invertSignals);
            if (!string.IsNullOrEmpty(sharedDevices))
            {
                sharedFileData.Insert(startIndex, sharedDevices);
            }

            WriteSharedFile(model.ProjectName, sharedFileData);
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
                return "";
            }

            const string prefix = "\t\t";
            var res = "";

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
                return "";
            }

            const string prefix = "\t\t";
            var res = "";

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
        private string SaveSignals(DeviceSignalsInfo signals, string prefix,
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
            var digIn = "";

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
            var digOut = "";

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
            var analogIn = "";

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
                    res += prefix + $"AO =\n{prefix}{{\n{analogIn}{prefix}}},\n";
                }
                else
                {
                    res += prefix + $"AI =\n{prefix}{{\n{analogIn}{prefix}}},\n";
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
            var analogOut = "";

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
        /// Генерация текста для записи в файл по главному проекту.
        /// Аргументы передаются в виде списка, в котором содержится описание
        /// для вставки в файл.
        /// </summary>
        /// <param name="remoteGateWays">Удаленные узлы (описание)</param>
        /// <param name="sharedDevices">Устройства-источники (описание)</param>
        /// <returns></returns>
        private string GenerateMainProjectTextForWriteInFile(
            List<string> remoteGateWays, List<string> sharedDevices)
        {
            var res = "";
            const string doubleNewLine = "\n\n";
            if (remoteGateWays.Count > 0)
            {
                string remoteGateWaysStr = string.Join(doubleNewLine,
                    remoteGateWays);
                res += $"remote_gateways =\n{{\n{remoteGateWaysStr}\n}}\n";
            }

            if (sharedDevices.Count > 0)
            {
                string sharedDevicesStr = string.Join(doubleNewLine,
                    sharedDevices);
                res += $"shared_devices =\n{{\n{sharedDevicesStr}\n}}";
            }

            return res;
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
            string path = Path.Combine(interprojectExchange.PathWithProjects, 
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
        /// Удалить Shared файл
        /// </summary>
        /// <param name="projectName">Имя проекта</param>
        public void DeleteSharedFile(string projectName)
        {
            string path = Path.Combine(interprojectExchange.PathWithProjects, 
                projectName, SharedFile);
            File.Delete(path);
        }

        /// <summary>
        /// Имя файла межконтроллерного обмена
        /// </summary>
        private string SharedFile { get; set; }

        private InterprojectExchange interprojectExchange;
    }
}