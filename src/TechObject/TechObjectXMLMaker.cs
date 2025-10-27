using EasyEPlanner;
using EasyEPlanner.FileSavers.XML;
using Eplan.EplApi.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TechObject
{
    public class TechObjectXMLMaker
    {
        public TechObjectXMLMaker(ITechObjectManager techObjectManager)
        {
            this.techObjectManager = techObjectManager;
        }

        /// <summary>
        /// Формирование узлов для операций, шагов и параметров объектов.
        /// </summary>
        /// <param name="root">корневой узел</param>
        /// <param name="useNewNames">Использовать новые имена объектов
        /// вместо OBJECT</param>
        /// <param name="combineTags">Комбинировать тэги в один подтип</param>
        public void GetObjectForXML(IDriver root, bool combineTags, bool useNewNames)
        {
            cdbxNewNames = useNewNames;
            cdbxTagView = combineTags;

            GenerateSystemNode(root);
            foreach (var num in Enumerable.Range(1, techObjectManager.TechObjects.Count))
            {
                TechObject item = techObjectManager.TechObjects[num - 1];
                List<Mode> modes = item.ModesManager.Modes;

                var stDescription = $"{item.NameBC}{item.TechNumber}";
                string objName = GenerateObjectName(item, num);
                
                // Операции: .CMD; 
                root.AddChannel(stDescription + (cdbxTagView ? "" : "_Операции"), $"{objName}.CMD");
                // Операции: .ST (на 33 операции 1 .ST)
                foreach (var i in Enumerable.Range(0, (modes.Count / 33) + 1))
                    root.AddChannel(stDescription + (cdbxTagView ? "" : "_Операции"), $"{objName}.ST[ {i + 1} ]")
                        .Logged();

                // Операции: .MODES; .OPERATIONS; .AVAILABILITY; .MODES_STEPS;
                foreach (var i in Enumerable.Range(0, modes.Count))
                {
                    root.AddChannel(stDescription + (cdbxTagView ? "" : "_Операции"), $"{objName}.MODES[ {i + 1} ]")
                        .Logged();
                    root.AddChannel(stDescription + (cdbxTagView ? "" : "_Состояния_Операций"), $"{objName}.OPERATIONS[ {i + 1} ]")
                        .Logged();
                    root.AddChannel(stDescription + (cdbxTagView ? "" : "_Доступность"), $"{objName}.AVAILABILITY[ {i + 1} ]"   );
                    root.AddChannel(stDescription + (cdbxTagView ? "" : "_Одиночные_Шаги"), $"{objName}.MODES_STEPS[ {i + 1} ]")
                        .Logged();
                }

                // Шаги: .STATE_STEPS;
                foreach (var modeIdx in Enumerable.Range(0, modes.Count))
                {
                    foreach (var state in modes[modeIdx].States)
                    {
                        foreach (var stepIdx in Enumerable.Range(0, state.Steps.Count))
                        {
                            root.AddChannel(stDescription + (cdbxTagView ? "" : "_Одиночные_Шаги"),
                                $"{objName}.{state.Type}_STEPS{modeIdx + 1}[ {stepIdx + 1} ]")
                                .Logged();
                        }
                    }
                }

                // Параметры:
                foreach (var paramsGroup in item.GetParamsManager().Items.OfType<Params>())
                {
                    foreach (var param in paramsGroup.Parameters)
                    {
                        root.AddChannel(stDescription + (cdbxTagView ? "" : "_Параметры"),
                                $"{objName}.{paramsGroup.NameForChannelBase}[ {param.GetParameterNumber} ]");
                    }
                }

                // Параметры ПИД (совместимость со старыми версиями)
                if (item.BaseTechObject != null && item.BaseTechObject.IsPID)
                {
                    foreach (var i in Enumerable.Range(1, 2))
                    {
                        root.AddChannel($"PID{num}" + (cdbxTagView ? "" : "_Параметры"), $"PID{num}.RT_PAR_F[ {i} ]");
                    }

                    foreach (var i in Enumerable.Range(1, 14))
                    {
                        root.AddChannel($"PID{num}" + (cdbxTagView ? "" : "_Параметры"), $"PID{num}.S_PAR_F[ {i} ]");
                    }
                }
            }
        }

        /// <summary>
        /// Генерация системных тегов
        /// </summary>
        /// <param name="rootNode">Узловой узел</param>
        private void GenerateSystemNode(IDriver root)
        {
            root.AddChannel("SYSTEM", "SYSTEM.UP_TIME");
            root.AddChannel("SYSTEM", "SYSTEM.WASH_VALVE_SEAT_PERIOD");
            root.AddChannel("SYSTEM", "SYSTEM.P_V_OFF_DELAY_TIME"   );
            root.AddChannel("SYSTEM", "SYSTEM.WASH_VALVE_UPPER_SEAT_TIME");
            root.AddChannel("SYSTEM", "SYSTEM.WASH_VALVE_LOWER_SEAT_TIME");
            root.AddChannel("SYSTEM", "SYSTEM.CMD");
            root.AddChannel("SYSTEM", "SYSTEM.CMD_ANSWER");
            root.AddChannel("SYSTEM", "SYSTEM.P_RESTRICTIONS_MODE");
            root.AddChannel("SYSTEM", "SYSTEM.P_RESTRICTIONS_MANUAL_TIME"   );
            root.AddChannel("SYSTEM", "SYSTEM.P_AUTO_PAUSE_OPER_ON_DEV_ERR");
            root.AddChannel("SYSTEM", "SYSTEM.VERSION");
            
            var nodes = IO.IOManager.GetInstance().IONodes;
            foreach (var nodeNumber in Enumerable.Range(1, nodes.Count - 1))
            {
                root.AddChannel("SYSTEM", $"SYSTEM.NODEENABLED[ {nodeNumber + 1} ]");
            }
        }

        /// <summary>
        /// Генерация имени объекта
        /// </summary>
        /// <param name="item">Объект</param>
        /// <param name="itemNumber">Глобальный номер</param>
        /// <returns></returns>
        private string GenerateObjectName(TechObject item, int itemNumber)
        {
            if (cdbxNewNames == true)
            {
                return item.NameBC.ToUpper() + item.TechNumber.ToString();
            }
            else
            {
                return "OBJECT" + itemNumber.ToString();
            }
        }

        private ITechObjectManager techObjectManager;

        /// <summary>
        /// Использовать имена объектов в базе каналов вместо OBJECT
        /// </summary>
        private bool cdbxNewNames;

        /// <summary>
        /// Сгруппировать тэги в один подтип
        /// </summary>
        private bool cdbxTagView;
    }
}
