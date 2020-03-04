using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            string pathToFiles = path + @"docs\statistics\";

            // 1. Проверить существование папок
            // 2. Если папок нет -> создать их

            SaveLOC(pathToFiles);
            SaveTagsCount(pathToFiles);
            SaveUnintsCount(pathToFiles);
            SaveEquipmentModulesCount(pathToFiles);
        }

        private static void SaveLOC(string path)
        {
            const string calculatingLOCFileName = "main.plua";
            // 1. Написать код, который посчитает мне все строки файла
            // 3. Генерация SVG
            // 4. Записать
        }

        private static void SaveTagsCount(string path)
        {
            var tagsCount = XMLReporter.GetTagsCount();

            // 1. Генерация SVG
            // 2. Записать
        }

        private static void SaveUnintsCount(string path)
        {
            var unitsCount = techObjectManager.UnitsCount;

            // 1. Генерация SVG
            // 2. Записать
        }

        private static void SaveEquipmentModulesCount(string path)
        {
            var equipmentModulesCount = techObjectManager.EquipmentModulesCount;

            // 1. Генерация SVG
            // 2. Записать
        }

        static string linesOfCodeMainProgramFileName = "lines_total.svg";
        static string countOfTagsFileName = "tags_total.svg";
        static string countOfUnitsFileName = "units_total.svg";
        static string counstOfEquipmentModulesFileName = "agregates_total.svg";

        static TechObjectManager techObjectManager = TechObjectManager
            .GetInstance();
    }
}
