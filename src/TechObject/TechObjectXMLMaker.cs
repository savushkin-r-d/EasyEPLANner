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
        /// <param name="rootNode">корневой узел</param>
        /// <param name="useNewNames">Использовать новые имена объектов
        /// вместо OBJECT</param>
        /// <param name="combineTags">Комбинировать тэги в один подтип</param>
        public void GetObjectForXML(TreeNode rootNode, bool combineTags,
            bool useNewNames)
        {
            cdbxNewNames = useNewNames;
            cdbxTagView = combineTags;

            GenerateSystemNode(rootNode);
            for (int num = 1; num <= techObjectManager.TechObjects.Count; num++)
            {
                TechObject item = techObjectManager.TechObjects[num - 1];

                var objNode = new TreeNode($"{item.NameBC}{item.TechNumber}");

                var objModesNode = new TreeNode(item.NameBC +
                    item.TechNumber.ToString() + "_Операции");
                var objOperStateNode = new TreeNode(item.NameBC +
                    item.TechNumber.ToString() + "_Состояния_Операций");
                var objAvOperNode = new TreeNode(item.NameBC +
                    item.TechNumber.ToString() + "_Доступность");
                var objStepsNode = new TreeNode(item.NameBC +
                    item.TechNumber.ToString() + "_Шаги");
                var objSingleStepsNode = new TreeNode(item.NameBC +
                    item.TechNumber.ToString() + "_Одиночные_Шаги");
                var objParamsNode = new TreeNode(item.NameBC +
                    item.TechNumber.ToString() + "_Параметры");

                string objName = GenerateObjectName(item, num);
                GenerateCMDTags(objName, objNode, objModesNode);
                GenerateSTTags(item, objName, objNode, objModesNode);
                GenerateModesOpersAvsStepsTags(item, objName, objNode,
                    objModesNode, objOperStateNode, objAvOperNode,
                    objStepsNode);

                GenerateSingleStepsTags(item, objName, objNode,
                    objSingleStepsNode);

                foreach (var paramsGroup in item.GetParamsManager().Items.OfType<Params>())
                {
                    GenerateParametersTags(objName, paramsGroup, objNode, objParamsNode);
                }

                var singleNodes = new TreeNode[] { objModesNode,
                    objOperStateNode, objAvOperNode, objStepsNode,
                    objSingleStepsNode, objParamsNode};
                GenerateRootNode(rootNode, objNode, singleNodes);

                if (item.BaseTechObject != null && item.BaseTechObject.IsPID)
                {
                    GeneratePIDNode(rootNode, 
                        techObjectManager.GetTechObjectN(item));
                }
            }
        }

        /// <summary>
        /// Генерация системных тегов
        /// </summary>
        /// <param name="rootNode">Узловой узел</param>
        private void GenerateSystemNode(TreeNode rootNode)
        {
            var systemNode = new TreeNode("SYSTEM");
            systemNode.Nodes.Add("SYSTEM.UP_TIME", "SYSTEM.UP_TIME");
            systemNode.Nodes.Add("SYSTEM.WASH_VALVE_SEAT_PERIOD",
                "SYSTEM.WASH_VALVE_SEAT_PERIOD");
            systemNode.Nodes.Add("SYSTEM.P_V_OFF_DELAY_TIME",
                "SYSTEM.P_V_OFF_DELAY_TIME");
            systemNode.Nodes.Add("SYSTEM.WASH_VALVE_UPPER_SEAT_TIME",
                "SYSTEM.WASH_VALVE_UPPER_SEAT_TIME");
            systemNode.Nodes.Add("SYSTEM.WASH_VALVE_LOWER_SEAT_TIME",
                "SYSTEM.WASH_VALVE_LOWER_SEAT_TIME");
            systemNode.Nodes.Add("SYSTEM.CMD", "SYSTEM.CMD");
            systemNode.Nodes.Add("SYSTEM.CMD_ANSWER", "SYSTEM.CMD_ANSWER");
            systemNode.Nodes.Add("SYSTEM.P_RESTRICTIONS_MODE",
                "SYSTEM.P_RESTRICTIONS_MODE");
            systemNode.Nodes.Add("SYSTEM.P_RESTRICTIONS_MANUAL_TIME",
                "SYSTEM.P_RESTRICTIONS_MANUAL_TIME");
            systemNode.Nodes.Add("SYSTEM.P_AUTO_PAUSE_OPER_ON_DEV_ERR",
                "SYSTEM.P_AUTO_PAUSE_OPER_ON_DEV_ERR");
            systemNode.Nodes.Add("SYSTEM.VERSION",
                "SYSTEM.VERSION");
            GenerateIONodesEnablingTags(systemNode);
            rootNode.Nodes.Add(systemNode);
        }

        /// <summary>
        /// Генерация тэгов для управления узлами контроллера
        /// </summary>
        /// <param name="node">Системный узел дерева</param>
        private void GenerateIONodesEnablingTags(TreeNode node)
        {
            var nodes = IO.IOManager.GetInstance().IONodes;
            
            // Первый узел - контроллер, его опускаем.
            for(int i = 1; i < nodes.Count; i++)
            {
                var text = $"SYSTEM.NODEENABLED[ {i + 1} ] -- {nodes[i].Name} {nodes[i].TypeStr}";
                node.Nodes.Add(text, text);
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

        /// <summary>
        /// Генерация CMD-тэгов для объекта
        /// </summary>
        /// <param name="obj">Имя объекта</param>
        private void GenerateCMDTags(string obj, TreeNode objNode,
            TreeNode objModesNode)
        {
            string tagName = obj + ".CMD";
            if (cdbxTagView == true)
            {
                objNode.Nodes.Add(tagName, tagName);
            }
            else
            {
                objModesNode.Nodes.Add(tagName, tagName);
            }
        }

        /// <summary>
        /// Генерация ST-тегов для проекта
        /// </summary>
        /// <param name="item">Объект</param>
        /// <param name="objName">Имя объекта</param>
        private void GenerateSTTags(TechObject item, string objName,
            TreeNode objNode, TreeNode objModesNode)
        {
            // 33 - Magic number
            int stCount = item.ModesManager.Modes.Count / 33;
            for (int i = 0; i <= stCount; i++)
            {
                string number = "[ " + (i + 1).ToString() + " ]";
                string fullTagName = objName + ".ST" + number;
                if (cdbxTagView == true)
                {
                    objNode.Nodes.Add(fullTagName, fullTagName);
                }
                else
                {
                    objModesNode.Nodes.Add(fullTagName, fullTagName);
                }
            }
        }

        /// <summary>
        /// Генерация тэгов по операциям, шагам, доступности, состояниям
        /// </summary>
        /// <param name="item">Объект</param>
        /// <param name="itemNumber">Глобальный номер</param>
        private void GenerateModesOpersAvsStepsTags(TechObject item, string obj,
            TreeNode objNode, TreeNode objModesNode, TreeNode objOperStateNode,
            TreeNode objAvOperNode, TreeNode objStepsNode)
        {
            string mode = obj + ".MODES";
            string step = mode + "_STEPS";
            string oper = obj + ".OPERATIONS";
            string av = obj + ".AVAILABILITY";

            var modes = item.ModesManager.Modes;
            for (int i = 0; i < modes.Count; ++i)
            {
                string number = $"[ {i + 1} ]";
                string modesTag = $"{mode}{number} -- {modes[i].Name}";
                string operationsTag = $"{oper}{number} -- {modes[i].Name}";
                string availabilityTag = $"{av}{number} -- {modes[i].Name}";
                string stepsTag = $"{step}{number} -- {modes[i].Name}";
                if (cdbxTagView)
                {
                    objNode.Nodes.Add(modesTag, modesTag);
                    objNode.Nodes.Add(operationsTag, operationsTag);
                    objNode.Nodes.Add(availabilityTag, availabilityTag);
                    objNode.Nodes.Add(stepsTag, stepsTag);
                }
                else
                {
                    objModesNode.Nodes.Add(modesTag, modesTag);
                    objOperStateNode.Nodes.Add(operationsTag, operationsTag);
                    objAvOperNode.Nodes.Add(availabilityTag, availabilityTag);
                    objStepsNode.Nodes.Add(stepsTag, stepsTag);
                }
            }
        }

        /// <summary>
        /// Генерация одиночных шагов для объекта
        /// </summary>
        /// <param name="techObject">Объект</param>
        /// <param name="objName">Имя объекта</param>
        /// <param name="objNode">Дерево объекта (пишется либо сюда)</param>
        /// <param name="objSingleStepsNode">Дерево для одиночных шагов 
        /// (либо сюда)</param>
        private void GenerateSingleStepsTags(TechObject techObject,
            string objName, TreeNode objNode, TreeNode objSingleStepsNode)
        {
            var modes = techObject.ModesManager.Modes;
            for (int modeIdx = 0; modeIdx < modes.Count; ++modeIdx)
            {
                foreach(var state in modes[modeIdx].States)
                {
                    var steps = state.Steps;
                    for (int stepIdx = 0; stepIdx < steps.Count; ++stepIdx)
                    {
                        string stepTag = $"{objName}.{state.Type}_STEPS{modeIdx + 1}[ {stepIdx + 1} ] -- {modes[modeIdx].Name} - {state.Name} - {steps[stepIdx].GetStepName()}";
                        if (cdbxTagView)
                        {
                            objNode.Nodes.Add(stepTag, stepTag);
                        }
                        else
                        {
                            objSingleStepsNode.Nodes.Add(stepTag, stepTag);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Генерация тэгов параметров объекта
        /// </summary>
        /// <param name="paramsCount">Количество параметров</param>
        /// <param name="objNode"></param>
        /// <param name="tagName">Имя тэга</param>
        private void GenerateParametersTags(string objName, Params parametersGroup, TreeNode objNode,
            TreeNode objParamsNode)
        {
            string tagPrefix = $"{objName}.{parametersGroup.NameForChannelBase}";
            var parameters = parametersGroup.Items.OfType<Param>().ToList();
            for (int i = 0; i < parameters.Count; ++i)
            {
                string tag = $"{tagPrefix}[ {i + 1} ]";
                if (!parameters[i].IsStub)
                    tag += $" -- { parameters[i].GetName()}";

                if (cdbxTagView)
                {
                    objNode.Nodes.Add(tag, tag);
                }
                else
                {
                    objParamsNode.Nodes.Add(tag, tag);
                }
            }
        }

        /// <summary>
        /// Генерация главного узла для экспорта в XML
        /// </summary>
        private void GenerateRootNode(TreeNode rootNode, TreeNode objNode,
            TreeNode[] singleNodes)
        {
            if (cdbxTagView == true)
            {
                rootNode.Nodes.Add(objNode);
            }
            else
            {
                rootNode.Nodes.AddRange(singleNodes);
            }
        }

        /// <summary>
        /// Генерация объекта-ПИДа
        /// </summary>
        /// <param name="rootNode">Главный узел</param>
        private void GeneratePIDNode(TreeNode rootNode, int num)
        {
            string tagName = $"PID{num}";
            TreeNode pidNode;
            if (cdbxTagView == true)
            {
                pidNode = new TreeNode($"{tagName}");
            }
            else
            {
                pidNode = new TreeNode($"{tagName}_Параметры");
            }

            const int rtParCount = 2;
            for (int i = 1; i <= rtParCount; i++)
            {
                string nodeDescription = $"{tagName}.RT_PAR_F[ {i} ]";
                pidNode.Nodes.Add(nodeDescription, nodeDescription);
            }

            const int sParCount = 14;
            for (int i = 1; i <= sParCount; i++)
            {
                string nodeDescription = $"{tagName}.S_PAR_F[ {i} ]";
                pidNode.Nodes.Add(nodeDescription, nodeDescription);
            }

            rootNode.Nodes.Add(pidNode);
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
