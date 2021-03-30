using System.IO;
using TechObject;
using System.Linq;

namespace EasyEPlanner
{
    /// <summary>
    /// Класс, сохраняющий статистику в картинках SVG
    /// </summary>
    public class SVGStatisticsSaver
    {
        public void Save(string path)
        {
            string pathToFiles = path + @"\docs\statistics\";
            svgFilePattern = Properties.Resources.ResourceManager
            .GetString("svgPattern");
            if (Directory.Exists(pathToFiles) == false)
            {
                Directory.CreateDirectory(pathToFiles);
            }

            SaveLOC(pathToFiles, path);
            SaveTagsCount(pathToFiles);
            SaveUnintsCount(pathToFiles);
            SaveEquipmentModulesCount(pathToFiles);
            SaveDevicesCount(pathToFiles);
            SaveIOLinkModulesPercentage(pathToFiles);
            SaveCouplersCount(pathToFiles);
            SaveIOModulesCount(pathToFiles);
        }

        /// <summary>
        /// Сохранить количество строк кода main.plua в SVG.
        /// </summary>
        /// <param name="folderPath">Путь к каталогу</param>
        /// <param name="locFilePath">Пусть к файлу, для которого надо 
        /// сохранить LOC</param>
        private void SaveLOC(string folderPath, string locFilePath)
        {
            const int maxLOCCount = 1000;

            folderPath += LinesOfCodeMainProgramFileName;
            locFilePath += @"\" + ProjectDescriptionSaver.MainProgramFileName;

            int loc = 0;
            if(File.Exists(locFilePath))
            {
                string[] readedFile = File.ReadAllLines(locFilePath,
                    EncodingDetector.DetectFileEncoding(locFilePath));
                loc = readedFile.Length;
            }

            string labelName = GenerateNounAfterNumber("строк", loc, "a", "и",
                string.Empty);
            string displayingText = $"{loc} {labelName} кода";
            string result = MakeStringForWriting(loc, maxLOCCount,
                displayingText);
            WriteFile(result, folderPath);
        }

        /// <summary>
        /// Сохранить количество тэгов в SVG.
        /// </summary>
        /// <param name="folderPath">Путь к каталогу</param>
        private void SaveTagsCount(string folderPath)
        {
            const int maxTagsCount = 5000;

            folderPath += CountOfTagsFileName;
            int tagsCount = XMLReporter.GetTagsCount();
            string labelName = GenerateNounAfterNumber("тэг", tagsCount,
                string.Empty, "а", "ов");
            string displayingText = $"{tagsCount} {labelName}";
            string result = MakeStringForWriting(tagsCount, maxTagsCount,
                displayingText);
            WriteFile(result, folderPath);
        }

        /// <summary>
        /// Сохранить количество аппаратов в SVG.
        /// </summary>
        /// <param name="folderPath">Путь к каталогу</param>
        private void SaveUnintsCount(string folderPath)
        {
            const int maxUnitsCount = 10;

            folderPath += CountOfUnitsFileName;
            int unitsCount = techObjectManager.UnitsCount;
            string labelName = GenerateNounAfterNumber("аппарат", unitsCount,
                string.Empty, "а", "ов");
            string displayingText = $"{unitsCount} {labelName}";
            string result = MakeStringForWriting(unitsCount, maxUnitsCount,
                displayingText);
            WriteFile(result, folderPath);
        }

        /// <summary>
        /// Сохранить количество агрегатов в SVG.
        /// </summary>
        /// <param name="folderPath">Путь к каталогу</param>
        private void SaveEquipmentModulesCount(string folderPath)
        {
            const int maxEquipCount = 50;

            folderPath += CountOfEquipmentModulesFileName;
            int equipCount = techObjectManager.EquipmentModulesCount;
            string labelName = GenerateNounAfterNumber("агрегат", equipCount,
                string.Empty, "а", "ов");
            string displayingText = $"{equipCount} {labelName}";
            string result = MakeStringForWriting(equipCount, maxEquipCount,
                displayingText);
            WriteFile(result, folderPath);
        }

        /// <summary>
        /// Сохранить количество устройств в SVG.
        /// </summary>
        /// <param name="folderPath">Путь к каталогу</param>
        private void SaveDevicesCount(string folderPath)
        {
            const int maxDevicesCount = 1000;

            folderPath += CountOfDevicesFileName;
            int devicesCount = deviceManager.Devices.Count;
            string labelName = GenerateNounAfterNumber("устройств",
                devicesCount, "о", "а", string.Empty);
            string displayingText = $"{devicesCount} {labelName}";
            string result = MakeStringForWriting(devicesCount, maxDevicesCount,
                displayingText);
            WriteFile(result, folderPath);
        }

        /// <summary>
        /// Сохранить количество IO-Link модулей в процентном соотношении
        /// к общему количеству в SVG.
        /// </summary>
        /// <param name="folderPath">Путь к каталогу</param>
        private void SaveIOLinkModulesPercentage(string folderPath)
        {
            int modulesCount = 0;
            int ioLinkModules = 0;
            foreach(var node in ioManager.IONodes)
            {
                modulesCount += node.IOModules.Count;
                foreach(var module in node.IOModules)
                {
                    if(module.IsIOLink())
                    {
                        ioLinkModules++;
                    }
                }
            }
            int valueInPercents = ValueAsPercentage(ioLinkModules,
                modulesCount);
            string displayingText = $"{valueInPercents}% IO-Link I/O";
            string result = MakeStringForWriting(ioLinkModules,
                modulesCount, displayingText);
            folderPath += IOLModulesInPercentFileName;
            WriteFile(result, folderPath);
        }

