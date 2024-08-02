using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Опции модификации устройств
    /// </summary>
    public interface IDevModifyOptions 
    {
        /// <summary>
        /// Тип модифицированного объекта - <b>Аппарат</b> <br/>
        /// </summary>
        /// <remarks>
        /// Используется при модификации MIXPROOF
        /// </remarks>
        bool IsUnit { get; }

        /// <summary>
        /// Модифицируется номер тех.объекта
        /// </summary>
        bool NumberModified { get; }

        /// <summary>
        /// Модифицируется ОУ тех.объекта
        /// </summary>
        bool NameModified { get; }

        /// <summary>
        /// Старое ОУ тех.объекта
        /// </summary>
        string OldTechObjectName { get; }

        /// <summary>
        /// Старый номер тех.объекта
        /// </summary>
        int OldTechObjectNumber { get; }

        /// <summary>
        /// Новое(текущее) ОУ тех.объекта
        /// </summary>
        string NewTechObjectName { get; }

        /// <summary>
        /// Новый(текущий) номер тех.объекта
        /// </summary>
        int NewTechObjectNumber { get; }
    }


    /// <summary>
    /// Опции модификации устройств
    /// </summary>
    public class DevModifyOptions : IDevModifyOptions
    {
        private TechObject techObject;
        private string oldTechObjectName;
        private int oldTechObjectNumber;

        /// <param name="techObject">Модифицируемый тех.объект</param>
        /// <param name="oldTechObjectName">Старое ОУ теъ.объекта</param>
        /// <param name="oldTechObjectNumber">Старый номер тех.объекта</param>
        public DevModifyOptions(TechObject techObject, string oldTechObjectName, int oldTechObjectNumber) 
        { 
            this.techObject = techObject;
            this.oldTechObjectName = oldTechObjectName;
            this.oldTechObjectNumber = oldTechObjectNumber;
        }

        public bool NumberModified => OldTechObjectNumber != NewTechObjectNumber;

        public bool NameModified => OldTechObjectName != NewTechObjectName;

        public bool IsUnit => techObject.BaseTechObject.S88Level == (int)BaseTechObjectManager.ObjectType.Unit;

        public string OldTechObjectName => oldTechObjectName;

        public int OldTechObjectNumber => oldTechObjectNumber;

        public string NewTechObjectName => techObject.NameEplan;

        public int NewTechObjectNumber => techObject.TechNumber;

    }
}
