using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TechObject
{
    public class LocalRestriction : Restriction
    {
        public LocalRestriction(string name, string value, string luaName, 
            SortedDictionary<int, List<int>> dict) : base(name, value, 
                luaName, dict)
        {
        }

        /// <summary>
        /// Установка новых значений ограничений для объектов с таким же типом
        /// </summary>
        /// <param name="dict">Словарь ограничений объекта, вызвавшего
        /// обновление</param>
        private void SetNewValueAtTheSameObjects(
            IDictionary<int, List<int>> dict)
        {
            var selectedLocalRestriction = this;
            var selectedRestrictionManager = selectedLocalRestriction.Parent;
            var selectedMode = selectedRestrictionManager.Parent as Mode;
            var selectedModesManager = selectedMode.Parent;
            var selectedTechObject = selectedModesManager.Parent as TechObject;
            var selectedTechObjectManager = selectedTechObject.Parent;

            int objTechTypeNum = 0;
            foreach (TechObject techObject in selectedTechObjectManager.Items)
            {
                bool skipObjectBecauseTechType =
                    techObject.TechType != selectedTechObject.TechType;
                if (skipObjectBecauseTechType)
                {
                    continue;
                }

                var newDict = MakeSimilarObjectDictionary(dict, techObject);

                var mode = techObject.ModesManager.Modes
                    .Where(x => x.Name == selectedMode.Name)
                    .FirstOrDefault();
                if (mode != null)
                {
                    var restrictions = mode.GetRestrictionManager()
                        .Restrictions
                        .Where(x => x.Name == Name)
                        .FirstOrDefault();
                    try
                    {
                        restrictions?.SetValue(newDict);
                    }
                    catch
                    {
                        objTechTypeNum = techObject.TechType;
                    }
                }
            }

            bool hasErrors = objTechTypeNum != 0;
            if (hasErrors)
            {
                ShowCrossRestrictionWarning(objTechTypeNum);
            }
        }

        private void ShowCrossRestrictionWarning(int restrictionObjType)
        {
            string message = "Ошибка обработки перекрестных ограничений " +
                $"в объектах с типом {restrictionObjType} - " +
                "проверьте операции в перекрестных объектах.\n";
            MessageBox.Show(message , "Предупреждение",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Создать словарь ограничений из старого объекта для других объектов 
        /// такого же типа.
        /// </summary>
        /// <param name="oldDict">Словарь ограничений объекта, который 
        /// инициировал создание словаря</param>
        /// <param name="techObject">Объект, для которого создается словарь
        /// </param>
        /// <returns></returns>
        private SortedDictionary<int, List<int>> MakeSimilarObjectDictionary(
            IDictionary<int, List<int>> oldDict, TechObject techObject)
        {
            var newDict = new SortedDictionary<int, List<int>>();
            var newOperations = new List<int>();
            
            foreach(var item in oldDict.Values)
            {
                foreach(var value in item)
                {
                    newOperations.Add(value);
                }
            }

            int objGlobalNumber = TechObjectManager.GetInstance()
                        .GetTechObjectN(techObject);
            newDict.Add(objGlobalNumber, newOperations);
            
            return newDict;
        }

        /// <summary>
        /// Установка значений для ограничения
        /// </summary>
        /// <param name="dictionary">Словарь ограничений для установки</param>
        public override void SetValue(IDictionary<int, List<int>> dictionary)
        {
            var oldRestriction = CreateCopyOfRestrictList(restrictList);
            restrictList = CreateCopyOfRestrictList(dictionary);
            ChangeRestrictStr();

            SortedDictionary<int, List<int>> deletedRestriction =
                GetDeletedRestriction(oldRestriction);

            ClearCrossRestriction(deletedRestriction);

            try
            {
                SetCrossRestriction();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Установка новых ограничений
        /// </summary>
        /// <param name="dict">Новый словарь с ограничениями</param>
        /// <returns></returns>
        override public bool SetNewValue(IDictionary<int, List<int>> dict)
        {
            base.SetNewValue(dict);
            SetNewValueAtTheSameObjects(dict);
            return true;
        }

        override public bool IsLocalRestrictionUse
        {
            get
            {
                return true;
            }
        }
    }
}
