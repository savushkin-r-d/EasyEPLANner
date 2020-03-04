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
            string pathToFiles = path + @"\docs\statistics\";

            if (Directory.Exists(pathToFiles) == false)
            {
                Directory.CreateDirectory(pathToFiles);
            }

            SaveLOC(pathToFiles);
            SaveTagsCount(pathToFiles);
            SaveUnintsCount(pathToFiles);
            SaveEquipmentModulesCount(pathToFiles);
        }

        /// <summary>
        /// Сохранить количество строк кода main.plua в SVG.
        /// </summary>
        /// <param name="folderPath"></param>
        private static void SaveLOC(string folderPath)
        {
            folderPath += linesOfCodeMainProgramFileName;
            //const string calculatingLOCFileName = "main.plua";
            // 1. Написать код, который посчитает мне все строки файла
            // 3. Генерация SVG
            // 4. Записать
        }

        /// <summary>
        /// Сохранить количество тэгов в SVG.
        /// </summary>
        /// <param name="folderPath">Пусть сохранения</param>
        private static void SaveTagsCount(string folderPath)
        {
            folderPath += countOfTagsFileName;
            int tagsCount = XMLReporter.GetTagsCount();
            string strForWriting = $"{tagsCount} тэг(ов)";
            string result = svgFilePattern.Replace("place text here",
                strForWriting);
            
            var tagsWriter = new StreamWriter(folderPath, false, Encoding.
                GetEncoding(1251));
            tagsWriter.WriteLine(result);
            tagsWriter.Flush();
            tagsWriter.Close();
        }

        /// <summary>
        /// Сохранить количество аппаратов в SVG.
        /// </summary>
        /// <param name="folderPath">Пусть сохранения</param>
        private static void SaveUnintsCount(string folderPath)
        {
            folderPath += countOfUnitsFileName;
            int unitsCount = techObjectManager.UnitsCount;
            string strForWriting = $"{unitsCount} аппарат(ов)";
            string result = svgFilePattern.Replace("place text here",
                strForWriting);

            var unitsWriter = new StreamWriter(folderPath, false, Encoding.
                GetEncoding(1251));
            unitsWriter.WriteLine(result);
            unitsWriter.Flush();
            unitsWriter.Close();
        }

        /// <summary>
        /// Сохранить количество агрегатов в SVG.
        /// </summary>
        /// <param name="folderPath"></param>
        private static void SaveEquipmentModulesCount(string folderPath)
        {
            folderPath += counstOfEquipmentModulesFileName;
            int equipmentModulesCount = techObjectManager.EquipmentModulesCount;
            string stroForWriting = $"{equipmentModulesCount} агрегат(ов)";
            string result = svgFilePattern.Replace("place text here",
                stroForWriting);

            var equipmentWriter = new StreamWriter(folderPath, false, Encoding.
                GetEncoding(1251));
            equipmentWriter.WriteLine(result);
            equipmentWriter.Flush();
            equipmentWriter.Close();
        }

        static string svgFilePattern = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
            "<svg width=\"100\" height=\"20\" version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\">\n" +
            "\n" +
            "\t<rect rx=\"4\" x=\"0\" width=\"100\" height=\"20\" fill=\"dimgrey\"\n/>" +
            "<rect rx=\"4\" x=\"0\" width=\"90.0\" height=\"20\" fill=\"green\"/>\n" +
            "\n" +
            "\t<g fill=\"#fff\" text-anchor=\"middle\" font-family=\"DejaVu Sans,Verdana,Geneva,sans-serif\" font-size=\"11\">\n" +
            "\t\t<text x=\"50.0\" y=\"14\">\n" +
            "\t\t\tplace text here" +
            "\t\t</text>" +
            "\t</g>" +
            "</svg>";

        static string linesOfCodeMainProgramFileName = "lines_total.svg";
        static string countOfTagsFileName = "tags_total.svg";
        static string countOfUnitsFileName = "units_total.svg";
        static string counstOfEquipmentModulesFileName = "agregates_total.svg";

        static TechObjectManager techObjectManager = TechObjectManager
            .GetInstance();
    }
}
