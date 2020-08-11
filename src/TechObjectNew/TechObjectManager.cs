using NewEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewTechObject
{
    /// <summary>
    /// Получение номера объекта в списке. Нумерация начинается с 1.
    /// </summary>
    public delegate int GetN(object obj);

    /// <summary>
    /// Менеджер объектов редактора.
    /// </summary>
    public class TechObjectManager : TreeViewItem, ITechObjectManager
    {
        private TechObjectManager()
        {
            objects = new List<ITreeViewItem>();
            objects.Add(new Master());
            objects.Add(new Unit());
            objects.Add(new Aggregate());
        }

        /// <summary>
        /// Получить экземпляр класса менеджера объектов
        /// </summary>
        /// <returns></returns>
        public static TechObjectManager GetInstance()
        {
            if (instance == null)
            {
                instance = new TechObjectManager();
            }

            return instance;
        }

        /// <summary>
         /// Получение объекта по номеру
         /// </summary>
         /// <param name="i">индекс </param>
         /// <returns></returns>
        public TechObject GetTObject(int i)
        {
            //if (objects != null)
            //{
            //    if (objects.Count >= i)
            //    {
            //        return objects[i - 1];
            //    }
            //}
            return null;
        }

        /// <summary>
        /// Получение номера операции в списке операций. 
        /// Нумерация начинается с 1.
        /// </summary>
        /// <param name="mode">Операция, номер которой хотим получить.</param>
        /// <returns>Номер заданной операции.</returns>
        public int GetTechObjectN(object techObject)
        {
            //return objects.IndexOf(techObject as TechObject) + 1;
            return 0;
        }

        /// <summary>
        /// Проверка и исправление ограничений при удалении/перемещении
        /// операции </summary>
        public void ChangeModeNum(int objNum, int prev, int curr)
        {
            //TODO: Переделать
            //foreach (TechObject to in objects)
            //{
            //    to.ChangeModeNum(objNum, prev, curr);
            //}
        }

        public List<TechObject> Objects
        {
            get
            {
                //TODO: Переделать
                return new List<TechObject>();
            }
        }

        #region реализация ITreeViewItem
        public override string[] DisplayText
        {
            get
            {
                string res = "\"" + ProjectName + "\"";
                if (objects.Count > 0)
                {
                    res += " (" + objects.Count + ")";
                }

                return new string[] { res, "" };
            }
        }

        public override ITreeViewItem[] Items
        {
            get
            {
                return objects.ToArray();
            }
        }

        public override ImageIndexEnum ImageIndex
        {
            get
            {
                return ImageIndexEnum.TechObjectManager;
            }
        }

        public override bool IsInsertable
        {
            get
            {
                return true;
            }
        }
        #endregion

        public string ProjectName { get;set; }

        private static TechObjectManager instance;
        private List<ITreeViewItem> objects;
    }
}
