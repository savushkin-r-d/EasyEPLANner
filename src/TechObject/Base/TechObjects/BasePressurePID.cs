﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Базовый узел давления ПИД
    /// </summary>
    public class BasePressurePID : BaseTechObject
    {
        public BasePressurePID() : base()
        {
            S88Level = 2;
            Name = "Узел давления ПИД";
            EplanName = "pressure_node_PID";
            BaseOperations = DataBase.Imitation.PressureNodePIDOperations();
            BaseProperties = DataBase.Imitation.EmptyProperties();
            BasicName = "pressure_node_PID";
            Equipment = DataBase.Imitation.PressureNodePIDEquipment();
            AggregateProperties = DataBase.Imitation
                .PressureNodeAggregateProperties();
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

            res += SaveOperations(objName, prefix);
            res += SaveOperationsSteps(objName, prefix);
            res += SaveEquipment(objName);

            return res;
        }
        #endregion
    }
}
