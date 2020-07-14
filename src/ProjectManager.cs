﻿using Device;
using Editor;
using IO;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using TechObject;

namespace EasyEPlanner
{
    /// <summary>
    /// Менеджер проекта.
    /// </summary>
    public class ProjectManager
    {
        /// <summary>
        /// Вычисление CRC16 кода строки.
        /// </summary>
        public static ushort CRC16(string str)
        {
            byte[] pcBlock = System.Text.Encoding.Default.GetBytes(str);
            int len = pcBlock.GetLength(0);

            ushort crc = 0xFFFF;

            ushort idx = 0;
            while (len-- > 0)
            {
                crc ^= (ushort)(pcBlock[idx++] << 8);

                for (int i = 0; i < 8; i++)
                    crc = (ushort)((crc & 0x8000) > 0 ?
                         ((crc << 1) ^ 0x1021) : (crc << 1));
            }

            return crc;
        }

        /// <summary>
        /// Сохранение описания в виде скрипта Lua.
        /// </summary>
        public void SaveAsLua(string PAC_Name, string path, bool silentMode)
        {
            var param = new ProjectDescriptionSaver.ParametersForSave(PAC_Name, 
                path, silentMode);

            if (silentMode)
            {
                ProjectDescriptionSaver.Save(param);
            }
            else
            {
                var t = new System.Threading.Thread(new System.Threading
                    .ParameterizedThreadStart(ProjectDescriptionSaver.Save));
                t.Start(param);
            }
        }

        /// <summary>
        /// Считывание описания.
        /// </summary>
        private int LoadDescriptionFromFile(out string LuaStr,
            out string errStr, string projectName, string fileName)
        {
            LuaStr = "";
            errStr = "";

            StreamReader sr = null;
            string path = GetPtusaProjectsPath(projectName) + projectName + fileName;

            try
            {
                if (!File.Exists(path))
                {
                    errStr = "Файл описания проекта \"" + path + "\" отсутствует!" +
                        " Создано пустое описание.";
                    return 1;
                }
            }
            catch (DriveNotFoundException)
            {
                errStr = "Укажите правильные настройки каталога!";
                return 1;
            }

            sr = new StreamReader(path,
                System.Text.Encoding.GetEncoding(1251));
            LuaStr = sr.ReadToEnd();
            sr.Close();

            return 0;
        }

