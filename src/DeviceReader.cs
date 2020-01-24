﻿using Eplan.EplApi.DataModel;
using System;
using System.Collections.Generic;
using StaticHelper;
using System.Text.RegularExpressions;
using Eplan.EplApi.Base;
using Device;

namespace EasyEPlanner
{
    /// <summary>
    /// Считыватель устройств
    /// </summary>
    class DeviceReader
    {
        public DeviceReader()
        {
            this.deviceManager = DeviceManager.GetInstance();
        }

        /// <summary>
        /// Получить привязку для сброса канала устройства
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetBindingForResettingChannel(
            Function deviceClampFunction, IO.IOModuleInfo moduleInfo,
            string devicesDescription = "")
        {
            var res = new Dictionary<string, string>();
            string clampNumberAsString = deviceClampFunction.Properties
                .FUNC_ADDITIONALIDENTIFYINGNAMEPART.ToString();

            if (deviceClampFunction.Name.Contains(DeviceManager
                .ValveTerminalName))
            {
                Function IOModuleFunction = ApiHelper
                    .GetIOModuleFunction(deviceClampFunction);
                string bindedDevice = deviceClampFunction.Name;
                Function IOModuleClampFunction = ApiHelper
                    .GetClampFunction(IOModuleFunction, bindedDevice);
                clampNumberAsString = IOModuleClampFunction.Properties
                .FUNC_ADDITIONALIDENTIFYINGNAMEPART.ToString();
            }

            int clampNumber;
            bool isDigit = int.TryParse(clampNumberAsString, out clampNumber);
            if (isDigit == false)
            {
                return res;
            }

            if (Array.IndexOf(moduleInfo.ChannelClamps, clampNumber) < 0)
            {
                return res;
            }

            DocumentTypeManager.DocumentType pageType = deviceClampFunction
                .Page.PageType;
            if (pageType != DocumentTypeManager.DocumentType.Circuit &&
                pageType != DocumentTypeManager.DocumentType.Overview)
            {
                return res;
            }

            if (devicesDescription == "")
            {
                devicesDescription = ApiHelper.GetFunctionalText(
                    deviceClampFunction);
            }

            if (devicesDescription == "")
            {
                return res;
            }

            var comment = "";
            var clampComment = "";
            Match actionMatch;
            bool isMultipleBinding = deviceManager.IsMultipleBinding(
                devicesDescription);
            if (isMultipleBinding == false)
            {
                int endPos = devicesDescription.IndexOf(CommonConst.NewLine);
                if (endPos > 0)
                {
                    comment = devicesDescription.Substring(endPos + 1);
                    devicesDescription = devicesDescription.Substring(0,
                        endPos);
                }

                devicesDescription = Regex.Replace(devicesDescription,
                    CommonConst.RusAsEngPattern, CommonConst.RusAsEnsEvaluator);

                actionMatch = Regex.Match(comment,
                    IODevice.IOChannel.ChannelCommentPattern,
                    RegexOptions.IgnoreCase);

                comment = Regex.Replace(comment,
                    IODevice.IOChannel.ChannelCommentPattern,
                    "", RegexOptions.IgnoreCase);
                comment = comment.Replace(CommonConst.NewLine, ". ").Trim();
                if (comment.Length > 0 && comment[comment.Length - 1] != '.')
                {
                    comment += ".";
                }
            }
            else
            {
                devicesDescription = Regex.Replace(devicesDescription,
                    CommonConst.RusAsEngPattern, CommonConst.RusAsEnsEvaluator);
                actionMatch = Regex.Match(comment,
                    IODevice.IOChannel.ChannelCommentPattern,
                    RegexOptions.IgnoreCase);
            }

            var descrMatch = Regex.Match(devicesDescription,
                DeviceManager.BINDING_DEVICES_DESCRIPTION_PATTERN);
            while (descrMatch.Success)
            {
                string devName = descrMatch.Groups["name"].Value;

                if (actionMatch.Success)
                {
                    clampComment = actionMatch.Value;
                }

                res.Add(devName, clampComment);
                descrMatch = descrMatch.NextMatch();
            }

            return res;
        }

        /// <summary>
        /// Прочитать устройства
        /// </summary>
        public void Read()
        {
            PrepareForReading();
            ReadDevices();            
        }

