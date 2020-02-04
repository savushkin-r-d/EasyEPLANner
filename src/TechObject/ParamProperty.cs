using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Свойство параметра
    /// </summary>
    class ParamProperty : Editor.ObjectProperty
    {
        public ParamProperty(string name, object value) : base(name, value)
        {

        }

        public override bool SetNewValue(string newValue)
        {
            try
            {
                string[] numbersStrings = newValue.Split(' ');
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

                base.SetValue(newValue);
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
        public void SetOperationParameters(List<int> modesNumbersWithParam)
        {
            var parameter = this.Parent;
            var parameters = parameter.Parent;
            var parametersManager = parameters.Parent;
            var techObject = parametersManager.Parent;
            var modesManager = (techObject as TechObject).ModesManager;
            var modes = modesManager.Modes;

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
