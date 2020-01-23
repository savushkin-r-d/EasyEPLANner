using Aga.Controls.Tree;
using Eplan.EplApi.Base;
using Eplan.EplApi.DataModel;
using Eplan.EplApi.HEServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Device;
using IO;
using StaticHelper;

namespace EasyEPlanner
{
    /// <summary>
    /// Класс, реализующий привязку канала устройства к модулю I/O
    /// </summary>
    public sealed class DeviceBinder
    {
        /// <summary>
        /// Конструктор, в который передается экземпляр формы для доступа
        /// к объектам формы.
        /// </summary>
        /// <param name="devicesForm">Форма с объектами</param>
        public DeviceBinder(DFrm devicesForm)
        {
            this.DevicesForm = devicesForm;
            this.startValues = new StartValuesForBinding(DevicesForm);
        }

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
                SelectedObject = ApiHelper.GetSelectedObject();
                SelectedClampFunction = ApiHelper.GetClampFunction(
                    SelectedObject);
                SelectedIOModuleFunction = ApiHelper
                    .GetIOModuleFunction(SelectedClampFunction);
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
            const string Reserve = "Резерв";
            
            bool isIOLink = CheckIOLink();
            if (isIOLink)
            {
                NewFunctionalText = GenerateFunctionalText(isIOLink);
            }
            else
            {
                NewFunctionalText = GenerateFunctionalText(isIOLink);
            }

            var oldFunctionalText = ApiHelper.GetFunctionalText(
                SelectedClampFunction);

            if (SelectedClampFunction.Properties.FUNC_TEXT.IsEmpty ||
                SelectedClampFunction.Properties.FUNC_TEXT == Reserve)
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
                    NewFunctionalText = Reserve;
                }
                else
                {
                    if ((Control.ModifierKeys & Keys.Control) ==
                        Keys.Control)
                    {
                        if (!(oldFunctionalText + NewLine).
                            Contains(SelectedChannel.Comment + NewLine))
                        {
                            MessageBox.Show(
                                "Действие канала устройства (\"" +
                                SelectedChannel.Comment + "\") " +
                                "отличается от действия уже " +
                                "привязанного канала модуля!",
                                "EPlaner",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);

                            return;
                        }

                        var functionalTextContainsDevice = 
                            FunctionalTextContainsDevice(oldFunctionalText, 
                            SelectedDevice.EPlanName);
                        if (functionalTextContainsDevice == true)
                        {
                            ResetDevicesChannel = NewFunctionalText;
                            NewFunctionalText = oldFunctionalText
                                .Replace(SelectedDevice.EPlanName, string.Empty)
                                .Trim();

                            if (NewFunctionalText.Length > 0)
                            {
                                //Если строка начинается не с символа "+",
                                //заменяем ее на "Резерв".
                                if (NewFunctionalText[0] != '+')
                                {
                                    NewFunctionalText = Reserve;
                                }
                            }
                        }
                        else
                        {
                            SetDevicesChannel = NewFunctionalText;
                            string text = NewLine + SelectedDevice.EPlanName;
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
            var moduleInfo = new IOModuleInfo();
            moduleInfo = moduleInfo.GetIOModuleInfo(name, out _);

            Dictionary<string, string> devicesComments = ProjectConfiguration
                .GetInstance().GetBindingForResettingChannel(
                SelectedClampFunction, moduleInfo, ResetDevicesChannel);

            foreach (KeyValuePair<string, string> pair in devicesComments)
            {
                var deviceName = pair.Key;
                var deviceComment = pair.Value;

                var device = DeviceManager.GetInstance().GetDevice(deviceName);

                string channelName = ApiHelper
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

            int propertyNumber = (int) Eplan.EplApi.DataModel
                .Properties.Article.ARTICLE_TYPENR;
            string name = GetSelectedIOModuleArticleProperty(propertyNumber);

            var deviceNumber = Convert.ToInt32(match.Groups["n"].Value);
            var moduleNumber = deviceNumber % 100;
            int nodeNumber;
            var clampNumber = SelectedClampFunction.Properties
                .FUNC_ADDITIONALIDENTIFYINGNAMEPART.ToInt();

            // Есть ли PXC A1 в проекте
            if (IOManager.GetInstance().IONodes[0].Type ==
                IONode.TYPES.T_PHOENIX_CONTACT_MAIN)
            {
                // Если есть "A1", то учитываем, что он первый
                nodeNumber = deviceNumber / 100;
            }
            else
            {
                // Если нету, оставляем как было
                nodeNumber = deviceNumber / 100 - 1;
            }

            var moduleInfo = new IOModuleInfo();
            moduleInfo = moduleInfo.GetIOModuleInfo(name, out _);
            var logicalPort = Array.IndexOf(moduleInfo.ChannelClamps,
                clampNumber) + 1;
            var moduleOffset = IOManager.GetInstance()
                .IONodes[nodeNumber].
                IOModules[moduleNumber - 1].InOffset;

            SelectedChannel.SetChannel(nodeNumber, moduleNumber,
                clampNumber, deviceNumber, logicalPort, moduleOffset);
        }

        /// <summary>
        /// Обновление дерева устройств после привязки
        /// </summary>
        private void RefreshTree()
        {
            DevicesForm.RefreshTreeAfterBinding();
        }

        /// <summary>
        /// Генерирование функционального текста для вставки
        /// </summary>
        /// <param name="isIOLink">Является ли модуль IO-Link</param>
        /// <returns>Функциональный текст</returns>
        private string GenerateFunctionalText(bool isIOLink)
        {
            const string PhoenixContact = "PXC";

            string functionalText = SelectedDevice.EPlanName + NewLine;
            if (string.IsNullOrEmpty(SelectedDevice.Description) == false)
            {
                functionalText += SelectedDevice.Description + NewLine;
            }
            if (string.IsNullOrEmpty(SelectedChannel.Comment) == false)
            {
                functionalText += SelectedChannel.Comment + NewLine;
            }

            int propertyNumber = (int) Eplan.EplApi.DataModel.Properties
                .Article.ARTICLE_MANUFACTURER;
            string manufacturer = GetSelectedIOModuleArticleProperty(
                propertyNumber);

            if (isIOLink == true && 
                manufacturer.Contains(PhoenixContact) &&
                SelectedDevice.Channels.Count > 1)
            {
                string channelType = ApiHelper
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
                IOModuleNamePattern);

            if (IOModuleMatch.Success == false)
            {
                return isIOLink;
            }

            var IOModuleNumber = Convert.ToInt32(IOModuleMatch.Groups["n"]
                .Value);

            IOModule module = IOManager.GetInstance()
                .GetModuleByPhysicalNumber(IOModuleNumber);
            isIOLink = module.isIOLink();

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
            var result = string.Empty;
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
                value = value.Replace(NewLine, string.Empty);
                if (value == deviceName)
                {
                    isContains = true;
                }
            }

            return isContains;
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
        /// Шаблон для разбора имени модуля ввода-вывода (-А101)
        /// </summary>
        const string IOModuleNamePattern = @"=*-A(?<n>\d+)";

        /// <summary>
        /// Инициализатор первоначальных значений для работы привязки
        /// </summary>
        private StartValuesForBinding startValues;

        /// <summary>
        /// Форма с данными
        /// </summary>
        private DFrm DevicesForm { get; set; }

        /// <summary>
        /// Шаблон для добавления новой строки
        /// </summary>
        const string NewLine = "\r\n";
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
