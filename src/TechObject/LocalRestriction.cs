﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    public class LocalRestriction : Restriction
    {
        public LocalRestriction(string Name, string Value, string LuaName, 
            SortedDictionary<int, List<int>> dict) : base(Name, Value, 
                LuaName, dict)
        {
        }

        /// <summary>
        /// Установка новых значений ограничений для объектов с таким же типом
        /// </summary>
        /// <param name="dict">Словарь ограничений объекта, вызвавшего
        /// обновление</param>
        private void SetNewValueAtTheSameObjects(
            SortedDictionary<int, List<int>> dict)
        {
            var selectedLocalRestriction = this;
            var selectedRestrictionManager = selectedLocalRestriction.Parent;
            var selectedMode = selectedRestrictionManager.Parent as Mode;
            var selectedModesManager = selectedMode.Parent;
            var selectedTechObject = selectedModesManager.Parent as TechObject;
            var selectedTechObjectManager = selectedTechObject.Parent;

            const string restrictionName = "Ограничения внутри объекта";
            foreach(var item  in selectedTechObjectManager.Items)
            {
                var techObject = item as TechObject;
                if (techObject.TechType == selectedTechObject.TechType)
                {
                    var newDict = MakeSimilarObjectDictionary(dict, techObject);

                    var mode = techObject.ModesManager.Modes
                        .Where(x => x.Name == selectedMode.Name)
                        .FirstOrDefault();

                    var restrictions = mode.GetRestrictionManager()
                        .Restrictions
                        .Where(x => x.Name == restrictionName)
                        .FirstOrDefault();

                    restrictions.SetValue(newDict);
                }
            }
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
            SortedDictionary<int, List<int>> oldDict, TechObject techObject)
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
            newDict.Add( techObject.GlobalNumber , newOperations);
            
            return newDict;
        }

        /// <summary>
        /// Установка значений для ограничения
        /// </summary>
        /// <param name="dictionary">Словарь ограничений для установки</param>
        public override void SetValue(SortedDictionary<int, List<int>> 
            dictionary)
        {
            var oldRestriction = new SortedDictionary<int, List<int>>(
                restrictList);
            restrictList = null;
            restrictList = new SortedDictionary<int, List<int>>(dictionary);
            ChangeRestrictStr();

            SortedDictionary<int, List<int>> deletedRestriction =
                GetDeletedRestriction(oldRestriction);
            ClearCrossRestriction(deletedRestriction);
            SetCrossRestriction();
        }

        /// <summary>
        /// Установка новых ограничений
        /// </summary>
        /// <param name="dict">Новый словарь с ограничениями</param>
        /// <returns></returns>
        override public bool SetNewValue(SortedDictionary<int, List<int>> dict)
        {
            SortedDictionary<int, List<int>> oldRestriction =
                new SortedDictionary<int, List<int>>(restrictList);
            restrictList = null;
            restrictList = new SortedDictionary<int, List<int>>(dict);
            //Компануем строку для отображения
            ChangeRestrictStr();

            SortedDictionary<int, List<int>> deletedRestriction =
                GetDeletedRestriction(oldRestriction);
            ClearCrossRestriction(deletedRestriction);
            SetCrossRestriction();
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
