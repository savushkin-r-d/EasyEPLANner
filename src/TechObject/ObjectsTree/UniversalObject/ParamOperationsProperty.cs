using System;
using System.Collections.Generic;
using System.Linq;
using Editor;

namespace TechObject
{
    /// <summary>
    /// Свойство, операции в которых участвует параметр
    /// </summary>
    class ParamOperationsProperty : ObjectProperty
    {
        public ParamOperationsProperty(string name, object value,
            object defaultValue) : base(name, value, defaultValue) { }

        public override bool SetNewValue(string newValue)
        {
            try
            {
                string[] numbersStrings = newValue.Trim().Split(' ');
                var numbers = new List<int>();
                foreach (string numberString in numbersStrings)
                {
                    bool isDigit = int.TryParse(numberString, out int number);
                    if (isDigit == false || number < -1)
                    {
                        throw new Exception();
                    }

                    if (numbersStrings.Length > 1 &&
                        numbersStrings.Contains("-1"))
                    {
                        throw new Exception();
                    }

                    numbers.Add(number);
                }

                SetOperationParameters(numbers);

                SetValue(newValue);
                return true;
            }
            catch
            {
                return false;
            }           
        }

        /// <summary>
        /// Установка параметров в операции
        /// </summary>
        /// <param name="modesNumbersWithParam">Список операций, в который
        /// надо добавить этот параметр</param>
        private void SetOperationParameters(List<int> modesNumbersWithParam)
        {
            ITreeViewItem parameter = Parent;
            ITreeViewItem parameters = parameter.Parent;
            ITreeViewItem parametersManager = parameters.Parent;
            ITreeViewItem techObject = parametersManager.Parent;
            ModesManager modesManager = (techObject as TechObject).ModesManager;
            List<Mode> modes = modesManager.Modes;

            if (modes.Count < modesNumbersWithParam.Max())
            {
                throw new Exception();
            }

            foreach (var mode in modes)
            {
                var modeParams = mode.GetOperationParams();
                bool isModeForAddParam = modesNumbersWithParam
                    .Contains(mode.GetModeNumber());
                if (isModeForAddParam)
                {
                    modeParams.AddParam(parameter as Param);
                }
                else
                {
                    modeParams.DeleteParam(parameter as Param);
                }
            }
        }
    }
}
