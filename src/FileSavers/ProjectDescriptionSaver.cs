using System;
using System.IO;
using System.Text;
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
        /// Сохранить описание контроллера, шины в main.io.lua и main.wago.lua
        /// </summary>
        /// <param name="par">Параметры</param>
        private static void SaveIOFile(ParametersForSave par)
        {
            string fileName = par.path + @"\" + mainIOFileName;
            var fileWriter = new StreamWriter(fileName, false,
                EncodingDetector.DetectFileEncoding(fileName));

            fileWriter.WriteLine("--version  = {0}", mainIOFileVersion);
            fileWriter.WriteLine(new string('-', numberOfDashes));
            fileWriter.WriteLine("PAC_name       = \'{0}\'", par.PAC_Name);

            ushort crc = ProjectManager.CRC16(par.PAC_Name);

            fileWriter.WriteLine("PAC_id         = \'{0}\'", crc);
            fileWriter.WriteLine(new string('-', numberOfDashes));

            fileWriter.Write(IOManager.SaveAsLuaTable(""));
            fileWriter.Write(deviceManager.SaveAsLuaTable(""));

            fileWriter.Flush();
            fileWriter.Close();

            // Делаем копию с другим именем (IO.lua и WAGO.lua идентичный)
            File.Copy(par.path + @"\" + mainIOFileName,
                par.path + @"\" + mainWagoFileName, true);
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
                mainTechObjectsFileVersion, par.PAC_Name, desctiption);

            string fileName = par.path + @"\" + mainTechObjectsFileName;
            var fileWriter = new StreamWriter(fileName, false,
                EncodingDetector.DetectFileEncoding(fileName));

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
                false, EncodingDetector.DetectFileEncoding(fileName));

            fileWriter.WriteLine("--version  = {0}", 
                mainTechDevicesFileVersion);
            fileWriter.WriteLine("--PAC_name = \'{0}\'", par.PAC_Name);
            fileWriter.WriteLine(new string('-', numberOfDashes));
            fileWriter.WriteLine(new string('-', numberOfDashes));

            fileWriter.Write(deviceManager.SaveDevicesAsLuaScript());

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
                mainRestrictionsFileVersion, resctrictions);

            string fileName = par.path + @"\" + mainRestrictionsFileName;
            var fileWriter = new StreamWriter(fileName, false,
                EncodingDetector.DetectFileEncoding(fileName));

            fileWriter.Write(restrictionsFileData);
            fileWriter.Flush();
            fileWriter.Close();
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
                    false, EncodingDetector.DetectFileEncoding(fileName));
                string mainPluaFilePattern = mainProgramFilePattern;
                mainPluaFilePattern = mainPluaFilePattern
                    .Replace("ProjectName", par.PAC_Name);
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
                //Создаем пустое описание сервера MODBUS.
                var fileWriter = new StreamWriter(fileName,
                    false, EncodingDetector.DetectFileEncoding(fileName));
                fileWriter.WriteLine("--version  = 1");
                fileWriter.WriteLine(new string('-', numberOfDashes));

                fileWriter.Flush();
                fileWriter.Close();
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
                var fileWriter = new StreamWriter(fileName,
                    false, EncodingDetector.DetectFileEncoding(fileName));
                fileWriter.WriteLine("--version  = 1");
                fileWriter.WriteLine(new string('-', numberOfDashes));
                fileWriter.WriteLine("system = system or { }");
                fileWriter.WriteLine("system.init_profibus = function()");
                fileWriter.WriteLine("end");

                fileWriter.Flush();
                fileWriter.Close();
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
                false, EncodingDetector.DetectFileEncoding(fileName));

            fileWriter.WriteLine("--version  = {0}", mainPRGFileVersion);
            fileWriter.WriteLine("--PAC_name = \'{0}\'", par.PAC_Name);
            fileWriter.WriteLine(new string('-', numberOfDashes));
            fileWriter.WriteLine(new string('-', numberOfDashes));
            string requireModules = Properties.Resources.ResourceManager
            .GetString("prgLuaRequireModules");
            fileWriter.WriteLine(requireModules);
            fileWriter.WriteLine("-- Основные объекты проекта (объекты, " +
                "описанные в Eplan).");

            fileWriter.WriteLine(PrgLuaSaver.Save("\t"));

            fileWriter.Flush();
            fileWriter.Close();
        }

        private const int mainIOFileVersion = 1;
        private const int mainTechObjectsFileVersion = 1;
        private const int mainTechDevicesFileVersion = 1;
        private const int mainRestrictionsFileVersion = 1;
        private const int mainPRGFileVersion = 1;

        private const string mainIOFileName = "main.io.lua";
        private const string mainWagoFileName = "main.wago.lua";
        private const string mainTechObjectsFileName = "main.objects.lua";
        private const string mainTechDevicesFileName = "main.devices.lua";
        private const string mainRestrictionsFileName = "main.restrictions.lua";
        private const string mainProgramFileName = "main.plua";
        private static string mainProgramFilePattern = "";
        private static bool mainProgramFilePatternIsLoaded = false;
        private const string mainModbusSRVFileName = "main.modbus_srv.lua";
        private const string mainProfibusFileName = "main.profibus.lua";
        private const string mainPRGFileName = "prg.lua";

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