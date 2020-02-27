using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Базовый пастеризатор
    /// </summary>
    public class BasePOU : BaseTechObject
    {
        public BasePOU() : base()
        {
            S88Level = 2;
            Name = "Пастеризатор";
            EplanName = "pasteurizator";
            BaseOperations = DataBase.Imitation.BaseEmptyOperations();
            BaseProperties = DataBase.Imitation.EmptyProperties();
            BasicName = "pasteurizator";
            Equipment = DataBase.Imitation.EmptyEquipment();
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
