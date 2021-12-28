using System;
using System.IO;
using Device;
using IO;
using System.Windows.Forms;

namespace EasyEPlanner
{
    public static class ProjectDescriptionSaver
    {
        static ProjectDescriptionSaver()
        {
            LoadFilePatterns();
        }

        /// <summary>
        /// Загрузка шаблонов для сохраняемых файлов
        /// </summary>
        private static void LoadFilePatterns()
        {
            const string mainProgramPatternFileName = "mainPattern.plua";
            string pathToSystemFiles = ProjectManager.GetInstance()
                .SystemFilesPath;
            string pathToMainPluaFile = Path
                .Combine(pathToSystemFiles, mainProgramPatternFileName);

            try
            {
                var reader = new StreamReader(pathToMainPluaFile,
                    EncodingDetector.DetectFileEncoding(pathToMainPluaFile));
                mainProgramFilePattern = reader.ReadToEnd();
                reader.Close();
                mainProgramFilePatternIsLoaded = true;
            }
            catch
            {
                MessageBox.Show($"Не найден шаблон {mainProgramFileName}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        /// <summary>
        /// Сохранить описание проекта
        /// </summary>
        /// <param name="param">Параметры</param>
        public static void Save(object param)
        {
            var par = param as ParametersForSave;
            if (!par.silentMode)
            {
                Logs.Show();
                Logs.DisableButtons();
                Logs.SetProgress(0);
            }

            try
            {
                try
                {
                    if (!Directory.Exists(par.path))
                    {
                        Directory.CreateDirectory(par.path);
                    }
                }
                catch (DriveNotFoundException)
                {
                    if (!par.silentMode)
                    {
                        Logs.AddMessage("Ошибка подключения к диску с " +
                            "проектами. Подключите диск!");
                        Logs.SetProgress(100);
                    }
                    return;
                }

                SaveIOFile(par);

                if (par.silentMode == false)
                {
                    Logs.SetProgress(50);
                }

                SaveTechObjectsFile(par);
                SaveTechDevicesFile(par);
                SaveRestrictionsFile(par);
                SaveMainFile(par);
                SaveModbusFile(par);
                SaveProfibusFile(par);
                SavePrgFile(par);
                SaveSharedFile(par);

                if (par.silentMode == false)
                {
                    if (!Logs.IsEmpty())
                    {
                        Logs.AddMessage("Done.");
                        Logs.ShowLastLine();
                    }
                    else
                    {
                        Logs.Hide();
                    }
                }
            }
            catch (Exception ex)
            {
                if (par.silentMode == false)
                {
                    Logs.AddMessage("Exception - " + ex);
                    Logs.AddMessage("");
                    Logs.AddMessage("");
                    Logs.ShowLastLine();
                }
            }
            finally
            {
                if (!par.silentMode && Logs.IsNull() == false)
                {
                    Logs.EnableButtons();
                    Logs.SetProgress(100);
                }
            }
        }

        /// <summary>
        /// Сохранить описание контроллера, шины в main.io.lua
        /// </summary>
        /// <param name="par">Параметры</param>
        private static void SaveIOFile(ParametersForSave par)
        {
            string fileName = par.path + @"\" + mainIOFileName;
            var fileWriter = new StreamWriter(fileName, false,
                EncodingDetector.MainFilesEncoding);

            fileWriter.WriteLine("--version  = {0}", mainIOFileVersion);
            fileWriter.WriteLine("--Eplanner  version = {0}",
                AssemblyVersion.GetVersion());
            fileWriter.WriteLine(new string('-', numberOfDashes));
            fileWriter.WriteLine("PAC_name       = \'{0}\'", par.PAC_Name);

            ushort crc = ProjectManager.CRC16(par.PAC_Name);

            fileWriter.WriteLine("PAC_id         = \'{0}\'", crc);
            fileWriter.WriteLine(new string('-', numberOfDashes));

            fileWriter.Write(IOManager.SaveAsLuaTable(""));
            fileWriter.Write(deviceManager.SaveAsLuaTableForMainIO(""));

            fileWriter.Flush();
            fileWriter.Close();
        }

        /// <summary>
        /// Сохранить объекты проекта в main.objects.lua.
        /// </summary>
        /// <param name="par">Параметры</param>
        private static void SaveTechObjectsFile(ParametersForSave par)
        {
            string filePattern = Properties.Resources.ResourceManager
                .GetString("mainObjectsPattern");
            string desctiption = techObjectManager.SaveAsLuaTable("");
            var descriptionFileData = string.Format(filePattern,
                mainTechObjectsFileVersion, AssemblyVersion.GetVersion(),
                par.PAC_Name, desctiption);

            string fileName = par.path + @"\" + MainTechObjectsFileName;
            var fileWriter = new StreamWriter(fileName, false,
                EncodingDetector.MainFilesEncoding);

            fileWriter.Write(descriptionFileData);
            fileWriter.Flush();
            fileWriter.Close();
        }

        /// <summary>
        /// Сохранить устройства проекта в main.devices.lua
        /// </summary>
        /// <param name="par">Параметры</param>
        private static void SaveTechDevicesFile(ParametersForSave par)
        {
            string fileName = par.path + @"\" + mainTechDevicesFileName;
            var fileWriter = new StreamWriter(fileName,
            false, EncodingDetector.MainFilesEncoding);

            fileWriter.WriteLine("--version  = {0}",
                mainTechDevicesFileVersion);
            fileWriter.WriteLine("--Eplanner  version = {0}",
                AssemblyVersion.GetVersion());
            fileWriter.WriteLine("--PAC_name = \'{0}\'", par.PAC_Name);
            fileWriter.WriteLine(new string('-', numberOfDashes));
            fileWriter.WriteLine(new string('-', numberOfDashes));

            fileWriter.Write(deviceManager.SaveAsLuaTableForMainDevices());

            fileWriter.Flush();
            fileWriter.Close();
        }

        /// <summary>
        /// Сохранить ограничения проекта в main.restrictions.lua
        /// </summary>
        /// <param name="par">Параметры</param>
        private static void SaveRestrictionsFile(ParametersForSave par)
        {
            string filePattern = Properties.Resources.ResourceManager
                .GetString("mainRestrictionsPattern");
            string resctrictions = techObjectManager
                .SaveRestrictionAsLua("");
            var restrictionsFileData = string.Format(filePattern,
                mainRestrictionsFileVersion, AssemblyVersion.GetVersion(),
                resctrictions);

            string fileName = par.path + @"\" + MainRestrictionsFileName;
            File.WriteAllText(fileName, restrictionsFileData,
                    EncodingDetector.MainFilesEncoding);
        }

        /// <summary>
        /// Сохранить файл main.plua (базовая инициализация).
        /// </summary>
        /// <param name="par">Параметры</param>
        private static void SaveMainFile(ParametersForSave par)
        {
            string fileName = par.path + @"\" + mainProgramFileName;
            if (!File.Exists(fileName) && mainProgramFilePatternIsLoaded)
            {
                //Создаем пустое описание управляющей программы.
                var fileWriter = new StreamWriter(fileName,
                    false, EncodingDetector.MainFilesEncoding);
                string mainPluaFilePattern = mainProgramFilePattern;
                mainPluaFilePattern = mainPluaFilePattern
                    .Replace("ProjectName", par.PAC_Name);
                mainPluaFilePattern = string.Format(mainPluaFilePattern,
                    AssemblyVersion.GetVersion());
                fileWriter.WriteLine(mainPluaFilePattern);

                fileWriter.Flush();
                fileWriter.Close();
            }
        }

        /// <summary>
        /// Сохранить файл для modbus в main.modbus.lua
        /// </summary>
        /// <param name="par">Параметры</param>
        private static void SaveModbusFile(ParametersForSave par)
        {
            string fileName = par.path + @"\" + mainModbusSRVFileName;
            if (!File.Exists(fileName))
            {
                //Создаем базовое описание сервера MODBUS.
                string filePattern = Properties.Resources.ResourceManager
                    .GetString("modbusSRVFilePattern");
                string modBusContent = string.Format(filePattern,
                    mainModbusSrvFileVersion, AssemblyVersion.GetVersion());
                File.WriteAllText(fileName, modBusContent,
                    EncodingDetector.MainFilesEncoding);
            }
        }

        /// <summary>
        /// Сохранить файл для profibus в main.profibus.lua
        /// </summary>
        /// <param name="par">Параметры</param>
        private static void SaveProfibusFile(ParametersForSave par)
        {
            string fileName = par.path + @"\" + mainProfibusFileName;
            if (!File.Exists(fileName))
            {
                //Создаем пустое описание конфигурации PROFIBUS.
                string content = "--version  = 1\n";
                content +=string.Format("--Eplanner version = {0}\n",
                    AssemblyVersion.GetVersion());
                content += "--Описание конфигурации PROFIBUS\n";
                content += new string('-', numberOfDashes) + "\n";
                content += "system = system or { }\n";
                content += "system.init_profibus = function()\n";
                content += "end\n";

                File.WriteAllText(fileName, content,
                    EncodingDetector.MainFilesEncoding);
            }
        }

        /// <summary>
        /// Сохранить программное описание проекта в prg.lua
        /// </summary>
        /// <param name="par">Параметры</param>
        private static void SavePrgFile(ParametersForSave par)
        {
            string fileName = par.path + @"\" + mainPRGFileName;
            var fileWriter = new StreamWriter(fileName,
                false, EncodingDetector.MainFilesEncoding);

            fileWriter.WriteLine("--version  = {0}", mainPRGFileVersion);
            fileWriter.WriteLine("--Eplanner version = {0}",
                AssemblyVersion.GetVersion());
            fileWriter.WriteLine("--PAC_name = \'{0}\'", par.PAC_Name);
            
            fileWriter.WriteLine(new string('-', numberOfDashes));
            fileWriter.WriteLine(new string('-', numberOfDashes));
            fileWriter.WriteLine(PrgLuaSaver.Save("\t"));

            fileWriter.Flush();
            fileWriter.Close();
        }

        private static void SaveSharedFile(ParametersForSave par)
        {
            string fileName = par.path + @"\" + sharedFileName;
            if (!File.Exists(fileName))
            {
                string sharedContent = Properties.Resources.ResourceManager
                    .GetString("sharedFilePattern");
                File.WriteAllText(fileName, sharedContent,
                    EncodingDetector.MainFilesEncoding);
            }
        }

        private const int mainIOFileVersion = 1;
        private const int mainTechObjectsFileVersion = 1;
        private const int mainTechDevicesFileVersion = 1;
        private const int mainRestrictionsFileVersion = 1;
        private const int mainPRGFileVersion = 1;
        private const int mainModbusSrvFileVersion = 1;

        private const string mainIOFileName = "main.io.lua";
        public const string MainTechObjectsFileName = "main.objects.lua";
        private const string mainTechDevicesFileName = "main.devices.lua";
        public const string MainRestrictionsFileName = "main.restrictions.lua";
        private const string mainProgramFileName = "main.plua";
        private static string mainProgramFilePattern = "";
        private static bool mainProgramFilePatternIsLoaded = false;
        private const string mainModbusSRVFileName = "main.modbus_srv.lua";
        private const string mainProfibusFileName = "main.profibus.lua";
        private const string mainPRGFileName = "prg.lua";
        private const string sharedFileName = "shared.lua";

        private const int numberOfDashes = 78;

        private static TechObject.ITechObjectManager techObjectManager =
              TechObject.TechObjectManager.GetInstance();
        private static DeviceManager deviceManager = DeviceManager
            .GetInstance();
        private static IOManager IOManager = IOManager.GetInstance();

        public static string MainProgramFileName
        {
            get
            {
                return mainProgramFileName;
            }
        }

        public class ParametersForSave
        {
            public string PAC_Name;
            public string path;
            public bool silentMode;

            public ParametersForSave(string PAC_Name, string path,
                bool silentMode)
            {
                this.PAC_Name = PAC_Name;
                this.path = path;
                this.silentMode = silentMode;
            }
        }
    }
}