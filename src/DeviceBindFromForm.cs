using Aga.Controls.Tree;
using Eplan.EplApi.Base;
using Eplan.EplApi.DataModel;
using Eplan.EplApi.HEServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace EasyEPlanner
{
    /// <summary>
    /// Класс, реализующий привязку канала устройства к модулю I/O
    /// </summary>
    public sealed class DeviceBind
    {
        /// <summary>
        /// Конструктор, в который передается экземпляр формы для доступа
        /// к объектам формы.
        /// </summary>
        /// <param name="devicesForm">Форма с объектами</param>
        public DeviceBind(DFrm devicesForm)
        {
            this.DevicesForm = devicesForm;
            this.startValues = new StartValues(DevicesForm);
        }

        /// <summary>
        /// Закрытый конструктор для безопасности использования
        /// </summary>
        private DeviceBind() { }

        /// <summary>
        /// Привязать выбранный канал устройства к модулю ввода-вывода
        /// </summary>
        public void Bind()
        {
            try
            {
                InitStartValues();
                PrepareFunctionalText();

                if (!string.IsNullOrEmpty(ResetDevicesChannel))
                {
                    ResetChannel();
                }

                if (!string.IsNullOrEmpty(SetDevicesChannel))
                {
                    BindChannel();
                }

                RefreshTree();
            }
            catch
            {
                // TODO: Errors handler
                return;
            }
        }

        /// <summary>
        /// Инициализация базовых значений переменных
        /// необходимых для привязки
        /// </summary>
        private void InitStartValues()
        {
            try
            {
                SelectedNode = startValues.GetSelectedNode();
                NodeFromSelectedNode = startValues
                    .GetNodeFromSelectedNode(SelectedNode);
                SelectedChannel = startValues
                    .GetChannel(NodeFromSelectedNode);
                SelectedDevice = startValues
                    .GetDevice(NodeFromSelectedNode);
                SelectedObject = startValues.GetSelectedObject();
                SelectedClampFunction = startValues
                    .GetSelectedClampFunction(SelectedObject);
                SelectedIOModuleFunction = startValues
                    .GetSelectedIOModuleFunction(SelectedClampFunction);
            }
            catch
            {
                const string Message = "Ошибка при инициализации " +
                    "базовых значений";
                throw new Exception(Message);
            }
        }

        /// <summary>
        /// Подготовка функционального текста для записи в функцию
        /// </summary>
        private void PrepareFunctionalText()
        {
            bool isIOLink = CheckIOLink();

            if (isIOLink)
            {
                string channelType = GetIOLinkChannelType();
                NewFunctionalText = SelectedDevice.EPlanName +
                    "\r\n" + SelectedDevice.Description +
                    "\r\n" + SelectedChannel.komment +
                    channelType;
            }
            else
            {
                NewFunctionalText = SelectedDevice.EPlanName +
                    "\r\n" + SelectedDevice.Description +
                    "\r\n" + SelectedChannel.komment;
            }

            var oldFunctionalText = SelectedClampFunction
                .Properties.FUNC_TEXT_AUTOMATIC
                .ToString(ISOCode.Language.L___);
            if (string.IsNullOrEmpty(oldFunctionalText))
            {
                oldFunctionalText = SelectedClampFunction.Properties
                    .FUNC_TEXT_AUTOMATIC
                    .ToString(ISOCode.Language.L_ru_RU);
            }

            if (SelectedClampFunction.Properties.FUNC_TEXT.IsEmpty ||
                SelectedClampFunction.Properties.FUNC_TEXT == "Резерв")
            {
                //Если нет функционального текста, устанавливаем его.
                if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                {
                    //С Ctrl устанавливаем только название устройства.
                    NewFunctionalText = SelectedDevice.EPlanName;
                }

                SetDevicesChannel = NewFunctionalText;
            }
            else
            {
                //Замена на "Резерв".
                if (SelectedClampFunction.Properties.FUNC_TEXT ==
                    NewFunctionalText)
                {
                    ResetDevicesChannel = NewFunctionalText;
                    NewFunctionalText = "Резерв";
                }
                else
                {
                    if ((Control.ModifierKeys & Keys.Control) ==
                        Keys.Control)
                    {
                        if (!(oldFunctionalText + "\r\n").
                            Contains(SelectedChannel.komment + "\r\n"))
                        {
                            MessageBox.Show(
                                "Действие канала устройства (\"" +
                                SelectedChannel.komment + "\") " +
                                "отличается от действия уже " +
                                "привязанного канала модуля!",
                                "EPlaner",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);

                            return;
                        }

                        if (oldFunctionalText.Contains(SelectedDevice
                            .EPlanName))
                        {
                            ResetDevicesChannel = NewFunctionalText;
                            NewFunctionalText = oldFunctionalText
                                .Replace(SelectedDevice.EPlanName, "")
                                .Trim();

                            if (NewFunctionalText.Length > 0)
                            {
                                //Если строка начинается не с символа "+",
                                //заменяем ее на "Резерв".
                                if (NewFunctionalText[0] != '+')
                                {
                                    NewFunctionalText = "Резерв";
                                }
                            }
                        }
                        else
                        {
                            SetDevicesChannel = NewFunctionalText;
                            var cursorPosition = oldFunctionalText
                                .IndexOf('\r');
                            if (cursorPosition < 0)
                            {
                                NewFunctionalText = oldFunctionalText +
                                    "\r\n" + SelectedDevice.EPlanName;
                            }
                            else
                            {
                                NewFunctionalText = oldFunctionalText.
                                    Insert(cursorPosition,
                                    "\r\n" + SelectedDevice.EPlanName);
                            }
                        }
                    }
                    else
                    {
                        //Замена на новое устройство.
                        ResetDevicesChannel = oldFunctionalText;
                        SetDevicesChannel = NewFunctionalText;
                    }
                }
            }
        }

        /// <summary>
        /// Очистить канал от старой привязки
        /// </summary>
        private void ResetChannel()
        {
            var name = string.Empty;

            if (SelectedIOModuleFunction != null &&
                SelectedIOModuleFunction.Articles.Count() > 0 &&
                !SelectedIOModuleFunction.Articles[0]
                .Properties[Eplan.EplApi.DataModel.Properties
                .Article.ARTICLE_TYPENR].IsEmpty)
            {
                name = SelectedIOModuleFunction.Articles[0]
                    .Properties[Eplan.EplApi.DataModel.Properties
                    .Article.ARTICLE_TYPENR];
            }

            var moduleInfo = new IO.IOModuleInfo();
            moduleInfo = moduleInfo.GetIOModuleInfo(name, out _);

            Dictionary<string, string> devicesComments =
                EplanDeviceManager.GetAssigment(SelectedClampFunction,
                moduleInfo, ResetDevicesChannel);

            foreach (KeyValuePair<string, string> pair in devicesComments)
            {
                var deviceName = pair.Key;
                var deviceComment = pair.Value;

                var device = Device.DeviceManager.GetInstance()
                    .GetDevice(deviceName);

                device.ClearChannel(moduleInfo.AddressSpaceType,
                    deviceComment);
            }
        }

        /// <summary>
        /// Привязать канал
        /// </summary>
        private void BindChannel()
        {
            if (SelectedIOModuleFunction == null)
            {
                return;
            }

            var regex = new Regex(IOModuleNamePattern);
            var match = regex.Match(SelectedIOModuleFunction.VisibleName);
            if (match.Success == false)
            {
                return;
            }

            var name = string.Empty;
            if (SelectedIOModuleFunction != null &&
                SelectedIOModuleFunction.Articles.Count() > 0 &&
                !SelectedIOModuleFunction.Articles[0]
                .Properties[Eplan.EplApi.DataModel.Properties
                .Article.ARTICLE_TYPENR].IsEmpty)
            {
                name = SelectedIOModuleFunction.Articles[0]
                    .Properties[Eplan.EplApi.DataModel
                    .Properties.Article.ARTICLE_TYPENR];
            }

            var deviceNumber = Convert.ToInt32(match.Groups["n"].Value);
            var moduleNumber = deviceNumber % 100;
            int nodeNumber;
            var clampNumber = SelectedClampFunction.Properties
                .FUNC_ADDITIONALIDENTIFYINGNAMEPART.ToInt();

            // Есть ли PXC A1 в проекте
            if (IO.IOManager.GetInstance().IONodes[0].Type ==
                IO.IONode.TYPES.T_PHOENIX_CONTACT_MAIN)
            {
                // Если есть "A1", то учитываем, что он первый
                nodeNumber = deviceNumber / 100;
            }
            else
            {
                // Если нету, оставляем как было
                nodeNumber = deviceNumber / 100 - 1;
            }

            var moduleInfo = new IO.IOModuleInfo();
            moduleInfo = moduleInfo.GetIOModuleInfo(name, out _);
            var logicalPort = Array.IndexOf(moduleInfo.ChannelClamps,
                clampNumber) + 1;
            var moduleOffset = IO.IOManager.GetInstance()
                .IONodes[nodeNumber].
                IOModules[moduleNumber - 1].InOffset;

            SelectedChannel.SetChannel(nodeNumber, moduleNumber,
                clampNumber, deviceNumber, logicalPort, moduleOffset);
        }

        /// <summary>
        /// Наличие IO-Link у модуля
        /// </summary>
        /// <returns></returns>
        private bool CheckIOLink()
        {
            var isIOLink = false;
            var IOModuleFunctionName = SelectedClampFunction
                .ParentFunction.VisibleName;
            var IOModuleMatch = Regex.Match(IOModuleFunctionName,
                IOModuleNamePattern);

            if (IOModuleMatch.Success == false)
            {
                return isIOLink;
            }

            var IOModuleNumber = Convert.ToInt32(IOModuleMatch
                .Groups["n"].Value);

            var IOManager = IO.IOManager.GetInstance();
            foreach (IO.IONode node in IOManager.IONodes)
            {
                foreach (IO.IOModule module in node.IOModules)
                {
                    if (module.PhysicalNumber == IOModuleNumber)
                    {
                        isIOLink = module.isIOLink();
                    }
                }
            }

            return isIOLink;
        }

        /// <summary>
        /// Получить тип канала для IO-Link модуля
        /// </summary>
        /// <returns></returns>
        private string GetIOLinkChannelType()
        {
            const string AI = "AI";
            const string AO = "AO";
            const string IOLink = "IO-Link";

            var name = SelectedChannel.name;
            if (name == AI || name == AO)
            {
                return IOLink;
            }
            else
            {
                return name;
            }
        }

        /// <summary>
        /// Обновление дерева устройств после привязки
        /// </summary>
        private void RefreshTree()
        {
            DevicesForm.RefreshTreeAfterBinding();
        }

        #region Закрытые поля
        /// <summary>
        /// Выбранный узел на дереве
        /// </summary>
        private TreeNodeAdv SelectedNode { get; set; }

        /// <summary>
        /// Описание Tag выбранного узла на дереве
        /// </summary>
        private Node NodeFromSelectedNode { get; set; }

        /// <summary>
        /// Привязываемое устройство
        /// </summary>
        private Device.IODevice SelectedDevice { get; set; }

        /// <summary>
        /// Привязываемый канал
        /// </summary>
        private Device.IODevice.IOChannel SelectedChannel { get; set; }

        /// <summary>
        /// Выбранный объект на графической схеме
        /// </summary>
        private StorableObject SelectedObject { get; set; }

        /// <summary>
        /// Функция модуля ввода-вывода
        /// </summary>
        private Function SelectedIOModuleFunction { get; set; }

        /// <summary>
        /// Функция клеммы модуля ввода-вывода
        /// </summary>
        private Function SelectedClampFunction { get; set; }

        /// <summary>
        /// Функциональный текст для записи в клемму
        /// </summary>
        private string NewFunctionalText { get; set; }

        /// <summary>
        /// Устройства для привязки к каналам
        /// </summary>
        private string SetDevicesChannel { get; set; }

        /// <summary>
        /// Устройства для сброса привязки к каналам
        /// </summary>
        private string ResetDevicesChannel { get; set; }

        /// <summary>
        /// Шаблон для разбора имени модуля ввода-вывода (-А101)
        /// </summary>
        const string IOModuleNamePattern = @"=*-A(?<n>\d+)";

        /// <summary>
        /// Инициализатор первоначальных значений для работы привязки
        /// </summary>
        private StartValues startValues;

        /// <summary>
        /// Форма с данными
        /// </summary>
        private DFrm DevicesForm { get; set; }
        #endregion
    }

    /// <summary>
    /// Инициализатор стартовых значений для привязки
    /// </summary>
    public sealed class StartValues
    {
        /// <summary>
        /// Конструктор принимающий форму с данными
        /// </summary>
        /// <param name="devicesForm">Форма с данными</param>
        public StartValues(DFrm devicesForm)
        {
            this.DevicesForm = devicesForm;
        }

        /// <summary>
        /// Получить выбранный узел из дерева каналов и устройств
        /// </summary>
        /// <returns>Выбранный в дереве узел</returns>
        public TreeNodeAdv GetSelectedNode()
        {
            try
            {
                TreeNodeAdv selectedNode = DevicesForm.GetLastSelectedNode();
                    

                return selectedNode;
            }
            catch
            {
                const string Message = "Некорректно выбран узел в " +
                    "дереве";
                throw new Exception(Message);
            }
        }

        /// <summary>
        /// Получить узел с каналом и устройством выбранного в дереве
        /// </summary>
        /// <param name="selectedNode">Выбранный в дереве узел</param>
        /// <returns>Узел с информацией из выбранного узла</returns>
        public Node GetNodeFromSelectedNode(TreeNodeAdv selectedNode)
        {
            var nodeFromSelectedNode = selectedNode.Tag as Node;

            if (nodeFromSelectedNode == null)
            {
                const string Message =
                    "Ошибка инициализации выбранного узла";
                throw new Exception(Message);
            }

            return nodeFromSelectedNode;
        }

        /// <summary>
        /// Получить привязываемый канал
        /// </summary>
        /// <param name="nodeFromSelectedNode">Узел с информацией
        /// </param>
        /// <returns>Канал</returns>
        public Device.IODevice.IOChannel GetChannel(
            Node nodeFromSelectedNode)
        {
            var channel = nodeFromSelectedNode.Tag as
                Device.IODevice.IOChannel;

            if (channel == null)
            {
                const string Message = "Канал не найден";
                throw new Exception(Message);
            }

            return channel;
        }

        /// <summary>
        /// Получить привязываемое устройство
        /// </summary>
        /// <param name="nodeFromSelectedNode">Узел с информацией
        /// </param>
        /// <returns>Устройство</returns>
        public Device.IODevice GetDevice(Node nodeFromSelectedNode)
        {
            var device = nodeFromSelectedNode.Parent.Tag as
                Device.IODevice;

            if (device == null)
            {
                const string Message = "Устройство не найдено";
                throw new Exception(Message);
            }

            return device;
        }

        /// <summary>
        /// Получить текущий проект
        /// </summary>
        /// <returns>Проект</returns>
        public Project GetProject()
        {
            SelectionSet selection = GetSelectionSet();
            const bool useDialog = false;

            Project project = selection.GetCurrentProject(useDialog);

            return project;
        }

        /// <summary>
        /// Получить объект выбранный на графической схеме
        /// </summary>
        /// <returns>Выбранный на схеме объект</returns>
        public StorableObject GetSelectedObject()
        {
            SelectionSet selection = GetSelectionSet();
            const bool isFirstObject = true;
            StorableObject selectedObject = selection.
                GetSelectedObject(isFirstObject);

            if (selectedObject is Function == false)
            {
                const string Message =
                    "Выбранный на схеме объект не найден";
                throw new Exception(Message);
            }

            return selectedObject;
        }

        /// <summary>
        /// Получить выборку из выбранных объектов в Eplan API
        /// </summary>
        /// <returns></returns>
        private SelectionSet GetSelectionSet()
        {
            var selection = new SelectionSet();
            selection.LockSelectionByDefault = false;
            return selection;
        }

        /// <summary>
        /// Получить функцию выбранной клеммы
        /// </summary>
        /// <param name="selectedObject">Выбранный на схеме объект
        /// </param>
        /// <returns>Функция клеммы модуля ввода-вывода</returns>
        public Function GetSelectedClampFunction(
            StorableObject selectedObject)
        {
            var clampFunction = selectedObject as Function;

            if (clampFunction.Category != Function.Enums.Category.
                PLCTerminal)
            {
                const string Message =
                    "Выбранная функция не является клеммой модуля " +
                    "ввода-вывода";
                throw new Exception(Message);
            }

            return clampFunction;
        }

        /// <summary>
        /// Получить функцию модуля ввода-вывода.
        /// Модуль, куда привязывается устройство.
        /// </summary>
        /// <param name="clampFunction">Функция клеммы модуля 
        /// ввода-вывода</param>
        public Function GetSelectedIOModuleFunction(
            Function clampFunction)
        {
            try
            {
                const string ValveTerminalName = "Y";
                var isValveTerminalClampFunction = false;
                Function IOModuleFunction = null;

                if (clampFunction.Name.Contains(ValveTerminalName))
                {
                    IOModuleFunction = GetIOModuleFunction(
                        clampFunction);
                    if (IOModuleFunction != null)
                    {
                        isValveTerminalClampFunction = true;
                    }
                }

                if (isValveTerminalClampFunction == false)
                {
                    IOModuleFunction = clampFunction.ParentFunction;
                }

                if (IOModuleFunction == null)
                {
                    MessageBox.Show(
                        "Данная клемма названа некорректно. Измените" +
                        " ее название (пример корректного названия " +
                        "\"===DSS1+CAB4-A409\"), затем повторите " +
                        "попытку привязки устройства.",
                        "EPlaner",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);

                    throw new Exception();
                }

                return IOModuleFunction;
            }
            catch
            {
                const string Message = "Не найдена клемма " +
                    "пневмоострова";
                throw new Exception(Message);
            }
        }

        /// <summary>
        /// Проверка клеммы модуля ввода-вывода на принадлежность к 
        /// пневмоострову и возврат параметра указывающего на это
        /// </summary>
        /// <param name="clampFunction">Функция клеммы модуля 
        /// ввода-вывода</param>   
        /// <returns>Функция модуля ввода-вывода</returns>
        private Function GetIOModuleFunction(Function clampFunction)
        {
            Function IOModuleFunction = null;
            const string valveTerminalPattern =
                @"([A-Z0-9]+\-[Y0-9]+)";
            string valveTerminalName = Regex.
                Match(clampFunction.Name, valveTerminalPattern).Value;

            if (string.IsNullOrEmpty(valveTerminalName))
            {
                const string Message = "Ошибка поиска ОУ " +
                    "пневмоострова";
                throw new Exception(Message);
            }

            var objectFinder = new DMObjectsFinder(GetProject());
            var functionsFilter = new FunctionsFilter();
            var properties = new FunctionPropertyList();
            properties.FUNC_MAINFUNCTION = true;
            functionsFilter.SetFilteredPropertyList(properties);
            functionsFilter.Category = Function.Enums.Category.PLCBox;
            Function[] functions = objectFinder
                .GetFunctions(functionsFilter);

            foreach (Function function in functions)
            {
                Function[] subFunctions = function.SubFunctions;
                if (subFunctions != null)
                {
                    foreach (Function subFunction in subFunctions)
                    {
                        var functionalText = subFunction.Properties.
                            FUNC_TEXT_AUTOMATIC.
                            ToString(ISOCode.Language.L___);
                        if (functionalText.Contains(valveTerminalName))
                        {
                            IOModuleFunction = subFunction.
                                ParentFunction;
                            return IOModuleFunction;
                        }
                    }
                }
            }
            return IOModuleFunction;
        }

        #region Закрытые поля
        /// <summary>
        /// Форма с данными
        /// </summary>
        private DFrm DevicesForm { get; set; }
        #endregion
    }
}
