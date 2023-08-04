using System.Collections.Generic;
using System.Linq;
using Editor;

namespace TechObject
{
    /// <summary>
    /// Все параметры технологической операции.
    /// </summary>
    public class OperationParams : TreeViewItem
    {
        public OperationParams()
        {
            items = new List<ITreeViewItem>();
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
        private static int CompareParams(ITreeViewItem firsItem, 
            ITreeViewItem seconditem)
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
        /// <param name="clone">Клонированная операция</param>
        /// <returns></returns>
        public OperationParams Clone(Mode clone)
        {
            var clonedOperationParams = new OperationParams();
            TechObject clonedObject = clone.Owner.Owner;
            Params objectParameters = clonedObject.GetParamsManager().Float;
            foreach(ITreeViewItem item in Items)
            {
                var operationParam = item as OperationParam;
                string luaName = operationParam.Param.GetNameLua();
                Param searchedParam = objectParameters.GetParam(luaName);
                if (searchedParam != null)
                {
                    clonedOperationParams.AddParam(searchedParam);
                }
            }

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

        override public ITreeViewItem[] Items
        {
            get
            {

                return items.ToArray();
            }
        }
        #endregion

        private List<ITreeViewItem> items;
    }
}
