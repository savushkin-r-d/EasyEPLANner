using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Все параметры технологической операции.
    /// </summary>
    public class OperationParams : Editor.TreeViewItem
    {
        public OperationParams()
        {
            items = new List<Editor.ITreeViewItem>();
        }

        public void AddParam(Param par)
        {
            bool containsThisParam = false;
            foreach (OperationParam param in items)
            {
                if (par.GetNameLua() == param.Param.GetNameLua())
                {
                    containsThisParam = true;
                }
            }

            if (!containsThisParam)
            {
                items.Add(new OperationParam(par));
                items.Sort(CompareParams);
            }
        }

        public void DeleteParam(Param paramForDelete)
        {
            foreach (OperationParam param in items)
            {
                if (paramForDelete.GetNameLua() == param.Param.GetNameLua())
                {
                    items.Remove(param);
                    break;
                }
            }
        }

        /// <summary>
        /// Сравнение между собой параметров операции
        /// </summary>
        /// <param name="x">Параметр 1</param>
        /// <param name="y">Параметр 2</param>
        /// <returns></returns>
        private static int CompareParams(Editor.ITreeViewItem x, 
            Editor.ITreeViewItem y)
        {
            var firstParam = x as OperationParam;
            var secondParam = y as OperationParam;

            //TODO: Получить название параметров 1 и 2.
            //TODO: Найти их реальные номера.
            //TODO: сравнить между собой и выдать результат.

            return 1;
        }

        #region Реализация ITreeViewItem
        override public string[] DisplayText
        {
            get
            {
                string res = string.Format("{0} ({1})", "Параметры", 
                    items.Count);
                return new string[] { res, "" };
            }
        }

        override public Editor.ITreeViewItem[] Items
        {
            get
            {
                if (items.Count > 0)
                {
                    return items.ToArray();
                }
                else
                {
                    return null;
                }
            }
        }
        #endregion

        private List<Editor.ITreeViewItem> items;
    }
}
