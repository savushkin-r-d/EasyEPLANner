using System.Text;
using System.IO;
using TechObject;

namespace EasyEPlanner
{
    /// <summary>
    /// Класс, сохраняющий статистику в картинках SVG
    /// </summary>
    public static class SVGStatisticsSaver
    {
        public static void Save(string path)
        {
            string pathToFiles = path + @"\docs\statistics\";
            svgFilePattern = Properties.Resources.ResourceManager
            .GetString("svgPattern");
            if (Directory.Exists(pathToFiles) == false)
            {
                Directory.CreateDirectory(pathToFiles);
            }

            SaveLOC(pathToFiles,path);
            SaveTagsCount(pathToFiles);
            SaveUnintsCount(pathToFiles);
            SaveEquipmentModulesCount(pathToFiles);
            SaveDevicesCount(pathToFiles);
            SaveIOLinkDevicesPercentage(pathToFiles);
        }

        /// <summary>
        /// Сохранить количество строк кода main.plua в SVG.
        /// </summary>
        /// <param name="folderPath">Путь к каталогу</param>
        /// <param name="locFilePath">Пусть к файлу, для которого надо 
        /// сохранить LOC</param>
        private static void SaveLOC(string folderPath, string locFilePath)
        {
            const int maxLOCCount = 1000;

            folderPath += linesOfCodeMainProgramFileName;
            locFilePath += @"\main.plua";

            string[] readedFile = File.ReadAllLines(locFilePath, 
                Encoding.GetEncoding(1251));
            int loc = readedFile.Length;
            string displayingText = $"{loc} строк кода";
            string result = MakeStringForWriting(loc, maxLOCCount,
                displayingText);
            WriteFile(result, folderPath);
        }

        /// <summary>
        /// Сохранить количество тэгов в SVG.
        /// </summary>
        /// <param name="folderPath">Путь к каталогу</param>
        private static void SaveTagsCount(string folderPath)
        {
            const int maxTagsCount = 5000;

            folderPath += countOfTagsFileName;
            int tagsCount = XMLReporter.GetTagsCount();
            string displayingText = $"{tagsCount} тэг(ов)";
            string result = MakeStringForWriting(tagsCount, maxTagsCount,
                displayingText);
            WriteFile(result, folderPath);
        }

        /// <summary>
        /// Сохранить количество аппаратов в SVG.
        /// </summary>
        /// <param name="folderPath">Путь к каталогу</param>
        private static void SaveUnintsCount(string folderPath)
        {
            const int maxUnitsCount = 10;

            folderPath += countOfUnitsFileName;
            int unitsCount = techObjectManager.UnitsCount;
            string displayingText = $"{unitsCount} аппарат(ов)";
            string result = MakeStringForWriting(unitsCount, maxUnitsCount,
                displayingText);
            WriteFile(result, folderPath);
        }

        /// <summary>
        /// Сохранить количество агрегатов в SVG.
        /// </summary>
        /// <param name="folderPath">Путь к каталогу</param>
        private static void SaveEquipmentModulesCount(string folderPath)
        {
            const int maxEquipCount = 50;

            folderPath += countOfEquipmentModulesFileName;
            int equipCount = techObjectManager.EquipmentModulesCount;
            string displayingText = $"{equipCount} агрегат(ов)";
            string result = MakeStringForWriting(equipCount, maxEquipCount,
                displayingText);
            WriteFile(result, folderPath);
        }

        /// <summary>
        /// Сохранить количество устройств в SVG.
        /// </summary>
        /// <param name="folderPath">Путь к каталогу</param>
        private static void SaveDevicesCount(string folderPath)
        {
            const int maxDevicesCount = 1000;

            folderPath += countOfDevicesFileName;
            int devicesCount = deviceManager.Devices.Count;
            string displayingText = $"{devicesCount} устройств";
            string result = MakeStringForWriting(devicesCount, maxDevicesCount,
                displayingText);
            WriteFile(result, folderPath);
        }

        /// <summary>
        /// Сохранить количество IO-Link модулей в процентном соотношении
        /// к общему количеству в SVG.
        /// </summary>
        /// <param name="folderPath">Путь к каталогу</param>
        private static void SaveIOLinkDevicesPercentage(string folderPath)
        {
            int modulesCount = 0;
            int ioLinkModules = 0;
            foreach(var node in ioManager.IONodes)
            {
                modulesCount += node.IOModules.Count;
                foreach(var module in node.IOModules)
                {
                    if(module.isIOLink())
                    {
                        ioLinkModules++;
                    }
                }
            }
            int valueInPercents = ValueAsPercentage(ioLinkModules,
                modulesCount);
            string displayingText = $"{valueInPercents}% IO-Link";
            string result = MakeStringForWriting(ioLinkModules,
                modulesCount, displayingText);
            folderPath += ioModulesInPercentage;
            WriteFile(result, folderPath);
        }

        /// <summary>
        /// Сделать текст для записи в файл
        /// </summary>
        /// <param name="itemsCount">Количество элементов</param>
        /// <param name="maxItemsCount">Максимальное количество элементов
        /// </param>
        /// <param name="displayingText">Отображаемый текст</param>
        /// <returns></returns>
        private static string MakeStringForWriting(int itemsCount, 
            int maxItemsCount, string displayingText)
        {
            int currentValue = ValueAsPercentage(itemsCount, maxItemsCount);
            string result = string.Format(svgFilePattern, percents,
                currentValue, displayingText);
            return result;
        }

        /// <summary>
        /// Расчет процентного соотношения параметров
        /// </summary>
        /// <param name="currentValue">Текущее значение параметры</param>
        /// <param name="maxValue">Максимальное значение параметра</param>
        /// <returns></returns>
        private static int ValueAsPercentage(int currentValue, int maxValue)
        {
            int result;
            result = (currentValue * percents) / maxValue;
            return result;
        }

        /// <summary>
        /// Запись файла со статистикой.
        /// </summary>
        /// <param name="text">Текст для записи</param>
        /// <param name="folderPath">Путь для записи</param>
        private static void WriteFile(string text, string folderPath)
        {
            var equipmentWriter = new StreamWriter(folderPath, false,
                Encoding.UTF8);
            equipmentWriter.WriteLine(text);
            equipmentWriter.Flush();
            equipmentWriter.Close();
        }

        static string svgFilePattern;

        static string linesOfCodeMainProgramFileName = "lines_total.svg";
        static string countOfTagsFileName = "tags_total.svg";
        static string countOfUnitsFileName = "units_total.svg";
        static string countOfEquipmentModulesFileName = "agregates_total.svg";
        static string countOfDevicesFileName = "devices_total.svg";
        static string ioModulesInPercentage = "io_link_usage.svg";

        /// <summary>
        /// 100% длина линии SVG. 
        /// </summary>
        const int percents = 100;

        static TechObjectManager techObjectManager = TechObjectManager
            .GetInstance();
        static Device.DeviceManager deviceManager = Device.DeviceManager
            .GetInstance();
        static IO.IOManager ioManager = IO.IOManager.GetInstance();
    }
}