        /// <summary>
        /// Путь к файлам .lua (к проекту)
        /// </summary>
        /// <returns></returns>
        public string GetPtusaProjectsPath(string projectName)
        {
            try
            {
                // Поиск пути к каталогу с надстройкой
                string[] originalAssemblyPath = OriginalAssemblyPath
                    .Split('\\');
                string configFileName = StaticHelper.CommonConst.ConfigFileName;

                int sourceEnd = originalAssemblyPath.Length;
                string path = @"";
                for (int source = 0; source < sourceEnd; source++)
                {
                    path += originalAssemblyPath[source].ToString() + "\\";
                }
                path += StaticHelper.CommonConst.ConfigFileName;

                // Поиск файла .ini
                if (!File.Exists(path))
                {
                    // Если не нашли - создаем новый и записываем дефолтные данные
                    new PInvoke.IniFile(path);
                    StreamWriter sr = new StreamWriter(path, true);
                    sr.WriteLine("[path]\nfolder_path=");
                    sr.Close();
                    sr.Flush();
                }
                PInvoke.IniFile iniFile = new PInvoke.IniFile(path);

                // Считывание и возврат пути каталога проектов
                string projectsFolders =
                    iniFile.ReadString("path", "folder_path", "");
                string[] projectsFolderArray = projectsFolders.Split(';');
                string projectsFolder = "";
                bool firstPathIsSaved = false;
                string firstPath = "";
                foreach (string pathFromArray in projectsFolderArray)
                {
                    if (pathFromArray != "")
                    {
                        if (firstPathIsSaved == false)
                        {
                            firstPath = pathFromArray;
                            firstPathIsSaved = true;
                        }
                        projectsFolder = pathFromArray;
                        if (projectsFolder.Last() != '\\')
                        {
                            projectsFolder += '\\';
                        }
                        string projectsPath = projectsFolder + projectName;
                        if (Directory.Exists(projectsPath))
                        {
                            return projectsFolder;
                        }
                    }
                }

                if (firstPathIsSaved == false && firstPath == "")
                {
                    MessageBox.Show("Путь к каталогу с проектами не найден.\n" +
                        "Пожалуйста, проверьте конфигурацию!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    return firstPath + '\\';
                }
            }
            catch
            {
                MessageBox.Show("Файл конфигурации не найден - будет создан новый со стандартным описанием." +
                    " Пожалуйста, измените путь к каталогу с проектами, где хранятся Lua файлы!",
                    "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return "";
        }

        /// <summary>
        /// Считывание описания.
        /// </summary>
        public int LoadDescription(out string errStr,
            string projectName, bool loadFromLua)
        {
            errStr = "";
            Logs.Clear();

            string LuaStr;
            int res = 0;

            Eplan.EplApi.Base.Progress oProgress =
                new Eplan.EplApi.Base.Progress("EnhancedProgress");
            oProgress.SetAllowCancel(false);
            oProgress.SetTitle("Считывание данных проекта");

            try
            {
                oProgress.BeginPart(15, "Считывание IO");
                projectConfiguration.ReadIO();
                oProgress.EndPart();

                oProgress.BeginPart(15, "Считывание устройств");
                if (projectConfiguration.DevicesIsRead == true)
                {
                    projectConfiguration.SynchronizeDevices();
                }
                else
                {
                    projectConfiguration.ReadDevices();
                }
                oProgress.EndPart();

                oProgress.BeginPart(25, "Считывание привязки устройств");
                projectConfiguration.ReadBinding();
                oProgress.EndPart();

                if (loadFromLua)
                {
                    oProgress.BeginPart(15, "Считывание технологических объектов");
                    res = LoadDescriptionFromFile(out LuaStr, out errStr, projectName, "\\main.objects.lua");
                    techObjectManager.LoadFromLuaStr(LuaStr, projectName);
                    errStr = "";
                    LuaStr = "";
                    res = LoadDescriptionFromFile(out LuaStr, out errStr, projectName, "\\main.restrictions.lua");
                    techObjectManager.LoadRestriction(LuaStr);
                    oProgress.EndPart();
                }

                oProgress.BeginPart(15, "Проверка данных");
                projectConfiguration.Check();
                oProgress.EndPart();

                oProgress.BeginPart(15, "Расчет IO-Link");
                IOManager.CalculateIOLinkAdresses();
                oProgress.EndPart(true);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
                oProgress.EndPart(true);
            }

            return res;
        }

        /// <summary>
        /// Инициализация.
        /// </summary>
        public void Init()
        {
            this.editor = Editor.Editor.GetInstance();
            this.techObjectManager = TechObjectManager.GetInstance();
            Logs.Init(new LogFrm());           
            this.IOManager = IOManager.GetInstance();
            DeviceManager.GetInstance();
            this.projectConfiguration = ProjectConfiguration.GetInstance();
            EProjectManager.GetInstance();
            BaseTechObjectManager.GetInstance();
        }

        /// <summary>
        /// Сохранение описания в виде таблицы Excel.
        /// </summary>
        public void SaveAsExcelDescription(string path)
        {
            System.Threading.Thread t = new System.Threading.Thread(
                    new System.Threading.ParameterizedThreadStart(ExportToExcel));

            t.Start(path);
        }

        /// <summary>
        /// Экспорт технологических объектов в Excel.
        /// </summary>
        /// <param name="path">Путь к директории сохранения</param>
        /// <param name="projectName">Имя проекта</param>
        private void ExportToExcel(object param)
        {
            var par = param as string;
            Logs.Show();

            Logs.DisableButtons();
            Logs.Clear();
            Logs.SetProgress(0);

            try
            {
                Logs.SetProgress(1);
                ExcelRepoter.ExportTechDevs(par);
                Logs.AddMessage("Done.");
            }

            catch (System.Exception ex)
            {
                Logs.AddMessage("Exception - " + ex);
            }
            finally
            {
                if (Logs.IsNull() == false)
                {
                    Logs.EnableButtons();
                    Logs.SetProgress(100);
                }
            }
        }

        /// <summary>
        /// Обновление подписей к клеммам модулей IO
        /// в соответствии с актуальным названием устройств.
        /// </summary>
        public void UpdateModulesBinding()
        {
            var errors = "";
            try
            {
                Logs.Clear();
                Logs.Show();
                Logs.AddMessage("Выполняется синхронизация..");
                errors = ModulesBindingUpdater.GetInstance().Execute();
                Logs.Clear();
            }
            catch (System.Exception ex)
            {
                Logs.AddMessage("Exception - " + ex);
            }
            finally
            {
                if (errors != "")
                {
                    Logs.AddMessage(errors);
                }

                if (Logs.IsNull() == false)
                {
                    Logs.AddMessage("Синхронизация завершена. ");
                    Logs.SetProgress(100);
                    Logs.EnableButtons();
                }
            }
        }

        /// <summary>
        /// Экспорт из проекта базы каналов.
        /// </summary>
        public void SaveAsCDBX(string projectName, bool combineTag = false,
            bool useNewNames = false, bool rewrite = false)
        {
            techObjectManager.SetCDBXTagView(combineTag);
            techObjectManager.SetCDBXNewNames(useNewNames);

            System.Threading.Thread t = new System.Threading.Thread(
                    new System.Threading.ParameterizedThreadStart(
                        SaveAsXMLThread));

            t.Start(new DataForSaveAsXML(projectName, rewrite));

        }

        private void SaveAsXMLThread(object param)
        {
            var data = param as DataForSaveAsXML;

            Logs.Show();
            Logs.DisableButtons();
            Logs.Clear();
            Logs.SetProgress(0);

            try
            {
                Logs.SetProgress(1);
                XMLReporter.SaveAsXML(data.pathToFile, data.rewrite);
                Logs.SetProgress(50);
                Logs.AddMessage("Done.");
            }
            catch (System.Exception ex)
            {
                Logs.AddMessage("Exception - " + ex);
            }
            finally
            {
                if (Logs.IsNull() == false)
                {
                    Logs.EnableButtons();
                    Logs.SetProgress(100);
                }
            }
        }

        /// <summary>
        /// Класс для передачи данных при сохранении XML базы каналов
        /// </summary>
        private class DataForSaveAsXML
        {
            public DataForSaveAsXML(string path, bool rewrite)
            {
                this.pathToFile = path;
                this.rewrite = rewrite;
            }

            public string pathToFile;
            public bool rewrite;
        }

        /// <summary>
        /// Получение экземпляра класса.
        /// </summary>
        /// <returns>Единственный экземпляр класса.</returns>
        public static ProjectManager GetInstance()
        {
            if (null == instance)
            {
                instance = new ProjectManager();
            }

            return instance;
        }

        /// <summary>
        /// Редактирование технологических объектов.
        /// </summary>
        /// <param name="objectsLuaStr">Скрипт Lua с текущими описанием.</param>        
        /// <returns>Результат редактирования.</returns>
        public string Edit()
        {
            string res = editor.Edit(techObjectManager as ITreeViewItem);

            return res;
        }

        //Участвующие в операции устройства, подсвеченные на карте Eplan.
        List<object> highlightedObjects = new List<object>();

        /// <summary>
        /// Отключить подсветку устройств
        /// </summary>
        /// <param name="isClosingProject">Флаг закрытия проекта</param>
        public void RemoveHighLighting(bool isClosingProject = false)
        {
            foreach (object obj in highlightedObjects)
            {
                var drawedObject = obj as Eplan.EplApi.DataModel.Graphics
                    .GraphicalPlacement;
                if (isClosingProject)
                {
                    drawedObject.SmartLock();
                }
                drawedObject.Remove();
            }

            highlightedObjects.Clear();
        }

        public void SetHighLighting(List<DrawInfo> objectsToDraw)
        {
            if (objectsToDraw == null)
            {
                return;
            }
            foreach (DrawInfo drawObj in objectsToDraw)
            {
                DrawInfo.Style howToDraw = drawObj.style;

                if (howToDraw == DrawInfo.Style.NO_DRAW)
                {
                    continue;
                }

                Eplan.EplApi.DataModel.Function oF =
                    (drawObj.dev as IODevice).EplanObjectFunction;
                if (oF == null)
                {
                    continue;
                }

                Eplan.EplApi.Base.PointD[] points = oF.GetBoundingBox();
                int number = 0;

                Eplan.EplApi.DataModel.Graphics.Rectangle rc =
                    new Eplan.EplApi.DataModel.Graphics.Rectangle();
                rc.Create(oF.Page);
                rc.IsSurfaceFilled = true;
                rc.DrawingOrder = -1;

                short colour = 0;
                switch (howToDraw)
                {
                    case DrawInfo.Style.GREEN_BOX:
                        colour = 3; //Green.

                        //Для сигналов подсвечиваем полностью всю линию.
                        if (oF.Name.Contains("DI") || oF.Name.Contains("DO"))
                        {
                            if (oF.Connections.Length > 0)
                            {
                                points[1].X =
                                oF.Connections[0].StartPin.ParentFunction.GetBoundingBox()[1].X;
                            }
                        }
                        break;

                    case DrawInfo.Style.RED_BOX:
                        colour = 252; //Red.
                        break;

                    case DrawInfo.Style.GREEN_UPPER_BOX:
                        points[0].Y += (points[1].Y - points[0].Y) / 2;
                        colour = 3; //Green.
                        break;

                    case DrawInfo.Style.GREEN_LOWER_BOX:
                        points[1].Y -= (points[1].Y - points[0].Y) / 2;
                        colour = 3; //Green.
                        break;

                    case DrawInfo.Style.GREEN_RED_BOX:
                        Eplan.EplApi.DataModel.Graphics.Rectangle rc2 =
                            new Eplan.EplApi.DataModel.Graphics.Rectangle();
                        rc2.Create(oF.Page);
                        rc2.IsSurfaceFilled = true;
                        rc2.DrawingOrder = 1;
                        rc2.SetArea(
                            new Eplan.EplApi.Base.PointD(
                                points[0].X, points[0].Y + (points[1].Y - points[0].Y) / 2),
                                points[1]);

                        rc2.Pen = new Eplan.EplApi.DataModel.Graphics.Pen(
                            252 /*Red*/, -16002, -16002, -16002, 0);

                        rc2.Properties.set_PROPUSER_TEST(1, oF.ToStringIdentifier());
                        highlightedObjects.Add(rc2);

                        points[1].Y -= (points[1].Y - points[0].Y) / 2;
                        colour = 3; //Green.
                        break;
                }

                rc.SetArea(points[0], points[1]);
                rc.Pen = new Eplan.EplApi.DataModel.Graphics.Pen(
                    colour, -16002, -16002, -16002, 0);

                if (number != 0)
                {
                    Eplan.EplApi.DataModel.Graphics.Text txt =
                        new Eplan.EplApi.DataModel.Graphics.Text();
                    txt.Create(oF.Page, number.ToString(), 3);
                    txt.Location = new Eplan.EplApi.Base.PointD(
                        points[1].X, points[1].Y);
                    txt.Justification =
                        Eplan.EplApi.DataModel.Graphics.TextBase.JustificationType.BottomRight;
                    txt.TextColorId = 0;

                    highlightedObjects.Add(txt);
                }

                rc.Properties.set_PROPUSER_TEST(1, oF.ToStringIdentifier());
                highlightedObjects.Add(rc);
            }
        }

        /// <summary>
        /// Получить ссылку на систему помощи Ostis
        /// </summary>
        /// <returns></returns>
        public string GetOstisHelpSystemLink()
        {
            var configFile = new PInvoke.IniFile(Path.Combine(
                OriginalAssemblyPath, 
                StaticHelper.CommonConst.ConfigFileName));
            string link = configFile.ReadString("helpSystem", "address", null);
            if (string.IsNullOrEmpty(link))
            {
                configFile.WriteString("helpSystem", "address", "");
            }
            return link;
        }

        /// <summary>
        /// Получить ссылку на основную страницы системы помощи Ostis
        /// </summary>
        /// <returns></returns>
        public string GetOstisHelpSystemMainPageLink()
        {
            var configFile = new PInvoke.IniFile(Path.Combine(
                OriginalAssemblyPath,
                StaticHelper.CommonConst.ConfigFileName));
            string link = configFile.ReadString("helpSystem", "mainAddress ", 
                null);
            if (string.IsNullOrEmpty(link))
            {
                configFile.WriteString("helpSystem", "mainAddress", "");
            }
            return link;
        }

        /// <summary>
        /// Путь к надстройке, к месту, из которого она подключалась к программе
        /// инженером.
        /// </summary>
        public string OriginalAssemblyPath
        {
            get
            {
                return Path.GetDirectoryName(AddInModule.OriginalAssemblyPath);
            }
        }

        /// <summary>
        /// Название папки с системными скриптами
        /// </summary>
        private const string luaFolder = "Lua";

        /// <summary>
        /// Путь к надстройке в теневом хранилище Eplan
        /// </summary>
        public string AssemblyPath 
        {
            get
            {
                return Path.GetDirectoryName(Assembly
                    .GetExecutingAssembly().Location);
            }
        }

        /// <summary>
        /// Путь к системным файлам Lua в теневом хранилище Eplan
        /// </summary>
        public string SystemFilesPath 
        {
            get
            {
                return Path.Combine(AssemblyPath, luaFolder);
            }
        }

        /// <summary>
        /// Путь к системным файлам Lua по месту подключения надстройки
        /// </summary>
        public string OriginalSystemFilesPath 
        {
            get
            {
                return Path.Combine(OriginalAssemblyPath, luaFolder);
            }
        }

        private ProjectManager() { }

        private IEditor editor; /// Редактор технологических объектов.
        private ITechObjectManager techObjectManager; /// Менеджер технологических объектов.

        private IOManager IOManager; /// Менеджер модулей ввода/вывода
        private ProjectConfiguration projectConfiguration; // Конфигурация проекта

        private static ProjectManager instance; /// Экземпляр класса.         
    }
}
