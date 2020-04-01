using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Базовый узел перемешивания
    /// </summary>
    public class BaseMixer : BaseTechObject
    {
        public BaseMixer() : base()
        {
            S88Level = 2;
            Name = "Узел перемешивания";
            EplanName = "mix_node";
            BaseOperations = DataBase.Imitation.BaseEmptyOperations();
            BaseProperties = DataBase.Imitation.EmptyProperties();
            BasicName = "mix_node";
            Equipment = DataBase.Imitation.MixerEquipment();
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

            res += base.SaveOperations(objName, prefix);
            res += base.SaveEquipment(objName);

            return res;
        }
        #endregion
    }
}
