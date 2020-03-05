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
        }

        /// <summary>
        /// Сохранить количество строк кода main.plua в SVG.
        /// </summary>
        /// <param name="folderPath">Путь к каталогу</param>
        /// <param name="locFilePath">Пусть к файлу, для которого надо 
        /// сохранить LOC</param>
        private static void SaveLOC(string folderPath, string locFilePath)
        {
            folderPath += linesOfCodeMainProgramFileName;
            locFilePath += @"\main.plua";

            string[] readedFile = File.ReadAllLines(locFilePath, 
                Encoding.GetEncoding(1251));
            int loc = readedFile.Length;
            string strForWriting = $"{loc.ToString()} строк кода";
            string result = string.Format(svgFilePattern, 100, 90, strForWriting);

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
            folderPath += countOfTagsFileName;
            int tagsCount = XMLReporter.GetTagsCount();
            string strForWriting = $"{tagsCount.ToString()} тэг(ов)";
            string result = string.Format(svgFilePattern, 100, 90, strForWriting);

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
            folderPath += countOfUnitsFileName;
            int unitsCount = techObjectManager.UnitsCount;
            string strForWriting = $"{unitsCount.ToString()} аппарат(ов)";
            string result = string.Format(svgFilePattern, 100, 90, strForWriting);

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
            folderPath += counstOfEquipmentModulesFileName;
            int equipmentModulesCount = techObjectManager.EquipmentModulesCount;
            string strForWriting = $"{equipmentModulesCount.ToString()} агрегат(ов)";
            string result = string.Format(svgFilePattern, 100, 90, strForWriting);

            var equipmentWriter = new StreamWriter(folderPath, false, 
                Encoding.UTF8);
            equipmentWriter.WriteLine(result);
            equipmentWriter.Flush();
            equipmentWriter.Close();
        }

        static string svgFilePattern;

        static string linesOfCodeMainProgramFileName = "lines_total.svg";
        static string countOfTagsFileName = "tags_total.svg";
        static string countOfUnitsFileName = "units_total.svg";
        static string counstOfEquipmentModulesFileName = "agregates_total.svg";

        static TechObjectManager techObjectManager = TechObjectManager
            .GetInstance();
    }
}
