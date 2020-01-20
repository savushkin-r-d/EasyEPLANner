using Eplan.EplApi.DataModel;
using Eplan.EplApi.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using StaticHelper;

namespace EasyEPlanner
{
    /// <summary>
    /// Класс для работы с конфигурацией проекта
    /// </summary>
    public class ProjectConfiguration
    {
        /// <summary>
        /// Закрытый конструктор.
        /// </summary>
        private ProjectConfiguration()
        {
            this.IOManager = IO.IOManager.GetInstance();
            this.deviceManager = Device.DeviceManager.GetInstance();
            this.techObjectManager = TechObject.TechObjectManager.GetInstance();
        }

        /// <summary>
        /// Получить экземпляр класса.
        /// </summary>
        public static ProjectConfiguration GetInstance()
        {
            return instance;
        }
  
        /// <summary>
        /// Синхронизация устройств в проекте с уже имеющимся описанием.
        /// 
        /// Делается слепок устройств которые есть сейчас, затем заново
        /// считываются устройства и сравнивается слепок устройств с текущими
        /// устройствами по алгоритму ниже.
        /// </summary>
        public void SynchronizeDevices()
        {
            int devicesCount = deviceManager.Devices.Count;
            Device.IODevice[] prevDevices = new Device.IODevice[devicesCount];
            deviceManager.Devices.CopyTo(prevDevices);

            ReadDevices();
            Synchronize(prevDevices);
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
        /// <param name="prevDevices">Предыдущий массив устройств</param>
        private void Synchronize(Device.IODevice[] prevDevices)
        {
            int prevDevicesCount = prevDevices.Length;
            int[] indexArray = new int[prevDevicesCount];                   //1            
            for (int i = 0; i < prevDevicesCount; i++)
            {
                indexArray[i] = 0;
            }

            bool needSynch = false;
            for (int k = 0; k < prevDevicesCount; k++)                      //2
            {
                Device.IODevice prevDevice = prevDevices[k];
                try
                {
                    if (prevDevice.EplanObjectFunction == null)
                    {
                        continue;
                    }

                    if (k < deviceManager.Devices.Count &&
                        prevDevice.Name == deviceManager.Devices[k].Name)
                    {
                        continue;
                    }

                    needSynch = true;
                    int idx = -1;
                    foreach (Device.IODevice newDev in deviceManager.Devices)
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

        /// <summary>
        /// Чтение конфигурации устройств.
        /// </summary>
        public void ReadDevices() 
        {
            //TODO: Временно, для тестов
            EplanDeviceManager.GetInstance().ReadConfigurationFromScheme();
        }

        /// <summary>
        /// Чтение конфигурации узлов и модулей ввода-вывода.
        /// </summary>
        public void ReadIO() 
        {

        }

        /// <summary>
        /// Чтение привязки устройств к модулям ввода-вывода.
        /// </summary>
        public void ReadBinding() 
        {

        }

        /// <summary>
        /// Возвращает словарь, содержащий привязку конкретного канала для
        /// его сброса
        /// </summary>
        public Dictionary<string, string> GetBindingForResettingChannel(
            Function deviceClampFunction, IO.IOModuleInfo moduleInfo, 
            string devicesDescription = "")
        {
            const string EmptyString = "";
            var res = new Dictionary<string, string>();
            string clampNumberAsString = deviceClampFunction.Properties
                .FUNC_ADDITIONALIDENTIFYINGNAMEPART.ToString();

            if (deviceClampFunction.Name.Contains("-Y"))
            {
                Function IOModuleFunction = ApiHelper
                    .GetIOModuleFunction(deviceClampFunction);
                string bindedDevice = deviceClampFunction.Name;
                Function IOModuleClampFunction = ApiHelper
                    .GetClampFunction(IOModuleFunction, bindedDevice);
                clampNumberAsString = IOModuleClampFunction.Properties
                .FUNC_ADDITIONALIDENTIFYINGNAMEPART.ToString();
            }

            int clampNumber;
            bool isDigit = int.TryParse(clampNumberAsString, out clampNumber);
            if (isDigit == false)
            {
                return res;
            }

            if (Array.IndexOf(moduleInfo.ChannelClamps, clampNumber) < 0)
            {
                return res;
            }

            DocumentTypeManager.DocumentType pageType = deviceClampFunction
                .Page.PageType;
            if (pageType != DocumentTypeManager.DocumentType.Circuit &&
                pageType != DocumentTypeManager.DocumentType.Overview)
            {
                return res;
            }

            if (devicesDescription == EmptyString)
            {
                devicesDescription = ApiHelper.GetFunctionalText(
                    deviceClampFunction);
            }

            if (devicesDescription == EmptyString)
            {
                return res;
            }

            var comment = EmptyString;
            var clampComment = EmptyString;
            Match actionMatch;
            var deviceEvaluator = new MatchEvaluator(RussianToEnglish);
            bool isMultipleBinding = deviceManager.IsMultipleBinding(
                devicesDescription);
            if (isMultipleBinding == false)
            {
                int endPos = devicesDescription.IndexOf("\n");
                if (endPos > 0)
                {
                    comment = devicesDescription.Substring(endPos + 1);
                    devicesDescription = devicesDescription.Substring(0, 
                        endPos);
                }

                devicesDescription = Regex.Replace(devicesDescription, 
                    RusAsEngPattern, deviceEvaluator);

                actionMatch = Regex.Match(comment, ChannelCommentPattern, 
                    RegexOptions.IgnoreCase);

                comment = Regex.Replace(comment, ChannelCommentPattern, 
                    EmptyString, RegexOptions.IgnoreCase);
                comment = comment.Replace("\n", ". ").Trim();
                if (comment.Length > 0 && comment[comment.Length - 1] != '.')
                {
                    comment += ".";
                }
            }
            else
            {
                devicesDescription = Regex.Replace(devicesDescription, 
                    RusAsEngPattern, deviceEvaluator);
                actionMatch = Regex.Match(comment, ChannelCommentPattern, 
                    RegexOptions.IgnoreCase);
            }

            var descrMatch = Regex.Match(devicesDescription, 
                Device.DeviceManager.BINDING_DEVICES_DESCRIPTION_PATTERN);
            while (descrMatch.Success)
            {
                string devName = descrMatch.Groups["name"].Value;

                if (actionMatch.Success)
                {
                    clampComment = actionMatch.Value;
                }

                res.Add(devName, clampComment);
                descrMatch = descrMatch.NextMatch();
            }

            return res;
        }

        /// <summary>
        /// Проверка конфигурации.
        /// </summary>
        /// <param name="silentMode">Тихий режим (без окна логов)</param>
        public void Check(bool silentMode = false) 
        {
            string errors;

            errors = deviceManager.Check();
            errors += IOManager.Check();
            errors += techObjectManager.Check();

            if (errors != string.Empty && silentMode == false)
            {
                ProjectManager.GetInstance().AddLogMessage(errors);
            }
        }

        /// <summary>
        /// MatchEvaluator для regular expression,
        /// замена русских букв на английские
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private string RussianToEnglish(Match m)
        {
            switch (m.ToString()[0])
            {
                case 'А':
                    return "A";
                case 'В':
                    return "B";
                case 'С':
                    return "C";
                case 'Е':
                    return "E";
                case 'К':
                    return "K";
                case 'М':
                    return "M";
                case 'Н':
                    return "H";
                case 'Х':
                    return "X";
                case 'Р':
                    return "P";
                case 'О':
                    return "O";
                case 'Т':
                    return "T";
            }

            return m.ToString();
        }

        /// <summary>
        /// Свойство, указывающее прочитаны устройства или нет.
        /// </summary>
        public bool DevicesIsRead
        {
            get
            {
                if (deviceManager.Devices.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        const string RusAsEngPattern = @"[АВСЕКМНХРОТ]";
        const string ChannelCommentPattern =
            @"(Открыть мини(?n:\s+|$))|" +
            @"(Открыть НС(?n:\s+|$))|" +
            @"(Открыть ВС(?n:\s+|$))|" +
            @"(Открыть(?n:\s+|$))|" +
            @"(Закрыть(?n:\s+|$))|" +
            @"(Открыт(?n:\s+|$))|" +
            @"(Закрыт(?n:\s+|$))|" +
            @"(Объем(?n:\s+|$))|" +
            @"(Поток(?n:\s+|$))|" +
            @"(Пуск(?n:\s+|$))|" +
            @"(Реверс(?n:\s+|$))|" +
            @"(Обратная связь(?n:\s+|$))|" +
            @"(Частота вращения(?n:\s+|$))|" +
            @"(Авария(?n:\s+|$))|" +
            @"(Напряжение моста\(\+Ud\)(?n:\s+|$))|" +
            @"(Референсное напряжение\(\+Uref\)(?n:\s+|$))";

        private IO.IOManager IOManager;
        private Device.DeviceManager deviceManager;
        private TechObject.TechObjectManager techObjectManager;

        static ProjectConfiguration instance = new ProjectConfiguration();

    }
}
