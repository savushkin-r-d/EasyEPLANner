using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner
{
    /// <summary>
    /// Класс для работы с конфигурацией проекта
    /// </summary>
    public class ProjectConfiguration
    {
        /// <summary>
        /// Синхронизация устройств в проекте
        /// </summary>
        /// <param name="deviesIsReaded"></param>
        public void SynchronizeDevices(bool deviesIsReaded)
        {

        }

        /// <summary>
        /// Чтение конфигурации устройств
        /// </summary>
        public void ReadDevices() 
        {

        }

        /// <summary>
        /// Чтение конфигурации узлов и модулей ввода-вывода
        /// </summary>
        public void ReadIO() 
        {

        }

        /// <summary>
        /// Чтение привязки устройств к модулям ввода-вывода
        /// </summary>
        public void ReadBinding() 
        {

        }

        /// <summary>
        /// Проверка конфигурации
        /// </summary>
        public void Check() 
        {

        }

        /// <summary>
        /// Получить экземпляр класса
        /// </summary>
        public static ProjectConfiguration GetInstance()
        {
            return instance;
        }

        static ProjectConfiguration instance = new ProjectConfiguration();
    }
}