        /// <summary>
        /// Сохранить количество модулей ввода-вывода в SVG.
        /// </summary>
        /// <param name="folderPath">Путь к каталогу</param>
        private void SaveIOModulesCount(string folderPath)
        {
            const int maxCount = 50;
            int modulesCount = ioManager.IONodes.SelectMany(x => x.IOModules)
                .Count();
            string labelname = GenerateNounAfterNumber("I/O модул",
                modulesCount, "ь", "я", "ей");
            string displayingText = $"{modulesCount} {labelname}";
            string result = MakeStringForWriting(modulesCount, maxCount,
                displayingText);
            folderPath += IOModulesCountFileName;
            WriteFile(result, folderPath);
        }

        /// <summary>
        /// Сохранить количество узлов I/O из всех узлов ввода-вывода в SVG.
        /// </summary>
        /// <param name="folderPath">Путь к каталогу</param>
        private void SaveCouplersCount(string folderPath)
        {
            const int maxCount = 10;
            int couplersCount = ioManager.IONodes
                .Where(x => x.IsCoupler).Count();
            string labelName = GenerateNounAfterNumber("уз", couplersCount,
                "ел", "ла", "лов");
            string displayingText = $"{couplersCount} {labelName}";
            string result = MakeStringForWriting(couplersCount, maxCount,
                displayingText);
            folderPath += CouplersCountFileName;
            WriteFile(result, folderPath);
        }

        /// <summary>
        /// Генерация существительного стоящего после числительного
        /// </summary>
        /// <param name="noun">Существительное</param>
        /// <param name="number">Числительное</param>
        /// <param name="subjectiveCaseSingularEnding">Окончание
        /// существительного в именительном падеже единственного значения
        /// </param>
        /// <param name="genitiveCaseSingularEnding">Окончание
        /// существительного в родительном падеже единственного значения
        /// </param>
        /// <param name="genitiveCasePluralEnding">Окончание существительного
        /// в родительном падеже множественного значения</param>
        /// <returns></returns>
        private string GenerateNounAfterNumber(string noun, int number,
            string subjectiveCaseSingularEnding,
            string genitiveCaseSingularEnding,
            string genitiveCasePluralEnding)
        {
            const int subjectiveCaseSingularDigit = 1;
            const int genitiveCasePluralDigit = 5;
            const int divider = 10;

            var lastDigit = number % divider;
            if (lastDigit == subjectiveCaseSingularDigit)
            {
                return $"{noun}{subjectiveCaseSingularEnding}";
            }
            else if (lastDigit > subjectiveCaseSingularDigit &&
                lastDigit < genitiveCasePluralDigit)
            {
                return $"{noun}{genitiveCaseSingularEnding}";
            }
            else
            {
                return $"{noun}{genitiveCasePluralEnding}";
            }
        }

        /// <summary>
        /// Сделать текст для записи в файл
        /// </summary>
        /// <param name="itemsCount">Количество элементов</param>
        /// <param name="maxItemsCount">Максимальное количество элементов
        /// </param>
        /// <param name="displayingText">Отображаемый текст</param>
        /// <returns></returns>
        private string MakeStringForWriting(int itemsCount, 
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
        private int ValueAsPercentage(int currentValue, int maxValue)
        {
            if (maxValue == 0)
            {
                return 0;
            }

            int result;
            result = (currentValue * percents) / maxValue;
            return result;
        }

        /// <summary>
        /// Запись файла со статистикой.
        /// </summary>
        /// <param name="text">Текст для записи</param>
        /// <param name="folderPath">Путь для записи</param>
        private void WriteFile(string text, string folderPath)
        {
            var equipmentWriter = new StreamWriter(folderPath, false,
                EncodingDetector.UTF8);
            equipmentWriter.WriteLine(text);
            equipmentWriter.Flush();
            equipmentWriter.Close();
        }

        string svgFilePattern;

        const string LinesOfCodeMainProgramFileName = "lines_total.svg";
        const string CountOfTagsFileName = "tags_total.svg";
        const string CountOfUnitsFileName = "units_total.svg";
        const string CountOfEquipmentModulesFileName = "agregates_total.svg";
        const string CountOfDevicesFileName = "devices_total.svg";
        const string IOLModulesInPercentFileName = "io_link_usage.svg";
        const string IOModulesCountFileName = "io_modules_total.svg";
        const string CouplersCountFileName = "io_couplers_total.svg";

        /// <summary>
        /// 100% длина линии SVG. 
        /// </summary>
        const int percents = 100;

        ITechObjectManager techObjectManager = TechObjectManager
            .GetInstance();
        Device.IDeviceManager deviceManager = Device.DeviceManager
            .GetInstance();
        IO.IIOManager ioManager = IO.IOManager.GetInstance();
    }
}
