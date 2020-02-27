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
        /// Сохранить информацию об операциях объекта в prg.lua
        /// </summary>
        /// <param name="objName">Имя объекта для записи</param>
        /// <param name="prefix">Отступ</param>
        /// <returns></returns>
        public override string SaveOperationsToPrgLua(string objName,
            string prefix)
        {
            var res = "";
            return res;
        }
    }
}
