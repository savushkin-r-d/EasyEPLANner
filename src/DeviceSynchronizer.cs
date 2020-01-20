using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner
{
    /// <summary>
    /// Синхронизатор устройств
    /// </summary>
    class DeviceSynchronizer
    {
        public DeviceSynchronizer()
        {
            this.deviceReader = new DeviceReader();
            this.techObjectManager = TechObject.TechObjectManager
                .GetInstance();
        }

        /// Делается слепок устройств, которые есть сейчас, затем заново
        /// считываются устройства и сравнивается слепок устройств с текущими
        /// устройствами по алгоритму ниже.
        public void Synchronize()
        {
            PreparePreviouslyDevices();
            deviceReader.Read();
            SynchronizeDevices();
        }

        /// <summary>
        /// Подготовка слепка устройств, которые есть сейчас.
        /// </summary>
        private void PreparePreviouslyDevices()
        {
            int devicesCount = deviceReader.DevicesCount;
            previouslyDevices = new Device.IODevice[devicesCount];
            deviceReader.CopyDevices(previouslyDevices);
        }

        /// <summary>
        /// Синхронизация.
        /// 1.Создаем массив целочисленных флагов, количество элементов в 
        ///котором равняется количеству элементов в массиве ранее считанных 
        ///устройств. Все элементы массива флагов устанавливаются в 0. Далее 
        ///будем считать, что если флаг = 0, то индекс объекта не изменился, 
        ///если флаг = -1, то индекс объекта помечен на удаление, если 
        ///флаг > 0, то изменяем старый индекс в операции на значение флага.
        ///2. Для каждого элемента массива предыдущих устройств проверяем 
        ///соответствие элементу нового списка.
        ///2.1. Пробуем проверить равенство объектов в двух списках устройств.
        ///Если элемент нового списка входит в старый, то проверяем равенство 
        ///их имен.
        ///2.2.Если имена неравны, проверяем равенство их индексов в списках. 
        ///Индексы не совпадают - в массиве флагов изменяем соответствующий 
        ///элемент на новый индекс.
        ///2.3. Проверяем индексы одинаковых объектов, если они неравны, то 
        ///аналогично изменяем флаг на новый индекс.
        ///2.4. Если объект, находящийся в старом списке был уже удален, то 
        ///обрабатываем исключение. Устанавливаем флаг элемента старого списка 
        ///в -1.
        ///3. Вызываем функцию синхронизации индексов, которая убирает
        ///удаленные устройства.
        /// </summary>
        private void SynchronizeDevices()
        {
            int prevDevicesCount = previouslyDevices.Length;
            int[] indexArray = new int[prevDevicesCount];                   //1            
            for (int i = 0; i < prevDevicesCount; i++)
            {
                indexArray[i] = 0;
            }

            bool needSynch = false;
            for (int k = 0; k < prevDevicesCount; k++)                      //2
            {
                Device.IODevice prevDevice = previouslyDevices[k];
                try
                {
                    if (prevDevice.EplanObjectFunction == null)
                    {
                        continue;
                    }

                    if (k < deviceReader.DevicesCount &&
                        prevDevice.Name == deviceReader.Devices[k].Name)
                    {
                        continue;
                    }

                    needSynch = true;
                    int idx = -1;
                    foreach (Device.IODevice newDev in deviceReader.Devices)
                    {
                        idx++;
                        if (newDev.EplanObjectFunction == prevDevice      //2.1
                            .EplanObjectFunction)
                        {
                            indexArray[k] = idx;
                            break;
                        }
                    }
                }
                catch                                                     //2.4
                {
                    indexArray[k] = -1;
                }
            }

            if (needSynch)                                                  //3
            {
                techObjectManager.Synch(indexArray);
            }
        }

        Device.IODevice[] previouslyDevices;

        DeviceReader deviceReader;
        TechObject.TechObjectManager techObjectManager;
    }
}
