using Eplan.EplApi.Base;
using Eplan.EplApi.DataModel;
using Eplan.EplApi.DataModel.EObjects;
using Eplan.EplApi.DataModel.Graphics;
using Eplan.EplApi.DataModel.MasterData;
using Eplan.EplApi.HEServices;
using Eplan.EplApi.HEServices.Exceptions;
using IO;
using LuaInterface;
using PInvoke;
using StaticHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace EasyEPlanner.ProjectImport
{
    [ExcludeFromCodeCoverage]
    public class ProjectImporter
    {
        /// <summary>
        /// Lua state
        /// </summary>
        private readonly Lua lua;

        /// <summary>
        /// Ширина текущего импортируемого узла
        /// </summary>
        private int currentNodeWidth;

        /// <summary>
        /// Путь каталога к макросам
        /// </summary>
        private string macrosPath = "";

        #region EPLAN - переменные
        /// <summary>
        /// Текущий проект
        /// </summary>
        private readonly Project project;

        /// <summary>
        /// Текущая редактируемая страница
        /// </summary>
        private Page currentPage;

        /// <summary>
        /// Свойства создания страницы
        /// </summary>
        private readonly PagePropertyList pageProperties = new PagePropertyList();

        /// <summary>
        /// Insert helper
        /// </summary>
        private readonly Insert Insert = new Insert();

        /// <summary>
        /// Объект для изменения имени функции
        /// </summary>
        private readonly NameService nameService = new NameService();
        #endregion

        #region Константы
        /// <summary>
        /// Lua-файл обработчик
        /// </summary>
        private static readonly string LUA_FILE_HANDLER = "sys_wago.lua";

        /// <summary>
        /// Название корневой страницы
        /// </summary>
        private const string ROOT_NAME = "ROOT";

        /// <summary>
        /// Название для шкафов управления
        /// </summary>
        private static readonly string CAB_NAME = "CAB";

        /// <summary>
        /// Описание для страницы с модулями ввода-вывода
        /// </summary>
        private const string PLC_BUS = "Шина ПЛК";

        /// <summary>
        /// Тип страницы для вставки макроса
        /// </summary>
        private static readonly WindowMacro.Enums.RepresentationType OVERVIEW = WindowMacro.Enums.RepresentationType.Overview;

        /// <summary>
        /// Вариант Е макроса - модуль на Overview
        /// </summary>
        private static readonly int MACRO_VARIANT_E = 4;

        /// <summary>
        /// Начальный отступ слева страницы для вставки узла и модулей (мм)
        /// </summary>
        private static readonly int X_OFFSET = 30;

        /// <summary>
        /// Верхняя точка страницы (мм)
        /// </summary>
        private static readonly int PAGE_HEIGHT = 292;

        /// <summary>
        /// Начальный отступ сверху страницы для вставки узла и модулей (Начало координат с низу страницы) (мм)
        /// </summary>
        private static readonly int Y_OFFSET = 30;

        /// <summary>
        /// Количество модулей в одной линии (мм)
        /// </summary>
        private static readonly int MODULES_IN_LINE = 25;

        /// <summary>
        /// Отступ между линиями модулей (мм)
        /// </summary>
        private static readonly int MODULES_LINE_OFFSET = 25;

        /// <summary>
        /// Ширина модулей (мм)
        /// </summary>
        private static readonly int MODULE_WIDTH = 12;

        /// <summary>
        /// Высота модулей (мм)
        /// </summary>
        private static readonly int MODULE_HEIGHT = 100;

        /// <summary>
        /// Индекс модуля заменяемого тип 2 в wprg4
        /// </summary>
        private static readonly int TYPE_2_NODE_N = 341;


        /// <summary>
        /// Индекс модуля заменяемого тип 3 в wprg4
        /// </summary>
        private static readonly int TYPE_3_NODE_N = 841;

        /// <summary>
        /// Индекс модуля заменяемого неопределенный тип в wprg4
        /// </summary>
        private static readonly int WOGO_DEFAULT_NODE_N = 352;
        #endregion

        public ProjectImporter(Project project, string WAGOFileName)
        {
            this.project = project;

            lua = new Lua();

            var path = Path.Combine(ProjectManager.GetInstance().SystemFilesPath, LUA_FILE_HANDLER);
            var script = File.ReadAllText(path);

            lua.RegisterFunction("Progress", null,
                typeof(Logs).GetMethod(nameof(Logs.SetProgress)));

            lua.DoString(script);

            var mainWago = new StreamReader(WAGOFileName, EncodingDetector.DetectFileEncoding(WAGOFileName), true).ReadToEnd();
            // Fix main.wago.plua '}'
            mainWago = Regex.Replace(mainWago, @"}(?=(\r|\n|\r\n)\t+group_dev_ex)", "},");
            // Remove unnecessary module
            mainWago = Regex.Replace(mainWago, "require 'sys_wago'", "");

            lua.DoString(mainWago);   
        }

        /// <summary>
        /// Функция импорта - запускает скрипт LUA
        /// </summary>
        public void Import()
        {
            macrosPath = ProjectManager.GetInstance().GetWagoMacrosPath();
            if (string.IsNullOrEmpty(macrosPath) || !Directory.Exists(macrosPath))
            {
                MessageBox.Show("Не найден путь каталога с макросами, пожалуйста, установите в файле configuration.ini" +
                    " в секции [path] свойство wago_macros_path, указывающее путь к папке с макросами.",
                    "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Logs.Clear();
            Logs.Show();

            pageProperties[Eplan.EplApi.DataModel.Properties.Page.DESIGNATION_PLANT] = ROOT_NAME;

            // call Import() function in lua
            lua.GetFunction("Import").Call(this);
            
            Logs.EnableButtons();
        }

        /// <summary>
        /// Импортировать узел (добавляет Макрос на ФСА)
        /// </summary>
        /// <param name="nodeType">Тип узла в wprg4.exe</param>
        /// <param name="nodeIndex">Порядковый номер узла</param>
        /// <param name="IP">IP-адрес узла</param>
        public bool ImportNode(int nodeType, int nodeIndex, string IP)
        {   
            pageProperties[Eplan.EplApi.DataModel.Properties.Page.DESIGNATION_LOCATION] = $"{CAB_NAME}{nodeIndex}";

            currentPage = new Page();

            currentPage.Create(project, DocumentTypeManager.DocumentType.Overview, pageProperties);
            currentPage.LockObject();
            currentPage.Properties.PAGE_NOMINATIOMN = PLC_BUS;

            int nodeN;
            switch (nodeType)
            {
                case 0: // 750-315 - RS-485               - UNDEFINDE MACROS
                case 2: // 750-341 - Ethernet coupler 
                    nodeN = TYPE_2_NODE_N;
                    break;

                case 1: // 750-815 - RS-485 (Programmable - UNDEFINDE MACROS
                case 3: // 750-841 - Ethernet (Programmable)
                    nodeN = TYPE_3_NODE_N;
                    break;

                default:
                    nodeN = WOGO_DEFAULT_NODE_N;
                    break;
            }

            var macro = OpenMacro(nodeN);

            var macroObjects = Insert.WindowMacro(macro,
                OVERVIEW, MACRO_VARIANT_E, currentPage,
                new PointD(X_OFFSET, PAGE_HEIGHT - Y_OFFSET),
                Insert.MoveKind.Absolute);


            var node = macroObjects?.OfType<PLC>().FirstOrDefault();
            if (node != null)
            {
                var nodeNumber = nodeIndex * 100;

                currentNodeWidth = node.Properties.RECTANGLE_WIDTH;
                try
                {
                    node.LockObject();
                    node.Properties.FUNC_PLCGROUP_STARTADDRESS = IP;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                nameService.SetVisibleNameAndAdjustFullName(
                    currentPage, node,
                    new FunctionPropertyList()
                    {
                        FUNC_CODE = "A",
                        FUNC_COUNTER = nodeNumber,
                    }, $"-A{nodeNumber}");

                Logs.AddMessage($"\n\n");
                Logs.AddMessage($"Импортирован узел -A{nodeNumber} [ 750-{nodeN} ]. Модули:\n");

                return true;
            }

            return false;
        }

        /// <summary>
        /// Открыть макрос WAGO по номеру модуля
        /// </summary>
        /// <param name="number">Идентификатор модуля</param>
        /// <returns>Макрос</returns>
        private WindowMacro OpenMacro(int number)
        {
            try
            {
                var macro = new WindowMacro();
                macro.Open(Path.Combine(macrosPath, $@"750-{number / 100 * 100}\WAGO.750-{number}.ema"), project, OVERVIEW, MACRO_VARIANT_E);
                return macro;
            }
            catch
            {
                Logs.Clear();
                Logs.Hide();
                MessageBox.Show($"Не удается найти макрос 750-{number}.ema, пожалуйста, правильно укажите в файле configuration.ini" +
                    " в секции [path] свойство wago_macros_path, указывающее путь к папке с макросами.",
                    "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return new WindowMacro();
        }

        /// <summary>
        /// Импортировать модуль (добавляет макрос на ФСА)
        /// </summary>
        /// <param name="moduleN">Идентификатор модуля</param>
        /// <param name="nodeIndex">Порядковый номер узла</param>
        /// <param name="moduleIndex">Порядковый номер модуля в шине</param>
        public bool ImportModule(int moduleN, int nodeIndex, int moduleIndex)
        {
            var macro = OpenMacro(moduleN);

            StorableObject[] objects =
                Insert.WindowMacro(macro,
                OVERVIEW, MACRO_VARIANT_E, currentPage,
                new PointD(
                    X_OFFSET + currentNodeWidth + (moduleIndex - 1) % MODULES_IN_LINE * MODULE_WIDTH,
                    PAGE_HEIGHT - Y_OFFSET - (moduleIndex - 1) / MODULES_IN_LINE * (MODULES_LINE_OFFSET + MODULE_HEIGHT)),
                Insert.MoveKind.Absolute);

            var module = objects?.OfType<PLC>().FirstOrDefault();
            if (module != null)
            {
                var moduleNumber = nodeIndex * 100 + moduleIndex;

                nameService.SetVisibleNameAndAdjustFullName(
                    currentPage, module,
                    new FunctionPropertyList()
                    {
                        FUNC_CODE = "A",
                        FUNC_COUNTER = moduleNumber,
                    }, $"-A{moduleNumber}");

                var moduleInfo = IOModuleInfo.GetModuleInfo($"750-{moduleN}", out var isStub);

                Logs.AddMessage($"\t-A{moduleNumber} [ {moduleInfo.Name} ] {(isStub ? "Неопределенный модуль" : moduleInfo.Description)};\n");
                
                return true;
            }

            return false;
        }
    }
}
