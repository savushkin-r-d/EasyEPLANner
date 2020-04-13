﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Базовый узел расхода
    /// </summary>
    public class BaseFlowNodePID : BaseTechObject
    {
        public BaseFlowNodePID() : base()
        {
            S88Level = 2;
            Name = "Узел расхода ПИД";
            EplanName = "flow_node_PID";
            BaseOperations = DataBase.Imitation.FlowNodePIDOperations();
            BaseProperties = DataBase.Imitation.EmptyProperties();
            BasicName = "flow_node_PID";
            Equipment = DataBase.Imitation.FlowNodePIDEquipment();
            AggregateProperties = DataBase.Imitation
                .FlowNodeAggregateProperties();
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
