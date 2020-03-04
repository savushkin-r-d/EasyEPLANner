using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechObject;
using Device;
using IO;

namespace EasyEPlanner
{
    public static class ProjectDescriptionSaver
    {

        public static void Save(object param)
        {
            var par = param as ProjectManager.ParamsForSave;

            StreamWriter mainIOFileWriter = null;
            StreamWriter mainTechObjectsFileWriter = null;
            StreamWriter mainTechDevicesFileWriter = null;

            StreamWriter mainRestrictionsFileWriter = null;
            StreamWriter mainFileWriter = null;
            StreamWriter prgFileWriter = null;

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

                string fileName = par.path + @"\" + mainIOFileName;
                mainIOFileWriter = new StreamWriter(fileName, false, Encoding.GetEncoding(1251));

                mainIOFileWriter.WriteLine("--version  = {0}", mainIOFileVersion);
                mainIOFileWriter.WriteLine("-- ----------------------------------------------------------------------------");
                mainIOFileWriter.WriteLine("PAC_name       = \'{0}\'", par.PAC_Name);
                ushort crc = ProjectManager.CRC16(par.PAC_Name);
                mainIOFileWriter.WriteLine("PAC_id         = \'{0}\'", crc);
                mainIOFileWriter.WriteLine("-- ----------------------------------------------------------------------------");

                if (par.silentMode == false)
                {
                    Logs.SetProgress(1);
                }

                mainIOFileWriter.Write(IOManager.SaveAsLuaTable(""));
                if (par.silentMode == false)
                {
                    Logs.SetProgress(50);
                }

                mainIOFileWriter.Write(deviceManager.SaveAsLuaTable(""));

                string fileName2 = par.path + @"\" + mainTechObjectsFileName;
                mainTechObjectsFileWriter = new StreamWriter(fileName2,
                    false, Encoding.GetEncoding(1251));

                mainTechObjectsFileWriter.WriteLine("--version  = {0}", mainTechObjectsFileVersion);
                mainTechObjectsFileWriter.WriteLine("--PAC_name = \'{0}\'", par.PAC_Name);
                mainTechObjectsFileWriter.WriteLine("-- ----------------------------------------------------------------------------");
                mainTechObjectsFileWriter.WriteLine("-- ----------------------------------------------------------------------------");

                string LuaStr = techObjectManager.SaveAsLuaTable("");
                mainTechObjectsFileWriter.WriteLine(LuaStr);

                string FILE_NAME3 = par.path + @"\" + mainTechDevicesFileName;
                mainTechDevicesFileWriter = new StreamWriter(FILE_NAME3,
                    false, Encoding.GetEncoding(1251));

                mainTechDevicesFileWriter.WriteLine("--version  = {0}", mainTechDevicesFileVersion);
                mainTechDevicesFileWriter.WriteLine("--PAC_name = \'{0}\'", par.PAC_Name);
                mainTechDevicesFileWriter.WriteLine("-- ----------------------------------------------------------------------------");
                mainTechDevicesFileWriter.WriteLine("-- ----------------------------------------------------------------------------");

                mainTechDevicesFileWriter.Write(deviceManager.SaveDevicesAsLuaScript());

                string FILE_NAME4 = par.path + @"\" + mainRestrictionsFileName;
                mainRestrictionsFileWriter = new StreamWriter(FILE_NAME4,
                    false, Encoding.GetEncoding(1251));

                mainRestrictionsFileWriter.WriteLine("--version  = {0}", mainRestrictionsFileVersion);
                mainRestrictionsFileWriter.WriteLine("-- ----------------------------------------------------------------------------");
                mainRestrictionsFileWriter.WriteLine("-- ----------------------------------------------------------------------------");
                mainRestrictionsFileWriter.Write(techObjectManager.SaveRestrictionAsLua(""));

                string mainFileName = par.path + @"\" + mainProgramFileName;
                if (!File.Exists(mainFileName))
                {
                    //Создаем пустое описание управляющей программы.
                    mainFileWriter = new StreamWriter(mainFileName,
                        false, Encoding.GetEncoding(1251));
                    mainFileWriter.WriteLine("--Проект \'{0}\'", par.PAC_Name);

                    mainFileWriter.WriteLine("------------------------------------------------------------------------------");
                    mainFileWriter.WriteLine("------------------------------------------------------------------------------");
                    mainFileWriter.WriteLine("--Пользовательская функция инициализации, выполняемая однократно в PAC.");
                    mainFileWriter.WriteLine("");
                    mainFileWriter.WriteLine("function user_init()");
                    mainFileWriter.WriteLine("end");
                    mainFileWriter.WriteLine("------------------------------------------------------------------------------");
                    mainFileWriter.WriteLine("------------------------------------------------------------------------------");
                    mainFileWriter.WriteLine("--Пользовательская функция, выполняемая каждый цикл в PAC.");
                    mainFileWriter.WriteLine("");
                    mainFileWriter.WriteLine("function user_eval()");
                    mainFileWriter.WriteLine("end");
                    mainFileWriter.WriteLine("------------------------------------------------------------------------------");
                    mainFileWriter.WriteLine("------------------------------------------------------------------------------");
                    mainFileWriter.WriteLine("--Функция инициализации параметров, выполняемая однократно в PAC.");
                    mainFileWriter.WriteLine("");
                    mainFileWriter.WriteLine("function init_params()");
                    mainFileWriter.WriteLine("end");
                    mainFileWriter.WriteLine("------------------------------------------------------------------------------");
                    mainFileWriter.WriteLine("------------------------------------------------------------------------------");
                }
                if (mainFileWriter != null)
                {
                    mainFileWriter.Flush();
                    mainFileWriter.Close();
                    mainFileWriter = null;
                }

                string modbusSrvFileName = par.path + @"\" + mainModbusSRVFileName;
                if (!File.Exists(modbusSrvFileName))
                {
                    //Создаем пустое описание сервера MODBUS.
                    mainFileWriter = new StreamWriter(modbusSrvFileName,
                        false, Encoding.GetEncoding(1251));
                    mainFileWriter.WriteLine("--version  = 1");
                    mainFileWriter.WriteLine("------------------------------------------------------------------------------");
                }
                if (mainFileWriter != null)
                {
                    mainFileWriter.Flush();
                    mainFileWriter.Close();
                    mainFileWriter = null;
                }

                string profibusFileName = par.path + @"\" + mainProfibusFileName;
                if (!File.Exists(profibusFileName))
                {
                    //Создаем пустое описание конфигурации PROFIBUS.
                    mainFileWriter = new StreamWriter(profibusFileName,
                        false, Encoding.GetEncoding(1251));
                    mainFileWriter.WriteLine("--version  = 1");
                    mainFileWriter.WriteLine("------------------------------------------------------------------------------");
                    mainFileWriter.WriteLine("system = system or { }");
                    mainFileWriter.WriteLine("system.init_profibus = function()");
                    mainFileWriter.WriteLine("end");
                }
                if (mainFileWriter != null)
                {
                    mainFileWriter.Flush();
                    mainFileWriter.Close();
                    mainFileWriter = null;
                }

                string FILE_NAME6 = par.path + @"\" + mainPRGFileName;
                prgFileWriter = new StreamWriter(FILE_NAME6,
                    false, Encoding.GetEncoding(1251));

                prgFileWriter.WriteLine("--version  = {0}", mainPRGFileVersion);
                prgFileWriter.WriteLine("--PAC_name = \'{0}\'", par.PAC_Name);
                prgFileWriter.WriteLine("-- ----------------------------------------------------------------------------");
                prgFileWriter.WriteLine("-- ----------------------------------------------------------------------------");
                prgFileWriter.WriteLine(string.Format("--Базовая функциональность\n{0}\n{1}\n{2}\n{3}\n",
                    "require( \"tank\" )",
                    "require(\"mixer\")",
                    "require(\"line\")",
                    "require(\"master\")"));
                prgFileWriter.WriteLine("-- Основные объекты проекта (объекты, описанные в Eplan).");
                prgFileWriter.WriteLine(PrgLuaSaver.Save("\t"));

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
                if (mainIOFileWriter != null)
                {
                    mainIOFileWriter.Flush();
                    mainIOFileWriter.Close();

                    // Делаем копию с другим именем (IO.lua и WAGO.lua идентичный)
                    File.Copy(par.path + @"\" + mainIOFileName,
                        par.path + @"\" + mainWagoFileName, true);
                }

                if (mainTechObjectsFileWriter != null)
                {
                    mainTechObjectsFileWriter.Flush();
                    mainTechObjectsFileWriter.Close();
                }

                if (mainTechDevicesFileWriter != null)
                {
                    mainTechDevicesFileWriter.Flush();
                    mainTechDevicesFileWriter.Close();
                }

                if (mainRestrictionsFileWriter != null)
                {
                    mainRestrictionsFileWriter.Flush();
                    mainRestrictionsFileWriter.Close();
                }

                if (prgFileWriter != null)
                {
                    prgFileWriter.Flush();
                    prgFileWriter.Close();
                }

                if (!par.silentMode && Logs.IsNull() == false)
                {
                    Logs.EnableButtons();
                    Logs.SetProgress(100);
                }
            }
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
        private const string mainModbusSRVFileName = "main.modbus_srv.lua";
        private const string mainProfibusFileName = "main.profibus.lua";
        private const string mainPRGFileName = "prg.lua";

        private static TechObjectManager techObjectManager = TechObjectManager
            .GetInstance();
        private static DeviceManager deviceManager = DeviceManager
            .GetInstance();
        private static IOManager IOManager = IOManager.GetInstance();
    }
}
