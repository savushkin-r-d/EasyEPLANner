using Eplan.EplApi.Base;
using Eplan.EplApi.DataModel;
using Eplan.EplApi.DataModel.Graphics;
using Eplan.EplApi.DataModel.MasterData;
using Eplan.EplApi.HEServices;
using LuaInterface;
using StaticHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyEPlanner.ProjectImportICP
{
    public interface IDevicesImporter
    {
        /// <summary>
        /// Функция импорта - запускает скрипт LUA
        /// </summary>
        void Import();

        /// <summary>
        /// [[ LuaMember ]] - вызывается из Lua
        /// 
        /// Импортировать устройство ввода-вывода
        /// </summary>
        /// <remarks>
        /// Импортируется без создания страниц EPLAN.
        /// Схемы генерируются отдельно при запуске <see cref="GenerateDevicesPages"/>
        /// </remarks>
        /// <param name="type">Тип устройства</param>
        /// <param name="number">Номер устройства</param>
        /// <param name="subtype">Подтип</param>
        /// <param name="description">Описание</param>
        /// <returns>Описание импортированного устройства: описание дополняется в Lua</returns>
        IImportDevice ImportDevice(string type, int number, string subtype, string description);

        /// <summary>
        /// [[ LuaMember ]] - вызывается из Lua
        /// 
        /// Генерация страниц EPLAN с устройствами
        /// </summary>
        void GenerateDevicesPages();

        /// <summary>
        /// Список импортированных устройств
        /// </summary>
        List<ImportDevice> ImportDevices { get; }
    }

    [ExcludeFromCodeCoverage]
    public class DevicesImporter : IDevicesImporter
    {
        #region константы
        /// <summary>
        /// Название lua-файла обработчика
        /// </summary>
        private static readonly string LUA_FILE_HANDLER = "sys_wago_device_importer.lua";

        /// <summary>
        /// Название корневой страницы
        /// </summary>
        private static readonly string ROOT_NAME = "ROOT";

        /// <summary>
        /// Название страницы с сигналами
        /// </summary>
        private static readonly string SIGNALS_PAGE_NAME = "Сигналы";

        /// <summary>
        /// Начальная позиция по X на странице для устройств (левый верхний угол)
        /// </summary>
        private static readonly int X_START = 20;

        /// <summary>
        /// Начальная позиция по Y на странице для устройств (левый верхний угол)
        /// </summary>
        private static readonly int Y_START = 270;

        /// <summary>
        /// Отступ между сигналами по вертикали
        /// </summary>
        private static readonly int SIGNAL_Y_OFFSET = 20;

        /// <summary>
        /// Отступ между сигналами по горизонтали
        /// </summary>
        private static readonly int SIGNAL_X_OFFSET = 60;

        /// <summary>
        /// Вариант символа сигнала <|
        /// </summary>
        private static readonly int SIGNAL_DI = 0;

        /// <summary>
        /// Вариант символа сигнала |>
        /// </summary>
        private static readonly int SIGNAL_DO = 2;

        /// <summary>
        /// Вариант символа заглушки сигнала <|--*
        /// </summary>
        private static readonly int SIGNAL_STUB_DI = 5;

        /// <summary>
        /// Вариант символа заглушки сигнала *--|>
        /// </summary>
        private static readonly int SIGNAL_STUB_DO = 7;

        /// <summary>
        /// Ширина сигнала
        /// </summary>
        private static readonly int SIGNAL_WIDTH = 20;

        /// <summary>
        /// Отступ описания сигнала
        /// </summary>
        private static readonly int SIGNAL_DESCRIPTION_Y_OFFSET = 10;

        /// <summary>
        /// Количество сигналов в одной колонке
        /// </summary>
        private static readonly int SIGNALS_IN_COLUMN = 12;

        /// <summary>
        /// Высота текста
        /// </summary>
        private static readonly double TEXT_HEIGHT = 1.5;

        /// <summary>
        /// Отступ между устройствами
        /// </summary>
        private static readonly int DEVICE_OFFSET = 40;

        /// <summary>
        /// Название страницы с устройствами
        /// </summary>
        private static readonly string DEVICES_PAGE_NAME = "Устройства";

        /// <summary>
        /// Количество устройств в строке
        /// </summary>
        private static readonly int DEVICES_IN_ROW = 10;

        /// <summary>
        /// Количество устройств в строке
        /// </summary>
        private static readonly int DEVICES_IN_COLUMN = 7;

        /// <summary>
        /// Смещение по X полного названия устройства из проекта ICP-CON
        /// </summary>
        private static readonly int DEVICE_FULL_WAGO_NAME_X_OFFSET = -7;

        /// <summary>
        /// Смещение по Y полного названия устройства из проекта ICP-CON
        /// </summary>
        private static readonly int DEVICE_FULL_WAGO_NAME_Y_OFFSET = 7;

        /// <summary>
        /// Смещение по X описания устройства
        /// </summary>
        private static readonly int DEVICE_DESCRIPTION_X_OFFSET = -20;

        /// <summary>
        /// Смещение по Y описания устройства
        /// </summary>
        private static readonly int DEVICE_DESCRIPTION_Y_OFFSET = -10;
        #endregion

        #region Объекты EPLAN

        /// <summary>
        /// Проект EPLAN
        /// </summary>
        private readonly Project project;

        /// <summary>
        /// Свойства создания страницы
        /// </summary>
        private readonly PagePropertyList pageProperties = new PagePropertyList();

        /// <summary>
        /// Символ сигнала
        /// </summary>
        private readonly Symbol signalSymbol;

        /// <summary>
        /// Символ устройства
        /// </summary>
        private readonly Symbol deviceSymbol;

        /// <summary>
        /// Символ заглушки сигнала
        /// </summary>
        private readonly Symbol stubSignalSymbol;

        /// <summary>
        /// Объект для изменения имени функции
        /// </summary>
        private readonly NameService nameService = new NameService();
        #endregion

        /// <summary>
        /// Lua state
        /// </summary>
        private readonly Lua lua;

        /// <summary>
        /// Список номеров танков проекта
        /// </summary>
        private readonly List<int> tanks = new List<int>();


        public DevicesImporter(Project project, string wagoData, string icpProjectData)
        {
            this.project = project;

            try
            {
                signalSymbol = new Symbol(new SymbolLibrary(project, "Bmk"), "AUS_01");
                deviceSymbol = new Symbol(new SymbolLibrary(project, "Bmk"), "ED_01");
            }
            catch
            {
                Logs.AddMessage("Не удается открыть библиотеку символов \"Bmk\";\n");
            }

            try
            {
                stubSignalSymbol = new Symbol(new SymbolLibrary(project, "SPECIAL"), "DCPPNG");
            }
            catch
            {
                Logs.AddMessage("Не удается открыть библиотеку символов \"SPECIAL\";\n");
            }

            pageProperties[Eplan.EplApi.DataModel.Properties.Page.DESIGNATION_PLANT] = ROOT_NAME;

            lua = new Lua();

            var path = Path.Combine(ProjectManager.GetInstance().SystemFilesPath, LUA_FILE_HANDLER);
            var script = File.ReadAllText(path);

            lua.RegisterFunction("Progress", null,
                typeof(Logs).GetMethod(nameof(Logs.SetProgress)));

            lua.DoString(script);
            lua.DoString(wagoData);

            // Получение информации о танках проекта
            foreach (Match match in Regex.Matches(icpProjectData, @"Tank[\W\w]+?(\d*) - номер объекта", RegexOptions.Multiline, TimeSpan.FromMilliseconds(100)))
            {
                if (match.Success && int.TryParse(match.Groups[1].Value, out var tank))
                {
                    tanks.Add(tank);
                }
            }
        }

        public void Import()
        {
            Logs.Clear();
            Logs.Show();

            Logs.AddMessage("\n\n");
            Logs.AddMessage("Импорт устройств:\n");

            // call Import() function in lua
            lua.GetFunction("Import").Call(this);

            Logs.EnableButtons();
        }


        public List<ImportDevice> ImportDevices { get; private set; } = new List<ImportDevice>();


        public IImportDevice ImportDevice(string type, int number, string subtype, string description)
        {
            var dev = new ImportDevice()
            {
                Type = type,
                FullNumber = number,
                Number = number,
                Subtype = subtype,
                Description = description
            };

            // Если в проекте есть подходящий номеру танк
            if (tanks.Contains(number / 100))
            {
                dev.Number = number % 100;
                dev.Object = $"TANK{number / 100}";
            }

            // Номер устройства подходит под нумерацию линий A-W (220-239 A; 640-659 W)
            if (number > 20000 && number < 66000)
            {
                dev.Number = number % 100; // номер устройства 241(XX)
                var objectGroup = number / 100; // группа объекта (XXX)01
                dev.Object = $"{(char)('A' + (objectGroup - 200) / 20)}{objectGroup % 20}";
            }


            Logs.AddMessage($"\t{dev.Type}{dev.FullNumber} -> {dev.Object}{dev.Type}{dev.Number} - \"{dev.Description}\"\n");

            ImportDevices.Add(dev);

            return dev;
        }

        public void GenerateDevicesPages()
        {
            foreach (var Object in ImportDevices.GroupBy(d => d.Object))
            {
                var devices = Object.ToList();

                if (string.IsNullOrEmpty(Object.Key))
                { // Генерация страницы с сигналами DO/DO без объекта
                    var signals = devices.Where(d => d.Type == "DO" || d.Type == "DI").ToList();
                    devices = devices.Except(signals).ToList();

                    GenerateSignalsPage(signals);
                }

                GenerateDevicesPage(devices, Object.Key);
            }
        }


        /// <summary>
        /// Генерация страницы с сигналами( типы DI/DO без объекта)
        /// </summary>
        /// <param name="signals">Список сигналов</param>
        private void GenerateSignalsPage(List<ImportDevice> signals)
        {
            var page = CreatePage(SIGNALS_PAGE_NAME);

            var X = X_START;
            var Y = Y_START;

            foreach (var device in signals)
            {
                var symbolVariant = new SymbolVariant(signalSymbol, device.Type == "DI" ? SIGNAL_DI : SIGNAL_DO);
                var function = new Function();

                function.Create(page, symbolVariant);
                function.LockObject();

                nameService.SetVisibleNameAndAdjustFullName(page, function,
                    new FunctionPropertyList()
                    {
                        FUNC_CODE = device.Type,
                        FUNC_COUNTER = device.Number,
                    }, $"-{device.Type}{device.Number}");


                new ApiHelper().SetSupplementaryFieldValue(function, 1, device.Subtype);
                function.Properties.FUNC_COMMENT = device.Description;
                function.Location = new PointD(X + (device.Type == "DI" ? 0 : SIGNAL_WIDTH), Y);

                // Заглушка для соединителя сигналов *---|>  <|---*
                var symbolVariantStub = new SymbolVariant(stubSignalSymbol, device.Type == "DI" ? SIGNAL_STUB_DI : SIGNAL_STUB_DO);
                var functionStub = new Function();

                functionStub.Create(page, symbolVariantStub);
                functionStub.LockObject();

                functionStub.Properties.FUNC_MAINFUNCTION = false;
                functionStub.Location = new PointD(X + (device.Type == "DI" ? SIGNAL_WIDTH : 0 ), Y);


                // Add Description Signature
                var description = new Text();
                description.Create(page, $"{device.Description}", TEXT_HEIGHT);
                description.LockObject();
                description.Location = new PointD(X, Y - SIGNAL_DESCRIPTION_Y_OFFSET);

                Y -= SIGNAL_Y_OFFSET;

                // Проверка количества сигналов в колонке
                if (Y == Y_START - SIGNAL_Y_OFFSET * SIGNALS_IN_COLUMN)
                {
                    X += SIGNAL_X_OFFSET;
                    Y = Y_START;
                }
            }
        }


        /// <summary>
        /// Генерация страницы с устройствами
        /// </summary>
        /// <param name="devices">Список устройств</param>
        /// <param name="objectName">Название объекта</param>
        private void GenerateDevicesPage(List<ImportDevice> devices, string objectName)
        {
            var page = CreatePage(DEVICES_PAGE_NAME, objectName);

            var X = X_START;
            var Y = Y_START;

            foreach (var device in devices)
            {
                var symbolVariant = new SymbolVariant(deviceSymbol, 0);
                var function = new Function();

                function.Create(page, symbolVariant);
                function.LockObject();

                nameService.SetVisibleNameAndAdjustFullName(page, function,
                    new FunctionPropertyList()
                    {
                        FUNC_CODE = device.Type,
                        FUNC_COUNTER = device.Number,
                    }, $"-{device.Type}{device.Number}");

                new ApiHelper().SetSupplementaryFieldValue(function, 1, device.Subtype);
                function.Properties.FUNC_COMMENT = device.Description;
                function.Location = new PointD(X, Y);

                // Add signature of full wago name
                var fullName = new Text();
                fullName.Create(page, $"{device.Type}{device.FullNumber}", TEXT_HEIGHT);
                fullName.LockObject();
                fullName.Location = new PointD(X + DEVICE_FULL_WAGO_NAME_X_OFFSET, Y + DEVICE_FULL_WAGO_NAME_Y_OFFSET);

                // Add description signature 
                var description = new Text();
                description.Create(page, device.Description, TEXT_HEIGHT);
                description.LockObject();
                description.Location = new PointD(X + DEVICE_DESCRIPTION_X_OFFSET, Y + DEVICE_DESCRIPTION_Y_OFFSET);


                X += DEVICE_OFFSET;
                
                // Проверка на количество устройств в ряду
                if (X == X_START + DEVICE_OFFSET * DEVICES_IN_ROW)
                {
                    X = X_START;
                    Y -= DEVICE_OFFSET;
                }

                // Проверка на количество устройств в колонке
                if (Y == Y_START - DEVICE_OFFSET * DEVICES_IN_COLUMN)
                {
                    X = X_START;
                    Y = Y_START;

                    // Create new page
                    page = CreatePage(DEVICES_PAGE_NAME, objectName);
                }
            }
        }

        
        /// <summary>
        /// Создать новую страницу
        /// </summary>
        /// <param name="name">Название</param>
        /// <param name="location">Локация</param>
        /// <returns></returns>
        private Page CreatePage(string name, string location = "")
        {
            var page = new Page();

            pageProperties[Eplan.EplApi.DataModel.Properties.Page.DESIGNATION_LOCATION] = location;

            page.Create(project, DocumentTypeManager.DocumentType.ProcessAndInstrumentationDiagram, pageProperties);
            page.LockObject();
            page.Properties.PAGE_NOMINATIOMN = $"{name} {location}".Trim();

            return page;
        }
    }
}
