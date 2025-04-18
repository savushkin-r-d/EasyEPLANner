﻿using Editor;
using EplanDevice;
using StaticHelper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace TechObject
{
    /// <summary>
    /// Параметр для действий. <br/>
    /// Используется для установки параметра для действия. <br/>
    /// </summary>
    /// <remarks>
    /// Класс имеет ссылку <c><see cref="Parameter"/></c> на привязанный параметр. <br/>
    /// Эта ссылка используется для отслеживания изменений этого параметра, <br/>
    /// перемещение привязанного параметра или его удаление из тех. объекта 
    /// изменит значение поля (<c><see cref="Value"/></c>).
    /// </remarks>
    public class ActionParameter : BaseParameter
    {
        /// <summary>
        /// Параметр дейтсвия
        /// </summary>
        /// <param name="luaName">Lua-название (используется не везде)</param>
        /// <param name="name">Название в Eplan</param>
        /// <param name="onlyParameterNumber">Установка параметров только по номерам</param>
        /// <param name="displayObject">Отображаемые значения в меню устройства и параметры объектов (По-умолчанию: только параметры)</param>
        public ActionParameter(string luaName, string name,
            bool onlyParameterNumber = true, List<DisplayObject> displayObject = null) 
            : base(luaName, name, "-1", displayObject ?? new List<DisplayObject>{ DisplayObject.Parameters })
        { 
            OnlyParameterNumber = onlyParameterNumber;
        }

        /// <summary>
        /// Рекурсивный обход родительских элементов до получения тех. объекта
        /// </summary>
        private TechObject GetTechObject(ITreeViewItem item)
        {
            if (item is null)
                return null;

            return item is TechObject techobject ?
                techobject : GetTechObject(item.Parent);
        }

        /// <summary>
        /// Параметры тех.объекта
        /// </summary>
        private Params Parameters => GetTechObject(Parent)?.GetParamsManager()?.Float;


        public override bool SetNewValue(string newValue)
        {
            Parameter = null;

            // В случае привязки нескольких устройств (с ФСА) оставляем последнее 
            newValue = newValue.Trim().Split(' ').LastOrDefault(); 
            newValue = string.IsNullOrEmpty(newValue) ? "-1" : newValue;

            return base.SetNewValue(newValue);
        }

        public override void UpdateOnGenericTechObject(ObjectProperty genericProperty)
        {
            Parameter = null;
            base.UpdateOnGenericTechObject(genericProperty);
        }

        public override void ModifyDevNames(IDevModifyOptions options)
        {
            var modified = deviceManager.GetModifiedDevice(
                deviceManager.GetDeviceByEplanName(Value),
                options);

            if (modified is null || modified.Description == CommonConst.Cap)
                return;

            SetNewValue(modified.Name);
        }

        public override string Value
        {
            get
            {
                if(Parameter is null)
                {
                    // Если параметр еще не привязан - пытаемся привязать параметр по значению
                    Parameter = int.TryParse(value.ToString(), out var parameterIndex) ?
                        Parameters?.GetParam(parameterIndex - 1) :
                        Parameters?.GetParam(value.ToString());

                    if (OnlyParameterNumber)
                        value = Parameter?.GetParameterNumber.ToString() ?? value;
                }
                else
                {
                    var parameterNumber = Parameter.GetParameterNumber;
                    if (parameterNumber == 0)
                    {
                        if (Parameters.GetParam(Parameter.GetNameLua()) == null)
                            SetNewValue("-1"); // Параметр удален
                        else
                            SetNewValue(Parameters.GetParam(Parameter.GetNameLua()).GetParameterNumber.ToString()); // Параметр заменен
                    }   
                    else if (OnlyParameterNumber && parameterNumber.ToString() != value.ToString())
                        SetNewValue(parameterNumber.ToString()); // Параметр задан номером и перемещен
                }

                return base.Value;
            }
        }

        /// <summary>
        /// Для установки параметра использовать только номер параметра
        /// </summary>
        public bool OnlyParameterNumber { get; private set; }

        public override bool IsDrawOnEplanPage => true;

        public override List<DrawInfo> GetObjectToDrawOnEplanPage()
        {
            if (deviceManager.GetDeviceByEplanName(Value) is IDevice dev &&
                dev.Description != CommonConst.Cap)
            {
                return new List<DrawInfo>() { new DrawInfo(DrawInfo.Style.GREEN_BOX, dev) };
            }

            return new List<DrawInfo>() { };
        }

        public override string[] DisplayText
        {
            get
            {
                var displayedValue = Value;
                if (displayedValue == CommonConst.EmptyValue )
                {
                    return new string[] { Name, CommonConst.StubForCells };
                }

                if (Parameter is null)
                {
                    var dev = deviceManager.GetDeviceByEplanName(displayedValue);

                    if (dev.Description != CommonConst.Cap)
                        return new string[] { Name, displayedValue };
                    else
                        return new string[] { Name, $"Параметр {displayedValue} не найден" };
                }
                
                return new string[] 
                { 
                    Name,
                    $"{Parameter.GetParameterNumber}. {Parameter.GetNameLua()}: " +
                    $"{Parameter.GetValue()} {Parameter.GetMeter()}" 
                };
            }
        }

        /// <summary>
        /// Привязанный параметр
        /// </summary>
        public Param Parameter { get; private set; }

        public override BaseParameter Clone()
        {
            var newProperty = new ActionParameter(LuaName, Name);
            newProperty.SetNewValue(Value);
            newProperty.NeedDisable = NeedDisable;
            
            return newProperty;
        }
    }
}
