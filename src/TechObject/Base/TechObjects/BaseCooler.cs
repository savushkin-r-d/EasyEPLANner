using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Базовый узел охлаждения
    /// </summary>
    public class BaseCooler : BaseTechObject
    {
        public BaseCooler() : base()
        {
            S88Level = 2;
            Name = "Узел охлаждения";
            EplanName = "cooler_node";
            BaseOperations = DataBase.Imitation.CoolerNodeOperations();
            BaseProperties = DataBase.Imitation.EmptyProperties();
            BasicName = "cooler_node";
            Equipment = DataBase.Imitation.CoolerNodeEquipment();
        }

        /// <summary>
        /// Клонировать объект
        /// </summary>
        /// <param name="techObject">Новый владелец базового объекта</param>
        /// <returns></returns>
        public override BaseTechObject Clone(TechObject techObject)
        {
            var cloned = DataBase.Imitation.BaseTechObjects()
                .Where(x => x.Name == this.Name)
                .FirstOrDefault();
            cloned.Owner = techObject;
            return cloned;
        }

        /// <summary>
        /// Инициализировать базовый объект как привязанный к аппарату агрегат.
        /// </summary>
        /// <param name="unit">Базовый объект к которому привязаны агрегаты
        /// </param>
        public override void InitAsAttachedAgregate(BaseTechObject unit)
        {
            switch(unit)
            {
                case BaseTank obj:
                    foreach(var operation in obj.Owner.ModesManager.Modes)
                    {
                        BaseProperty prop;
                        switch(operation.GetBaseOperation().Name)
                        {
                            case "Наполнение":
                                prop = new BoolShowedProperty(
                                    "NEED_COOLING_DURING_FILL", 
                                    "Использовать узел охлаждения", "false");
                                operation.GetBaseOperation().AddProperty(prop);
                                break;

                            case "Хранение":
                                prop = new BoolShowedProperty(
                                    "NEED_COOLING_DURING_STORING", 
                                    "Использовать узел охлаждения", "false");
                                operation.GetBaseOperation().AddProperty(prop);
                                break;

                            case "Выдача":
                                prop = new BoolShowedProperty(
                                    "NEED_COOLING_DURING_OUT", 
                                    "Использовать узел охлаждения", "false");
                                operation.GetBaseOperation().AddProperty(prop);
                                break;

                            default:
                                break;
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Удалить базовый объект как привязанный к аппарату агрегат.
        /// </summary>
        /// <param name="unit">Базовый объект к которому привязаны агрегаты
        /// </param>
        public override void RemoveAsAttachedAgregate(BaseTechObject unit)
        {
            switch (unit)
            {
                case BaseTank obj:
                    foreach (var operation in obj.Owner.ModesManager.Modes)
                    {
                        string luaName;
                        switch (operation.GetBaseOperation().Name)
                        {
                            case "Наполнение":
                                luaName = "NEED_COOLING_DURING_FILL";
                                operation.GetBaseOperation()
                                    .RemoveProperty(luaName);
                                break;

                            case "Хранение":
                                luaName = "NEED_COOLING_DURING_STORING";
                                operation.GetBaseOperation()
                                    .RemoveProperty(luaName);
                                break;

                            case "Выдача":    
                                luaName = "NEED_COOLING_DURING_OUT";
                                operation.GetBaseOperation()
                                    .RemoveProperty(luaName);
                                break;

                            default:
                                break;
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Можно ли привязывать данный объект к другим объектам.
        /// </summary>
        public override bool IsAttachable
        {
            get
            {
                return true;
            }
        }

        #region сохранение prg.lua
        /// <summary>
        /// Сохранить информацию об операциях объекта в prg.lua
        /// </summary>
        /// <param name="objName">Имя объекта для записи</param>
        /// <param name="prefix">Отступ</param>
        /// <returns></returns>
        public override string SaveToPrgLua(string objName,
            string prefix)
        {
            var res = "";

            var modesManager = this.Owner.ModesManager;
            var modes = modesManager.Modes;
            foreach (Mode mode in modes)
            {
                var baseOperation = mode.GetBaseOperation();
                switch (baseOperation.Name)
                {
                    case "Охлаждение":
                        res += SaveCoolingOperation(prefix, objName, mode,
                            baseOperation);
                        break;
                }
            }

            res += "\n";
            return res;
        }

        /// <summary>
        /// Сохранить операцию охлаждения.
        /// </summary>
        /// <param name="prefix">Отступ</param>
        /// <param name="objName">Имя объекта</param>
        /// <param name="mode">Операция</param>
        /// <param name="baseOperation">Базовая операция</param>
        /// <returns></returns>
        private string SaveCoolingOperation(string prefix, string objName,
            Mode mode, BaseOperation baseOperation)
        {
            var res = "";

            res += objName + ".operations = \t\t--Операции.\n";
            res += prefix + "{\n";
            res += prefix + baseOperation.LuaName.ToUpper() + " = " + 
                mode.GetModeNumber() + ",\t\t--Охлаждение.\n";
            res += prefix + "}\n";

            return res;
        }
        #endregion
    }
}
