using Editor;
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
        /// Параметр действия
        /// </summary>
        /// <param name="luaName">LUA-название</param>
        /// <param name="name">Название</param>
        /// <param name="defaultValue">Значение по-умолчанию</param>
        /// <param name="displayObjects">Объекты отображаемые в меню "Устройства, параметры объектов"</param>
        /// <param name="onlyParameterNumber">Можно установить только номер параметра</param>
        public ActionParameter(string luaName, string name) 
            : base(luaName, name, "-1", new List<DisplayObject>{ DisplayObject.Parameters })
        { }

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
            return base.SetNewValue(newValue);
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

                    value = Parameter?.GetParameterNumber.ToString() ?? value;
                }
                else
                {
                    var parameterNumber = Parameter.GetParameterNumber;
                    if (parameterNumber == 0)
                        SetNewValue("-1"); // Параметр удален
                    else if (parameterNumber.ToString() != value.ToString())
                        SetNewValue(parameterNumber.ToString()); // Параметр перемещен
                }

                return base.Value;
            }
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
