using Eplan.EplApi.Base;
using TechObject;

namespace EasyEPlanner
{
    /// <summary>
    /// Синхронизатор устройств
    /// </summary>
    class DeviceSynchronizer
    {
        static DeviceSynchronizer()
        {
            synchronizeService = new DeviceSynchronizeService();
        }

        public DeviceSynchronizer()
        {
            deviceReader = new DeviceReader();
            techObjectManager = TechObjectManager.GetInstance();
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
            prevDevices = new Device.IODevice[devicesCount];
            deviceReader.CopyDevices(prevDevices);
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
        ///2.1. Если объект, находящийся в старом списке был уже удален, то 
        ///обрабатываем исключение. Устанавливаем флаг элемента старого списка 
        ///в -1.
        ///2.2. Пробуем проверить равенство объектов в двух списках устройств.
        ///Если элемент нового списка входит в старый, то проверяем равенство 
        ///их имен.
        ///3. Вызываем функцию синхронизации индексов, которая убирает
        ///удаленные устройства.
        /// </summary>
        private void SynchronizeDevices()
        {
            int prevDevicesCount = prevDevices.Length;
            int[] indexArray = new int[prevDevicesCount];                    //1            
            bool needSynch = false;

            for (int k = 0; k < prevDevicesCount; k++)                       //2
            {
                Device.IODevice prevDevice = prevDevices[k];
                var prevDevEplanObjFunc = prevDevice.EplanObjectFunction;

                if (prevDevEplanObjFunc == null ||
                    (k < deviceReader.DevicesCount &&
                    prevDevice.Name == deviceReader.Devices[k].Name))
                {
                    // Т.к если мы не заполним, то будет "0", а это съест другой
                    // алгоритм приняв за устройство.
                    indexArray[k] = -2;
                    continue;
                }

                needSynch = true;
                int idx = -1;
                foreach (Device.IODevice newDev in deviceReader.Devices)
                {
                    idx++;
                    const string deviceSkipSign = "1";                     //2.1
                    if (prevDevEplanObjFunc.IsValid != true ||
                        (prevDevEplanObjFunc.Properties
                        .FUNC_SUPPLEMENTARYFIELD[1].IsEmpty != true &&
                        prevDevEplanObjFunc.Properties
                        .FUNC_SUPPLEMENTARYFIELD[1]
                        .ToString(ISOCode.Language.L___) == deviceSkipSign))
                    {
                        indexArray[k] = -1;
                        break;
                    }

                    if (newDev.EplanObjectFunction == prevDevEplanObjFunc) //2.2
                    {
                        indexArray[k] = idx;
                        break;
                    }
                }
            }

            if (needSynch)                                                   //3
            {
                techObjectManager.Synch(indexArray);
            }
        }

        /// <summary>
        /// Получить сервис синхронизации устройств
        /// </summary>
        /// <returns></returns>
        public static IDeviceSynchronizeService GetSynchronizeService()
        {
            return synchronizeService;
        }

        private static IDeviceSynchronizeService synchronizeService;
        private Device.IODevice[] prevDevices;
        private DeviceReader deviceReader;
        private ITechObjectManager techObjectManager;
    }
}