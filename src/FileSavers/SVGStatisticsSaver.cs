﻿using System.Text;
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
            string strForWriting = $"{loc.ToString()} строк кода";
            int currentValue = ValueAsPercentage(loc, maxLOCCount);
            string result = string.Format(svgFilePattern, percents, 
                currentValue, strForWriting);

            var locWriter = new StreamWriter(folderPath, false, Encoding.UTF8);
            locWriter.WriteLine(result);
            locWriter.Flush();
            locWriter.Close();
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
            string strForWriting = $"{tagsCount.ToString()} тэг(ов)";
            int currentValue = ValueAsPercentage(tagsCount, maxTagsCount);
            string result = string.Format(svgFilePattern, percents, 
                currentValue, strForWriting);

            var tagsWriter = new StreamWriter(folderPath, false, Encoding.UTF8);
            tagsWriter.WriteLine(result);
            tagsWriter.Flush();
            tagsWriter.Close();
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
            string strForWriting = $"{unitsCount.ToString()} аппарат(ов)";
            int currentValue = ValueAsPercentage(unitsCount, maxUnitsCount);
            string result = string.Format(svgFilePattern, percents, 
                currentValue, strForWriting);

            var unitsWriter = new StreamWriter(folderPath, false, 
                Encoding.UTF8);
            unitsWriter.WriteLine(result);
            unitsWriter.Flush();
            unitsWriter.Close();
        }

        /// <summary>
        /// Сохранить количество агрегатов в SVG.
        /// </summary>
        /// <param name="folderPath">Путь к каталогу</param>
        private static void SaveEquipmentModulesCount(string folderPath)
        {
            const int maxEquipCount = 50;

            folderPath += counstOfEquipmentModulesFileName;
            int equipCount = techObjectManager.EquipmentModulesCount;
            string strForWriting = $"{equipCount.ToString()} агрегат(ов)";
            int currentValue = ValueAsPercentage(equipCount, maxEquipCount);
            string result = string.Format(svgFilePattern, percents, 
                currentValue, strForWriting);

            var equipmentWriter = new StreamWriter(folderPath, false, 
                Encoding.UTF8);
            equipmentWriter.WriteLine(result);
            equipmentWriter.Flush();
            equipmentWriter.Close();
        }

        /// <summary>
        /// Расчет процентного соотношения параметров
        /// </summary>
        /// <param name="currentValue">Текущее значение параметры</param>
        /// <param name="maxValue">Максимальное значение параметра</param>
        /// <returns></returns>
        private static int ValueAsPercentage(int currentValue, int maxValue)
        {
            int result = 0;
            const int maxPercent = 100;
            result = (currentValue * maxPercent) / maxValue;
            return result;
        }

        static string svgFilePattern;

        static string linesOfCodeMainProgramFileName = "lines_total.svg";
        static string countOfTagsFileName = "tags_total.svg";
        static string countOfUnitsFileName = "units_total.svg";
        static string counstOfEquipmentModulesFileName = "agregates_total.svg";

        const int percents = 100; // 100% длина линии SVG. 

        static TechObjectManager techObjectManager = TechObjectManager
            .GetInstance();
    }
}