        /// <summary>
        /// Подготовка к чтению устройств
        /// </summary>
        private void PrepareForReading() 
        {
            deviceManager.Clear();
            var objectFinder = new DMObjectsFinder(ApiHelper.GetProject());

            var propertyList = new FunctionPropertyList();
            propertyList.FUNC_MAINFUNCTION = true;

            var functionsFilter = new FunctionsFilter();
            functionsFilter.IsPlaced = true;
            functionsFilter.SetFilteredPropertyList(propertyList);

            deviceFunctions = objectFinder.GetFunctions(functionsFilter);
        }

        /// <summary>
        /// Чтение устройств
        /// </summary>
        private void ReadDevices() 
        {
            foreach (var function in deviceFunctions)
            {
                bool skip = NeedToSkip(function);
                if (skip == true)
                {
                    continue;
                }

                string name = GetName(function);
                string description = GetDescription(function);
                string subType = GetSubType(function);
                string parameters = GetParameters(function);
                string properties = GetProperties(function);
                string runtimeParameters = GetRuntimeParameters(function);
                int deviceLocation = GetDeviceLocation(function);
                string articleName = GetArticleName(function);

                string error;
                deviceManager.AddDeviceAndEFunction(name, description,
                    subType, parameters, runtimeParameters, properties,
                    deviceLocation, function, out error, articleName);

                if (error != "")
                {
                    ProjectManager.GetInstance().AddLogMessage(error);
                }
            }

            deviceManager.Sort();
        }

        /// <summary>
        /// Нужно ли пропускать функцию.
        /// </summary>
        /// <param name="function">Функция для проверки</param>
        /// <returns></returns>
        private bool NeedToSkip(Function function)
        {
            var skip = false;

            if (function.VisibleName == "" ||
                    function.Page == null ||
                    function.Page.PageType != DocumentTypeManager.
                    DocumentType.ProcessAndInstrumentationDiagram)
            {
                skip = true;
                return skip;
            }

            //Признак пропуска данного устройства (прим., для ручной заслонки).
            if (!function.Properties.FUNC_SUPPLEMENTARYFIELD[1].IsEmpty)
            {
                skip = true;
                return skip;
            }

            return skip;
        }

        /// <summary>
        /// Получить имя устройства.
        /// </summary>
        /// <param name="function">Функция устройства</param>
        /// <returns></returns>
        private string GetName(Function function)
        {
            var name = function.Name;
            name = Regex.Replace(name, CommonConst.RusAsEngPattern,
                    CommonConst.RusAsEnsEvaluator);
            return name;
        }

        /// <summary>
        /// Получить описание устройства.
        /// </summary>
        /// <param name="function">Функция устройства</param>
        /// <returns></returns>
        private string GetDescription(Function function)
        {
            var description = "";
            string descriptionPattern = "([\'\"])";

            if (!function.Properties.FUNC_COMMENT.IsEmpty)
            {
                description = function.Properties.FUNC_COMMENT
                    .ToString(ISOCode.Language.L___);

                if (description == "")
                {
                    description = function.Properties.FUNC_COMMENT
                        .ToString(ISOCode.Language.L_ru_RU);
                }

                description = Regex.Replace(description,
                    descriptionPattern, "");
            }

            if (description == null)
            {
                description = "";
            }

            return description;
        }

        /// <summary>
        /// Получить подтип устройства.
        /// </summary>
        /// <param name="function">Функция устройства</param>
        /// <returns></returns>
        private string GetSubType(Function function)
        {
            var subType = "";

            if (!function.Properties.FUNC_SUPPLEMENTARYFIELD[2].IsEmpty)
            {
                subType = function.Properties.FUNC_SUPPLEMENTARYFIELD[2]
                    .ToString(ISOCode.Language.L___);

                if (subType == "")
                {
                    subType = function.Properties.FUNC_SUPPLEMENTARYFIELD[2]
                        .ToString(ISOCode.Language.L_ru_RU);
                }

                subType = subType.Trim();
                subType = Regex.Replace(subType, 
                    CommonConst.RusAsEngPattern, CommonConst.RusAsEnsEvaluator);
            }

            if (subType == null)
            {
                subType = "";
            }

            return subType;
        }

        /// <summary>
        /// Получить свойства устройства.
        /// </summary>
        /// <param name="function">Функция устройства</param>
        /// <returns></returns>
        private string GetProperties(Function function)
        {
            var properties = "";

            if (!function.Properties.FUNC_SUPPLEMENTARYFIELD[4].IsEmpty)
            {
                properties = function.Properties.FUNC_SUPPLEMENTARYFIELD[4]
                    .ToString(ISOCode.Language.L___);

                if (properties == "")
                {
                    properties = function.Properties.FUNC_SUPPLEMENTARYFIELD[4]
                        .ToString(ISOCode.Language.L_ru_RU);
                };

                properties = Regex.Replace(properties,
                    CommonConst.RusAsEngPattern, CommonConst.RusAsEnsEvaluator);
            }

            if (properties == null)
            {
                properties = "";
            }

            return properties;
        }

