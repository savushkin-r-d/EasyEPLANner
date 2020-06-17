using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EasyEPlanner
{
    /// <summary>
    /// Межконтроллерный обмен сигналами. Стартовый класс
    /// </summary>
    public class InterprojectExchangeStarter
    {
        private InterprojectExchangeStarter()
        {
            interprojectExchange = InterprojectExchange.GetInstance();
            deviceComparer = new DeviceComparer();
        }

        /// <summary>
        /// Начало настройки межконтроллерного обмена.
        /// </summary>
        public static void Start()
        {
            var instance = GetInstance();
            bool isReadSignals = instance.ReadDevices();
            bool isLoadData = instance
                .LoadCurrentInterprojectExchange(isReadSignals);
            instance.ShowForm(isLoadData);
        }

        /// <summary>
        /// Загрузка текущих устройств.
        /// </summary>
        private bool ReadDevices()
        {
            bool saveDescrSilentMode = true;
            EProjectManager.GetInstance().SyncAndSave(saveDescrSilentMode);
            return true;
        }

        /// <summary>
        /// Загрузить текущие данные по межпроектному обмену сигналами.
        /// </summary>
        /// <param name="isReadDevices">Прочитаны ли устройства текущие</param>
        private bool LoadCurrentInterprojectExchange(bool isReadDevices)
        {
            if (isReadDevices == false)
            {
                return false;
            }

            var projName = EProjectManager.GetInstance()
                .GetCurrentProjectName();
            var devices = new List<DeviceDTO>();
            foreach(var dev in Device.DeviceManager.GetInstance().Devices)
            {
                var devDTO = new DeviceDTO(dev.Name, dev.EPlanName, 
                    dev.Description);
                devices.Add(devDTO);
            }

            devices.Sort(deviceComparer);

            interprojectExchange.LoadModel(projName, devices);

            return true;
            //TODO: чтение данных по проекту и перекрестных данных
        }

        /// <summary>
        /// Показать форму для работы с межпроектным обменом.
        /// </summary>
        private void ShowForm(bool isLoaded)
        {
            if (isLoaded == false)
            {
                return;
            }

            if (form == null || form.IsDisposed)
            {
                form = new InterprojectExchangeForm();
            }
            form.ShowDialog();
        }

        /// <summary>
        /// Получить экземпляр класса. Singleton
        /// </summary>
        /// <returns></returns>
        public static InterprojectExchangeStarter GetInstance()
        {
            if (interprojectExchangeStarter == null)
            {
                interprojectExchangeStarter = new InterprojectExchangeStarter();
            }

            return interprojectExchangeStarter;
        }

        private InterprojectExchangeForm form;
        private InterprojectExchange interprojectExchange;
        private static InterprojectExchangeStarter interprojectExchangeStarter;
        private DeviceComparer deviceComparer;
    }

    /// <summary>
    /// Компаратор для устройств
    /// </summary>
    class DeviceComparer : IComparer<DeviceDTO>
    {
        public int Compare(DeviceDTO dev1, DeviceDTO dev2)
        {
            var device1 = Regex.Match(dev1.EplanName, regexPattern);
            var device2 = Regex.Match(dev2.EplanName, regexPattern);
            if (!(device1.Success || device2.Success))
            {
                return 0;
            }

            string dev1Obj = device1.Groups["object"].Value;
            string dev2Obj = device2.Groups["object"].Value;
            if (dev1Obj != dev2Obj)
            {
                return dev1Obj.CompareTo(dev2Obj);
            }

            string dev1ObjNumStr = device1.Groups["object_n"].Value;
            string dev2ObjNumStr = device2.Groups["object_n"].Value;
            if (dev1ObjNumStr != dev2ObjNumStr)
            {
                int dev1ObjNum = int.Parse(dev1ObjNumStr);
                int dev2ObjNum = int.Parse(dev2ObjNumStr);
                return dev1ObjNum.CompareTo(dev2ObjNum);
            }

            string dev1Type = device1.Groups["type"].Value;
            string dev2Type = device2.Groups["type"].Value;
            if (dev1Type != dev2Type)
            {
                return dev1Type.CompareTo(dev2Type);
            }

            string dev1NumStr = device1.Groups["n"].Value;
            string dev2NumStr = device2.Groups["n"].Value;
            if (dev1NumStr != dev2NumStr)
            {
                int dev1Num = int.Parse(dev1NumStr);
                int dev2Num = int.Parse(dev2NumStr);
                return dev1Num.CompareTo(dev2Num);
            }

            return 0;
        }

        public string regexPattern = Device.DeviceManager.DESCRIPTION_PATTERN;
    }
}
