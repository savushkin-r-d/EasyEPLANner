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
        /// <param name="firsItem">Параметр 1</param>
        /// <param name="seconditem">Параметр 2</param>
        /// <returns></returns>
        private static int CompareParams(Editor.ITreeViewItem firsItem, 
            Editor.ITreeViewItem seconditem)
        {
            var firstParam = firsItem as OperationParam;
            var secondParam = seconditem as OperationParam;

            var firstNumberString = firstParam.DisplayText[0]
                .Split('.')
                .Where(x => x.Any(y => char.IsDigit(y)) == true)
                .FirstOrDefault();
            var secondNumberString = secondParam.DisplayText[0]
                .Split('.')
                .Where(x => x.Any(y => char.IsDigit(y)) == true)
                .FirstOrDefault();

            int.TryParse(firstNumberString, out int firstNumber);
            int.TryParse(secondNumberString, out int secondNumber);

            if (firstNumber > secondNumber)
            {
                return 1;
            }
            else if (firstNumber < secondNumber)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Копировать параметры операции
        /// </summary>
        /// <returns></returns>
        public OperationParams Clone()
        {
            var clonedOperationParams = new OperationParams();

            return clonedOperationParams;
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

                return items.ToArray();
            }
        }
        #endregion

        private List<Editor.ITreeViewItem> items;
    }
}