        /// <summary>
        /// Получить параметры времени выполнения.
        /// </summary>
        /// <param name="function">Функция устройства</param>
        /// <returns></returns>
        private string GetRuntimeParameters(Function function)
        {
            var runtimeParameters = "";

            if (!function.Properties.FUNC_SUPPLEMENTARYFIELD[5].IsEmpty)
            {
                runtimeParameters = function.Properties
                    .FUNC_SUPPLEMENTARYFIELD[5]
                    .ToString(ISOCode.Language.L___);

                if (runtimeParameters == "")
                {
                    runtimeParameters = function.Properties.
                        FUNC_SUPPLEMENTARYFIELD[5]
                        .ToString(ISOCode.Language.L_ru_RU);
                };

                if (runtimeParameters == null)
                {
                    runtimeParameters = "";
                }

                runtimeParameters = Regex.Replace(runtimeParameters,
                    CommonConst.RusAsEngPattern, CommonConst.RusAsEnsEvaluator);
            }

            return runtimeParameters;
        }

        /// <summary>
        /// Получить номер шкафа, где располагается устройство.
        /// </summary>
        /// <param name="function">Функция устройства</param>
        /// <returns></returns>
        private int GetDeviceLocation(Function function)
        {
            int deviceLocation = 0;

            if (!function.Properties.FUNC_SUPPLEMENTARYFIELD[6].IsEmpty)
            {
                deviceLocation = int.Parse(function.Properties
                    .FUNC_SUPPLEMENTARYFIELD[6]
                    .ToString(ISOCode.Language.L___));
            }

            return deviceLocation;
        }

        /// <summary>
        /// Получить имя изделия устройства.
        /// </summary>
        /// <param name="function">Функция устройства</param>
        /// <returns></returns>
        private string GetArticleName(Function function)
        {
            var articleName = "";

            var articlesRefs = function.ArticleReferences;
            if (articlesRefs.Length > 0 &&
                function.ArticleReferences[0].PartNr != "" &&
                function.ArticleReferences[0].PartNr != null)
            {
                articleName = function.ArticleReferences[0].PartNr;
            }

            return articleName;
        }

        /// <summary>
        /// Получить параметры устройства.
        /// </summary>
        /// <param name="function">Функция устройства</param>
        /// <returns></returns>
        private string GetParameters(Function function)
        {
            var parameters = "";

            if (!function.Properties.FUNC_SUPPLEMENTARYFIELD[3].IsEmpty)
            {
                parameters = function.Properties.FUNC_SUPPLEMENTARYFIELD[3]
                    .ToString(ISOCode.Language.L___);

                if (parameters == "")
                {
                    parameters = function.Properties.FUNC_SUPPLEMENTARYFIELD[3]
                        .ToString(ISOCode.Language.L_ru_RU);
                };

                parameters = Regex.Replace(parameters, 
                    CommonConst.RusAsEngPattern, CommonConst.RusAsEnsEvaluator);
            }

            if (parameters == null)
            {
                parameters = "";
            }

            return parameters;
        }

        /// <summary>
        /// Копировать устройства в массив
        /// </summary>
        /// <param name="array">Массив для копируемых данных</param>
        public void CopyDevices(IODevice[] array)
        {
            deviceManager.Devices.CopyTo(array);
        }

        /// <summary>
        /// Количество считанных устройств
        /// </summary>
        public int DevicesCount
        {
            get
            {
                return deviceManager.Devices.Count;
            }
        }

        /// <summary>
        /// Считанные устройства
        /// </summary>
        public List<IODevice> Devices
        {
            get
            {
                return deviceManager.Devices;
            }
        }

        /// <summary>
        /// Свойство, указывающее прочитаны устройства или нет.
        /// </summary>
        public bool DevicesIsRead
        {
            get
            {
                if (deviceManager.Devices.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Функции для поиска устройств.
        /// </summary>
        Function[] deviceFunctions;

        /// <summary>
        /// Менеджер устройств.
        /// </summary>
        DeviceManager deviceManager;
    }
}
