using Eplan.EplApi.Base;
using TechObject;

namespace EasyEPlanner
{
    /// <summary>
    /// Синхронизатор устройств
    /// </summary>
    public class DeviceSynchronizer
    {
        static DeviceSynchronizer()
        {
            synchronizeService = new DeviceSynchronizeService();
        }

        public DeviceSynchronizer(IDeviceReader deviceReader)
        {
            this.deviceReader = deviceReader;
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
            prevDevices = new EplanDevice.IODevice[devicesCount];
            deviceReader.CopyDevices(prevDevices);
        }

        /// <summary>
        /// Синхронизация.
        /// 1.Создаем массив целочисленных флагов, количество элементов в 
        ///котором равняется количеству элементов в массиве ранее считанных 
        ///устройств. Все элементы массива флагов устанавливаются в 0. Далее 
        ///будем считать, что если флаг = -2, то индекс объекта не изменился, 
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
            int deleteDeviceIndex = -1;
            int doNothingIndex = -2;
            const string deviceSkipSign = "1";

            for (int k = 0; k < prevDevicesCount; k++)                       //2
            {
                EplanDevice.IODevice prevDevice = prevDevices[k];
                var prevDeviceEplanFunc = prevDevice.EplanObjectFunction;

                bool addedNewDevice = prevDeviceEplanFunc == null;
                bool deviceNotChanged = k < deviceReader.DevicesCount &&
                    prevDevice.Name == deviceReader.Devices[k].Name;
                if (addedNewDevice || deviceNotChanged)
                {
                    //Если мы не заполним, то будет "0", а это съест другой
                    //алгоритм приняв за устройство.
                    indexArray[k] = doNothingIndex;
                    continue;
                }

                needSynch = true;
                int deviceIndex = -1;
                foreach (EplanDevice.IODevice newDevice in deviceReader.Devices)
                {
                    deviceIndex++;                                         //2.1
                    bool deviceDeleted = prevDeviceEplanFunc.IsValid == false;
                    if (deviceDeleted)
                    {
                        SetDeviceIndex(indexArray, k, deleteDeviceIndex);
                        break;
                    }
                    else
                    {
                        bool deviceOff = prevDeviceEplanFunc.Properties
                        .FUNC_SUPPLEMENTARYFIELD[1].IsEmpty == false &&
                        prevDeviceEplanFunc.Properties
                        .FUNC_SUPPLEMENTARYFIELD[1]
                        .ToString(ISOCode.Language.L___) == deviceSkipSign;
                        bool deviceMainFunctionOff =
                            prevDeviceEplanFunc.IsMainFunction == false;
                        if (deviceOff || deviceMainFunctionOff)
                        {
                            SetDeviceIndex(indexArray, k, deleteDeviceIndex);
                            break;
                        }
                    }   

                    bool foundNewDeviceIndex =
                        newDevice.EplanObjectFunction == prevDeviceEplanFunc;
                    if (foundNewDeviceIndex)                               //2.2
                    {
                        SetDeviceIndex(indexArray, k, deviceIndex);
                        break;
                    }
                }
            }

            if (needSynch)                                                   //3
            {
                techObjectManager.Synch(indexArray);
            }
        }

        private void SetDeviceIndex(int[] indexArray, int arrId, int devId)
            => indexArray[arrId] = devId;

        /// <summary>
        /// Получить сервис синхронизации устройств
        /// </summary>
        /// <returns></returns>
        public static IDeviceSynchronizeService GetSynchronizeService()
        {
            return synchronizeService;
        }

        private static IDeviceSynchronizeService synchronizeService;
        private EplanDevice.IODevice[] prevDevices;
        private IDeviceReader deviceReader;
        private ITechObjectManager techObjectManager;
    }
}