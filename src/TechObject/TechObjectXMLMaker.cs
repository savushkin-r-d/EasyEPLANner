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

                foreach(Params paramsGroup in item.GetParamsManager().Items)
                {
                    string groupName = $"{objName}." +
                        $"{paramsGroup.NameForChannelBase}";
                    int count = paramsGroup.Items.Length;
                    GenerateParametersTags(count, objNode, objParamsNode,
                        groupName);
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
            int nodesCount = IO.IOManager.GetInstance().IONodes.Count;
            // Первый узел - контроллер, его опускаем.
            int startValue = 2;
            for(int i = startValue; i <= nodesCount; i++)
            {
                node.Nodes.Add($"SYSTEM.NODEENABLED[ {i} ]",
                    $"SYSTEM.NODEENABLED[ {i} ]");
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
            for (int i = 1; i <= item.ModesManager.Modes.Count; i++)
            {
                string number = "[ " + i.ToString() + " ]";
                if (cdbxTagView == true)
                {
                    objNode.Nodes.Add(mode + number, mode + number);
                    objNode.Nodes.Add(oper + number, oper + number);
                    objNode.Nodes.Add(av + number, av + number);
                    objNode.Nodes.Add(step + number, step + number);
                }
                else
                {
                    objModesNode.Nodes.Add(mode + number, mode + number);
                    objOperStateNode.Nodes.Add(oper + number, oper + number);
                    objAvOperNode.Nodes.Add(av + number, av + number);
                    objStepsNode.Nodes.Add(step + number, step + number);
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
            List<Mode> modes = techObject.ModesManager.Modes;
            for (int modeNum = 1; modeNum <= modes.Count; modeNum++)
            {
                foreach(var state in modes[modeNum - 1].States)
                {
                    int stepsCount = state.Steps.Count;
                    for (int stepNum = 1; stepNum <= stepsCount; stepNum++)
                    {
                        string stepTag = $"{objName}." +
                            $"{state.Type}_STEPS{modeNum}[ {stepNum} ]";
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
        private void GenerateParametersTags(int paramsCount, TreeNode objNode,
            TreeNode objParamsNode, string tagName)
        {
            for (int i = 1; i <= paramsCount; i++)
            {
                string number = "[ " + i.ToString() + " ]";
                string fullTagName = tagName + number;
                if (cdbxTagView == true)
                {
                    objNode.Nodes.Add(fullTagName, fullTagName);
                }
                else
                {
                    objParamsNode.Nodes.Add(fullTagName, fullTagName);
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
