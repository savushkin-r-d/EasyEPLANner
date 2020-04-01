using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Базовый узел подогрева
    /// </summary>
    public class BaseHeaterPID : BaseTechObject
    {
        public BaseHeaterPID() : base()
        {
            S88Level = 2;
            Name = "Узел подогрева ПИД";
            EplanName = "heater_node_PID";
            BaseOperations = DataBase.Imitation.HeaterNodePIDOperations();
            BaseProperties = DataBase.Imitation.EmptyProperties();
            BasicName = "heater_node_PID";
            Equipment = DataBase.Imitation.HeaterNodePIDEquipment();
            AggregateProperties = DataBase.Imitation.EmptyAggregateProperties();
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
        /// Можно ли привязывать данный объект к другим объектам.
        /// </summary>
        public override bool IsAttachable
        {
            get
            {
                return true;
            }
        }

        #region Сохранение в prg.lua
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

            res += base.SaveOperations(objName, prefix);
            res += base.SaveEquipment(objName);

            return res;
        }
        #endregion
    }
}
