using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewTechObject
{
    /// <summary>
    /// Менеджер объектов редактора.
    /// </summary>
    public class TechObjectManager : ITechObjectManager
    {
        private TechObjectManager()
        {
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

        private static TechObjectManager instance;
    }
}
