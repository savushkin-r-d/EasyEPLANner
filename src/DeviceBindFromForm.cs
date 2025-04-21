using Aga.Controls.Tree;
using Eplan.EplApi.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using EplanDevice;
using IO;
using StaticHelper;
using IO.View;
using System.Diagnostics.CodeAnalysis;

namespace EasyEPlanner
{
    /// <summary>
    /// Класс, реализующий привязку канала устройства к модулю I/O
    /// </summary>
    public sealed class DeviceBinder
    {
        IApiHelper apiHelper;
        IIOHelper ioHelper;

        /// <summary>
        /// Конструктор, в который передается экземпляр формы для доступа
        /// к объектам формы.
        /// </summary>
        /// <param name="devicesForm">Форма с объектами</param>
        public DeviceBinder(DFrm devicesForm, IApiHelper apiHelper, IIOHelper ioHelper)
        {
            this.DevicesForm = devicesForm;
            this.startValues = new StartValuesForBinding(DevicesForm);
            this.apiHelper = apiHelper;
            this.ioHelper = ioHelper;
        }

        /// <summary>
        /// Привязать выбранный канал устройства к модулю ввода-вывода
        /// </summary>
        public void Bind()
        {
            try
            {
                InitStartValues();
                
                if (!IsCorrectClampNumber())
                    return;
 
                PrepareFunctionalText();

                if (!string.IsNullOrEmpty(ResetDevicesChannel))
                {
                    ResetChannel();
                    SelectedClamp?.Reset();
                }

                SetFunctionalTextInClamp();

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
        /// Проверка на корректный номер клемы при привязке. 
        /// Если для привязки выбрана вспомогательная клема (питание или 0V),
        /// то функция вернет false и привязки не произойдет.
        /// </summary>
        /// <returns></returns>
        private bool IsCorrectClampNumber()
        {
            string clampNumberAsString = DeviceBindingHelper
                .GetClampNumberAsString(SelectedClampFunction, ioHelper);
            bool isDigit = int.TryParse(clampNumberAsString,
                out int clampNumber);
            
            if (!isDigit)
            {
                return false;
            }

            int propertyNumber = (int)Eplan.EplApi.DataModel.Properties
                .Article.ARTICLE_TYPENR;
            string name = GetSelectedIOModuleArticleProperty(propertyNumber);
            var moduleInfo = IOModuleInfo.GetModuleInfo(name.Trim(), out _);

            return Array.IndexOf(moduleInfo.ChannelClamps, clampNumber) >= 0;
        }

        /// <summary>
        /// Инициализация базовых значений переменных
        /// необходимых для привязки
        /// </summary>
        [ExcludeFromCodeCoverage]
        private void InitStartValues()
        {
            SelectedNode = startValues.GetSelectedNode();
            SelectedClamp = IOViewControl.DataContext?.SelectedClamp;
            NodeFromSelectedNode = startValues.GetNodeFromSelectedNode(SelectedNode);
            SelectedChannel = startValues.GetChannel(NodeFromSelectedNode);
            SelectedDevice = startValues.GetDevice(NodeFromSelectedNode);
            
            try
            {
                // Пробуем получить выбранную клемму на ФСА
                SelectedObject = apiHelper.GetSelectedObject();
                SelectedClampFunction = ioHelper.GetClampFunction(SelectedObject);
                SelectedIOModuleFunction = ioHelper.GetIOModuleFunction(SelectedClampFunction);
            }
            catch
            {
                // Если нет, то пытаемся получить выбранную клемму в окне узлов и модулей
                // (если таковой нет, то генерируем исключение)
                SelectedClampFunction = 
                    (IOViewControl.DataContext?.SelectedClampFunction as EplanFunction)
                        ?.Function ?? throw new ArgumentNullException();
                    
                SelectedIOModuleFunction = ioHelper.GetIOModuleFunction(SelectedClampFunction);
            }
        }

        /// <summary>
        /// Подготовка функционального текста для записи в функцию
        /// </summary>
        private void PrepareFunctionalText()
        {            
            NewFunctionalText = GenerateFunctionalText(CheckIOLink());
            var oldFunctionalText = apiHelper.GetFunctionalText(
                SelectedClampFunction);

            if (SelectedClampFunction.Properties.FUNC_TEXT.IsEmpty ||
                SelectedClampFunction.Properties.FUNC_TEXT == CommonConst.Reserve)
            {
                //Если нет функционального текста, устанавливаем его.
                if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                {
                    //С Ctrl устанавливаем только название устройства.
                    NewFunctionalText = SelectedDevice.EplanName;
                }

                SetDevicesChannel = NewFunctionalText;
            }
            else
            {
                //Замена на "Резерв".
                if (SelectedClampFunction.Properties.FUNC_TEXT
                    .ToString(Eplan.EplApi.Base.ISOCode.Language.L___) ==
                    NewFunctionalText.Replace("\r", ""))
                {
                    ResetDevicesChannel = NewFunctionalText;
                    NewFunctionalText = CommonConst.Reserve;
                }
                else
                {
                    if ((Control.ModifierKeys & Keys.Control) ==
                        Keys.Control)
                    {
                        if (!(oldFunctionalText + CommonConst
                            .NewLineWithCarriageReturn)
                            .Contains(SelectedChannel.Comment +
                            CommonConst.NewLineWithCarriageReturn))
                        {
                            MessageBox.Show(
                                "Действие канала устройства (\"" +
                                SelectedChannel.Comment + "\") " +
                                "отличается от действия уже " +
                                "привязанного канала модуля!",
                                "EPlaner",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);

                            throw new Exception();
                        }

                        var functionalTextContainsDevice = 
                            FunctionalTextContainsDevice(oldFunctionalText, 
                            SelectedDevice.EplanName);
                        if (functionalTextContainsDevice == true)
                        {
                            ResetDevicesChannel = NewFunctionalText;
                            NewFunctionalText = oldFunctionalText
                                .Replace(SelectedDevice.EplanName, "")
                                .Trim();

                            if (NewFunctionalText.Length > 0)
                            {
                                //Если строка начинается не с символа "+",
                                //заменяем ее на "Резерв".
                                if (NewFunctionalText[0] != '+')
                                {
                                    NewFunctionalText = CommonConst.Reserve;
                                }
                            }
                        }
                        else
                        {
                            string text = "";
                            if (oldFunctionalText == CommonConst.Reserve)
                            {
                                oldFunctionalText = "";
                                text = SelectedDevice.EplanName;
                            }
                            else
                            {
                                text = CommonConst.NewLineWithCarriageReturn +
                                SelectedDevice.EplanName;
                            }

                            SetDevicesChannel = NewFunctionalText;
                            NewFunctionalText = oldFunctionalText + text;
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
            int propertyNumber = (int) Eplan.EplApi.DataModel.Properties
                .Article.ARTICLE_TYPENR;
            string name = GetSelectedIOModuleArticleProperty(propertyNumber);
            var moduleInfo = IOModuleInfo.GetModuleInfo(name, out _);

            Dictionary<string, string> devicesComments = ProjectConfiguration
                .GetInstance().GetBindingForResettingChannel(
                SelectedClampFunction, moduleInfo, ResetDevicesChannel);

            foreach (KeyValuePair<string, string> pair in devicesComments)
            {
                var deviceName = pair.Key;
                var deviceComment = pair.Value;

                var device = DeviceManager.GetInstance().GetDevice(deviceName);

                string channelName = DeviceBindingHelper
                    .GetChannelNameForIOLinkModuleFromString(deviceComment);

                device.ClearChannel(moduleInfo.AddressSpaceType,
                    deviceComment, channelName);
            }
        }

        /// <summary>
        /// Установка функционального текста в клемму
        /// </summary>
        private void SetFunctionalTextInClamp()
        {
            SelectedClampFunction.Properties.FUNC_TEXT = NewFunctionalText;
        }

        /// <summary>
        /// Привязать канал
        /// </summary>
        [ExcludeFromCodeCoverage]
        private void BindChannel()
        {
            if (SelectedIOModuleFunction == null)
            {
                return;
            }

            var regex = new Regex(IOManager.IONamePattern);
            var match = regex.Match(SelectedIOModuleFunction.VisibleName);
            if (match.Success == false)
            {
                return;
            }

            var reader = new DeviceBindingReader(new ProjectHelper(apiHelper), apiHelper);

            reader.ReadModuleClampBinding(new EplanFunction(SelectedClampFunction));
        }

        /// <summary>
        /// Обновление дерева устройств и модулей после привязки
        /// </summary>
        private void RefreshTree()
        {
            DevicesForm.RefreshTreeAfterBinding();
            IOViewControl.Instance?.RefreshTreeAfterBinding();
        }

        /// <summary>
        /// Генерирование функционального текста для вставки
        /// </summary>
        /// <param name="isIOLink">Является ли модуль IO-Link</param>
        /// <returns>Функциональный текст</returns>
        private string GenerateFunctionalText(bool isIOLink)
        {
            const string PhoenixContact = "PXC";

            string functionalText = SelectedDevice.EplanName + 
                CommonConst.NewLineWithCarriageReturn;
            if (string.IsNullOrEmpty(SelectedDevice.Description) == false)
            {
                functionalText += SelectedDevice.Description +
                    CommonConst.NewLineWithCarriageReturn;
            }
            if (string.IsNullOrEmpty(SelectedChannel.Comment) == false)
            {
                functionalText += SelectedChannel.Comment +
                    CommonConst.NewLineWithCarriageReturn;
            }

            int propertyNumber = (int) Eplan.EplApi.DataModel.Properties
                .Article.ARTICLE_MANUFACTURER;
            string manufacturer = GetSelectedIOModuleArticleProperty(
                propertyNumber);

            if (isIOLink == true && 
                manufacturer.Contains(PhoenixContact) &&
                SelectedDevice.Channels.Count > 1)
            {
                string channelType = DeviceBindingHelper
                    .GetChannelNameForIOLinkModuleFromString(
                    SelectedChannel.Name);
                functionalText += channelType;
            }

            return functionalText;
        }

        /// <summary>
        /// Наличие IO-Link у модуля
        /// </summary>
        /// <returns></returns>
        private bool CheckIOLink()
        {
            var isIOLink = false;
            string IOModuleFunctionName = SelectedClampFunction
                .ParentFunction.VisibleName;
            var IOModuleMatch = Regex.Match(IOModuleFunctionName,
                IOManager.IONamePattern);

            if (IOModuleMatch.Success == false)
            {
                return isIOLink;
            }

            var IOModuleNumber = Convert.ToInt32(IOModuleMatch.Groups["n"]
                .Value);

            IIOModule module = IOManager.GetInstance()
                .GetModuleByPhysicalNumber(IOModuleNumber);
            isIOLink = module.IsIOLink();

            return isIOLink;
        }

        /// <summary>
        /// Получение свойств выбранного модуля ввода-вывода по их глобальному
        /// индексу, который указан в перечислении.
        /// Предполагаем, что там всего 1 устройство - модуль ввода-вывода.
        /// </summary>
        /// <param name="propetyNumber">Индекс</param>
        /// <returns>Строка со значением</returns>
        private string GetSelectedIOModuleArticleProperty(int propetyNumber)
        {
            var result = "";
            if (SelectedIOModuleFunction != null &&
                SelectedIOModuleFunction.Articles.Count() > 0 &&
                !SelectedIOModuleFunction.Articles[0]
                .Properties[propetyNumber].IsEmpty)
            {
                result = SelectedIOModuleFunction.Articles[0]
                    .Properties[propetyNumber];
            }
            return result;
        }

        /// <summary>
        /// Содержит ли функциональный текст с устройствами конкретное
        /// устройство
        /// </summary>
        /// <param name="deviceName">Имя устройства для поиска</param>
        /// <param name="functionalText">Функциональный текст с устройствами
        /// </param>
        /// <returns></returns>
        private bool FunctionalTextContainsDevice(string functionalText,
            string deviceName)
        {
            var isContains = false;
            var matches = Regex.Matches(functionalText,
                            DeviceManager.BINDING_DEVICES_DESCRIPTION_PATTERN);

            foreach (Match match in matches)
            {
                string value = match.Value;
                value = value.Replace(CommonConst.NewLineWithCarriageReturn, "");
                if (value == deviceName)
                {
                    isContains = true;
                }
            }

            return isContains;
        }

        #region Закрытые поля
        
        /// <summary>
        /// Выбранная клемма в <see cref="IO.View.IOViewControl">окне модулей</see>
        /// </summary>
        private IO.ViewModel.IClamp SelectedClamp { get; set; }

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
        private IODevice SelectedDevice { get; set; }

        /// <summary>
        /// Привязываемый канал
        /// </summary>
        private IODevice.IOChannel SelectedChannel { get; set; }

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
        /// Инициализатор первоначальных значений для работы привязки
        /// </summary>
        private StartValuesForBinding startValues;

        /// <summary>
        /// Форма с данными
        /// </summary>
        private DFrm DevicesForm { get; set; }
        #endregion
    }

    /// <summary>
    /// Инициализатор стартовых значений для привязки
    /// </summary>
    public sealed class StartValuesForBinding
    {
        /// <summary>
        /// Конструктор принимающий форму с данными
        /// </summary>
        /// <param name="devicesForm">Форма с данными</param>
        public StartValuesForBinding(DFrm devicesForm)
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
        /// Получить узел с каналом и устройством, выбранный в дереве
        /// </summary>
        /// <param name="selectedNode">Выбранный в дереве узел</param>
        /// <returns>Узел с информацией из выбранного узла</returns>
        public Node GetNodeFromSelectedNode(TreeNodeAdv selectedNode)
        {
            var nodeFromSelectedNode = selectedNode.Tag as Node;

            if (nodeFromSelectedNode == null)
            {
                const string Message = "Ошибка инициализации выбранного узла";
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
        public IODevice.IOChannel GetChannel(Node nodeFromSelectedNode)
        {
            var channel = nodeFromSelectedNode.Tag as IODevice.IOChannel;

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
        public IODevice GetDevice(Node nodeFromSelectedNode)
        {
            var device = nodeFromSelectedNode.Parent.Tag as IODevice;

            if (device == null)
            {
                const string Message = "Устройство не найдено";
                throw new Exception(Message);
            }

            return device;
        }

        #region Закрытые поля
        /// <summary>
        /// Форма с данными
        /// </summary>
        private DFrm DevicesForm { get; set; }
        #endregion
    }
}
