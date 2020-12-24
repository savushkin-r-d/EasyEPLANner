using Device;
using IO;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using TechObject;
using System.Threading;

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
                {
                    crc = (ushort)((crc & 0x8000) > 0 ? 
                        ((crc << 1) ^ 0x1021) : (crc << 1));
                }
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
                var t = new Thread(new ParameterizedThreadStart(
                    ProjectDescriptionSaver.Save));
                t.CurrentCulture = Thread.CurrentThread.CurrentCulture;
                t.Start(param);
            }
        }

        #region Загрузка описания проекта
        /// <summary>
        /// Считывание описания.
        /// </summary>
        public int LoadDescription(out string errStr,
            string projectName, bool loadFromLua)
        {
            errStr = "";
            Logs.Clear();
            EProjectManager.GetInstance().ProjectDataIsLoaded = false;

            string LuaStr;
            int res = 0;

            var oProgress = new Eplan.EplApi.Base.Progress("EnhancedProgress");
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
                    EncodingDetector.MainFilesEncoding = null;

                    oProgress.BeginPart(15, "Считывание технологических " +
                        "объектов");
                    res = LoadDescriptionFromFile(out LuaStr, out errStr, 
                        projectName, 
                        $"\\{ProjectDescriptionSaver.MainTechObjectsFileName}");
                    techObjectManager.LoadDescription(LuaStr, projectName);
                    errStr = "";
                    LuaStr = "";
                    res = LoadDescriptionFromFile(out LuaStr, out errStr, 
                        projectName,
                        $"\\{ProjectDescriptionSaver.MainRestrictionsFileName}");
                    techObjectManager.LoadRestriction(LuaStr);
                    oProgress.EndPart();
                }

                oProgress.BeginPart(15, "Проверка данных");
                projectConfiguration.Check();
                oProgress.EndPart();

                oProgress.BeginPart(15, "Расчет IO-Link");
                IOManager.CalculateIOLinkAdresses();
                oProgress.EndPart(true);
                EProjectManager.GetInstance().ProjectDataIsLoaded = true;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
                oProgress.EndPart(true);
                EProjectManager.GetInstance().ProjectDataIsLoaded = false;
            }

            return res;
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
            string path = GetPtusaProjectsPath(projectName) + projectName +
                fileName;

            try
            {
                if (!File.Exists(path))
                {
                    errStr = "Файл описания проекта \"" + path +
                        "\" отсутствует! Создано пустое описание.";
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    File.WriteAllText(path, string.Empty);
                }
            }
            catch (DriveNotFoundException)
            {
                errStr = "Укажите правильные настройки каталога!";
                return 1;
            }

            bool needEncoding = EncodingDetector.MainFilesEncoding == null &&
                fileName.Contains(ProjectDescriptionSaver
                .MainTechObjectsFileName);
            if (needEncoding)
            {
                EncodingDetector.MainFilesEncoding = EncodingDetector
                    .DetectFileEncoding(path);
            }

            sr = new StreamReader(path, EncodingDetector.MainFilesEncoding);
            LuaStr = sr.ReadToEnd();
            sr.Close();

            return 0;
        }
        #endregion

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
                    // Если не нашли - создаем новый, 
                    // записываем дефолтные данные
                    new PInvoke.IniFile(path);
                    StreamWriter sr = new StreamWriter(path, true);
                    sr.WriteLine("[path]\nfolder_path=");
                    sr.Close();
                    sr.Flush();
                }
                var iniFile = new PInvoke.IniFile(path);

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
                        "Пожалуйста, проверьте конфигурацию!", "Внимание",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    return firstPath + '\\';
                }
            }
            catch
            {
                MessageBox.Show("Файл конфигурации не найден - будет создан " +
                    "новый со стандартным описанием. Пожалуйста, измените " +
                    "путь к каталогу с проектами, где хранятся Lua файлы!",
                    "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return "";
        }

        /// <summary>
        /// Инициализация.
        /// </summary>
        public void Init()
        {
            CheckLibsAndFiles();

            editor = Editor.Editor.GetInstance();
            techObjectManager = TechObjectManager.GetInstance();
            Logs.Init(new LogFrm());           
            IOManager = IOManager.GetInstance();
            DeviceManager.GetInstance();
            projectConfiguration = ProjectConfiguration.GetInstance();
            EProjectManager.GetInstance();
            BaseTechObjectManager.GetInstance();
        }

        /// <summary>
        /// Проверка доступности библиотек и файлов для надстройки.
        /// </summary>
        private void CheckLibsAndFiles()
        {
            CheckExcelLibs();
            CopySystemFiles();
        }

        #region Генерация Excel
        /// <summary>
        /// Сохранение описания в виде таблицы Excel.
        /// </summary>
        public void SaveAsExcelDescription(string path)
        {
            var t = new System.Threading.Thread(
                new System.Threading.ParameterizedThreadStart(
                    ExportToExcel));

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
        #endregion

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

        #region Генерация базы каналов
        /// <summary>
        /// Экспорт из проекта базы каналов.
        /// </summary>
        public void SaveAsCDBX(string projectName, bool combineTag = false,
            bool useNewNames = false, bool rewrite = false)
        {
            Thread t = new Thread(
                new ParameterizedThreadStart(SaveAsXMLThread));

            var dataForSave = new DataForSaveAsXML(projectName, rewrite,
                combineTag, useNewNames);
            t.CurrentCulture = StaticHelper.CommonConst
                .CultureWithDotInsteadComma;
            t.Start(dataForSave);
        }

        /// <summary>
        /// Экспорт базы каналов, поток.
        /// </summary>
        /// <param name="param">Параметр потока</param>
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
                XMLReporter.SaveAsXML(data.pathToFile, data.rewrite,
                    data.cdbxTagView, data.cdbxNewNames);
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
            public DataForSaveAsXML(string pathToFile, bool rewrite,
                bool cdbxTagView, bool cdbxNewNames)
            {
                this.pathToFile = pathToFile;
                this.rewrite = rewrite;
                this.cdbxNewNames = cdbxNewNames;
                this.cdbxTagView = cdbxTagView;
            }

            public string pathToFile;
            public bool rewrite;
            public bool cdbxTagView;
            public bool cdbxNewNames;
        }
#endregion

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
        /// Редактирование технологических объектов. Новое дерево.
        /// </summary>
        /// <returns>Результат редактирования</returns>
        public void StartEdit()
        {
            editor
                .OpenEditor(techObjectManager as Editor.ITreeViewItem);
        }

        #region Подсветка объектов на схеме
        /// <summary>
        /// Участвующие в операции устройства, подсвеченные на карте Eplan.
        /// </summary>
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

        /// <summary>
        /// Установка подсветки устройств
        /// </summary>
        /// <param name="objectsToDraw">Устройства для подсветки</param>
        public void SetHighLighting(object objectsToDraw)
        {
            if (objectsToDraw == null)
            {
                return;
            }

            if(objectsToDraw is List<Editor.DrawInfo> drawInfoNew)
            {
                SetHighlighting(drawInfoNew);
            }
        }

        /// <summary>
        /// Подсветка из нового редактора
        /// </summary>
        /// <param name="objectsToDraw"></param>
        private void SetHighlighting(List<Editor.DrawInfo> objectsToDraw)
        {
            foreach (Editor.DrawInfo drawObj in objectsToDraw)
            {
                Editor.DrawInfo.Style howToDraw = drawObj.DrawingStyle;

                if (howToDraw == Editor.DrawInfo.Style.NO_DRAW)
                {
                    continue;
                }

                Eplan.EplApi.DataModel.Function objectFunction =
                    (drawObj.DrawingDevice as IODevice).EplanObjectFunction;

                if (objectFunction == null)
                {
                    continue;
                }

                Eplan.EplApi.Base.PointD[] points = objectFunction
                    .GetBoundingBox();
                short colour = 0;
                switch (howToDraw)
                {
                    case Editor.DrawInfo.Style.GREEN_BOX:
                        SetGreenBoxHighlight(ref colour, objectFunction,
                            points);
                        break;

                    case Editor.DrawInfo.Style.RED_BOX:
                        SetRedBoxHiglight(ref colour);
                        break;

                    case Editor.DrawInfo.Style.GREEN_UPPER_BOX:
                        SetGreenUpperBoxHighlight(ref colour, points);
                        break;

                    case Editor.DrawInfo.Style.GREEN_LOWER_BOX:
                        SetGreenLowerBoxHighlight(ref colour, points);
                        break;

                    case Editor.DrawInfo.Style.GREEN_RED_BOX:
                        SetGrenRedBoxHiglight(ref colour, objectFunction,
                            points);
                        break;
                }

                AddBoxForHighlighting(colour, objectFunction, points);
            }
        }

        /// <summary>
        /// Настроить как зеленый прямоугольник.
        /// </summary>
        /// <param name="colour">Цвет</param>
        /// <param name="oF">Функция объекта</param>
        /// <param name="points">Точки</param>
        private void SetGreenBoxHighlight(ref short colour, 
            Eplan.EplApi.DataModel.Function oF,
            Eplan.EplApi.Base.PointD[] points)
        {
            colour = 3; //Green.

            //Для сигналов подсвечиваем полностью всю линию.
            if (oF.Name.Contains("DI") || oF.Name.Contains("DO"))
            {
                if (oF.Connections.Length > 0)
                {
                    points[1].X = oF.Connections[0].StartPin
                        .ParentFunction.GetBoundingBox()[1].X;
                }
            }
        }
        
        /// <summary>
        /// Настроить как красный прямоугольник
        /// </summary>
        /// <param name="colour">Цвет</param>
        private void SetRedBoxHiglight(ref short colour)
        {
            colour = 252; //Red.
        }

        /// <summary>
        /// Настроить как половина зеленого прямоугольника сверху
        /// </summary>
        /// <param name="colour">Цвет</param>
        /// <param name="points">Точки</param>
        private void SetGreenUpperBoxHighlight(ref short colour,
            Eplan.EplApi.Base.PointD[] points)
        {
            points[0].Y += (points[1].Y - points[0].Y) / 2;
            colour = 3; //Green.
        }

        /// <summary>
        /// Настроить как половина зеленого прямоугольника снизу
        /// </summary>
        /// <param name="colour">Цвет</param>
        /// <param name="points">Точки</param>
        private void SetGreenLowerBoxHighlight(ref short colour,
            Eplan.EplApi.Base.PointD[] points)
        {
            points[1].Y -= (points[1].Y - points[0].Y) / 2;
            colour = 3; //Green.
        }

        /// <summary>
        /// Настроить как зелено-серый прямоугольник
        /// </summary>
        /// <param name="colour">Цвет</param>
        /// <param name="oF">Функция объекта</param>
        /// <param name="points">Точки</param>
        private void SetGrenRedBoxHiglight(ref short colour,
            Eplan.EplApi.DataModel.Function oF,
            Eplan.EplApi.Base.PointD[] points)
        {
            var rc2 = new Eplan.EplApi.DataModel.Graphics.Rectangle();
            rc2.Create(oF.Page);
            rc2.IsSurfaceFilled = true;
            rc2.DrawingOrder = 1;
            rc2.SetArea(new Eplan.EplApi.Base.PointD(points[0].X,
                points[0].Y + (points[1].Y - points[0].Y) / 2),
                points[1]);

            rc2.Pen = new Eplan.EplApi.DataModel.Graphics.Pen(
                252 /*Red*/, -16002, -16002, -16002, 0);

            rc2.Properties.set_PROPUSER_TEST(1,
                oF.ToStringIdentifier());
            highlightedObjects.Add(rc2);

            points[1].Y -= (points[1].Y - points[0].Y) / 2;
            colour = 3; //Green.
        }

        /// <summary>
        /// Добавить прямоугольник в подсвечиваемые элементы
        /// </summary>
        /// <param name="colour">Цвет</param>
        /// <param name="objectFunction">Функция объекта</param>
        /// <param name="points">Точки</param>
        private void AddBoxForHighlighting(short colour,
            Eplan.EplApi.DataModel.Function objectFunction,
            Eplan.EplApi.Base.PointD[] points)
        {
            var rc = new Eplan.EplApi.DataModel.Graphics.Rectangle();
            rc.Create(objectFunction.Page);
            rc.IsSurfaceFilled = true;
            rc.DrawingOrder = -1;
            rc.SetArea(points[0], points[1]);
            rc.Pen = new Eplan.EplApi.DataModel.Graphics.Pen(colour, -16002,
                -16002, -16002, 0);
            rc.Properties.set_PROPUSER_TEST(1, objectFunction
                .ToStringIdentifier());
            highlightedObjects.Add(rc);
        }
        #endregion

        #region OSTIS
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
        #endregion

        /// <summary>
        /// Проверить Excel библиотеки надстройки.
        /// </summary>
        private void CheckExcelLibs()
        {
            const string spireLicense = "Spire.License.dll";
            const string spireXLS = "Spire.XLS.dll";
            const string spirePDF = "Spire.Pdf.dll";

            string SpireLicensePath = Path.Combine(AssemblyPath, spireLicense);
            string SpireXLSPath = Path.Combine(AssemblyPath, spireXLS);
            string SpirePDFPath = Path.Combine(AssemblyPath, spirePDF);

            if (File.Exists(SpireLicensePath) == false ||
                File.Exists(SpireXLSPath) == false ||
                File.Exists(SpirePDFPath) == false)
            {
                var files = new string[] { spireLicense, spireXLS, spirePDF };
                CopySpireXLSFiles(AssemblyPath, files, OriginalAssemblyPath);
            }
        }

        /// <summary>
        /// Копировать файлы библиотек Spire XLS
        /// </summary>
        /// <param name="shadowAssemblySpireFilesDir">Путь к библиотекам
        /// в теневом хранилище Eplan</param>
        /// <param name="files">Имена файлов для копирования</param>
        /// <param name="originalPath">Путь к надстройке из каталога
        /// подключения надстройки</param>
        private void CopySpireXLSFiles(string shadowAssemblySpireFilesDir,
            string[] files, string originalPath)
        {
            var libsDir = new DirectoryInfo(originalPath);
            foreach (FileInfo file in libsDir.GetFiles())
            {
                if (files.Contains(file.Name))
                {
                    string path = Path.Combine(shadowAssemblySpireFilesDir,
                        file.Name);
                    file.CopyTo(path, true);
                }
            }
        }

        /// <summary>
        /// Копирует системные .lua файлы если они не загрузились
        /// в теневое хранилище (Win 7 fix).
        /// <param name="systemFilesPath">Путь к Lua файлам
        /// в теневом хранилище Eplan</param>
        /// <param name="originalSystemFilesPath">Путь к файлам Lua в месте 
        /// подключения надстройки к программе</param>
        /// </summary>
        private void CopySystemFiles()
        {
            Directory.CreateDirectory(SystemFilesPath);

            var systemFilesDir = new DirectoryInfo(OriginalSystemFilesPath);
            FileInfo[] systemFiles = systemFilesDir.GetFiles();
            foreach (FileInfo systemFile in systemFiles)
            {
                string pathToFile = Path.Combine(SystemFilesPath,
                    systemFile.Name);
                systemFile.CopyTo(pathToFile, true);
            }
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
        /// Папка с скриптами командой строки
        /// </summary>
        private const string cmdScriptsFolder = "CMD";

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

        /// <summary>
        /// Путь к файлам с скриптами командной строки для проверки проекта по
        /// месту подключения надстройки
        /// </summary>
        public string OriginalCMDFilesPath
        {
            get
            {
                return Path.Combine(OriginalAssemblyPath, cmdScriptsFolder);
            }
        }

        private ProjectManager() { }

        /// <summary>
        /// Редактор технологических объектов.
        /// </summary>
        private Editor.IEditor editor;

        /// <summary>
        /// Менеджер технологических объектов.
        /// </summary>
        private ITechObjectManager techObjectManager;

        /// <summary>
        /// Менеджер модулей ввода/вывода.
        /// </summary>
        private IOManager IOManager;

        /// <summary>
        /// Конфигурация проекта.
        /// </summary>
        private ProjectConfiguration projectConfiguration;

        /// <summary>
        /// Экземпляр класса ProjectManager
        /// </summary>
        private static ProjectManager instance;      
    }
}
