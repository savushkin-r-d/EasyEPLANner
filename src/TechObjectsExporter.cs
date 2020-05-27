using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TechObject;

namespace EasyEPlanner
{
    /// <summary>
    /// Класс, экспортирующий описание объектов проекта.
    /// </summary>
    public class TechObjectsExporter
    {
        private TechObjectsExporter() { }

        /// <summary>
        /// Singleton
        /// </summary>
        /// <returns></returns>
        public static TechObjectsExporter GetInstance()
        {
            if (techObjectsExporter == null)
            {
                techObjectsExporter = new TechObjectsExporter();
            }
            return techObjectsExporter;
        }

        /// <summary>
        /// Имена доступных для экспорта объектов.
        /// </summary>
        public string[] ExportingObjectsNames
        {
            get
            {
                return TechObjectManager.GetInstance().Items
                .Select(x => x.DisplayText[0])
                .ToArray();
            }
        }

        /// <summary>
        /// Экспорт описания проекта в файл.
        /// </summary>
        /// <param name="path">Место сохранения</param>
        /// <param name="objectsNums">Список (номера) экспортируемых
        /// объектов</param>
        public void Export(string path, List<int> objectsNums)
        {
            string objectsDescription = "";
            string objectsRestriction = "";
            var objects = TechObjectManager.GetInstance().Objects;
            foreach (var obj in objects)
            {
                if (objectsNums.Contains(obj.GlobalNumber))
                {
                    objectsDescription += obj.SaveAsLuaTable("\t\t");
                    objectsRestriction += obj.SaveRestrictionAsLua("\t");
                }
                else
                {
                    objectsDescription += $"\t[ {obj.GlobalNumber} ] = {{0}},\n";
                }
            }

            try
            {
                var fileWriter = new StreamWriter(path, false, Encoding.UTF8);
                
                WriteObjectsDescription(fileWriter, objectsDescription);
                fileWriter.WriteLine();
                WriteObjectsRestriction(fileWriter, objectsRestriction);
                
                fileWriter.Flush();
                fileWriter.Close();
            }
            catch
            {
                throw new Exception("Ошибка записи в файл при экспорте");
            }            
        }

        /// <summary>
        /// Записать описание объектов.
        /// </summary>
        /// <param name="fileWriter">Поток для записи</param>
        /// <param name="description">Описание объектов</param>
        private void WriteObjectsDescription(StreamWriter fileWriter, 
            string description)
        {
            string filePattern = Properties.Resources.ResourceManager
                .GetString("mainObjectsPattern");
            string descriptionFileData = string.Format(filePattern, "not used",
                "not used", description);
            fileWriter.Write(descriptionFileData);
        }

        /// <summary>
        /// Записать ограничения.
        /// </summary>
        /// <param name="fileWriter">Поток для записи</param>
        /// <param name="restrictions">Ограничения</param>
        private void WriteObjectsRestriction(StreamWriter fileWriter, 
            string restrictions)
        {
            string filePattern = Properties.Resources.ResourceManager
                .GetString("mainRestrictionsPattern");
            string restrictionsFileData = string.Format(filePattern, "not used", 
                restrictions);
            fileWriter.Write(restrictionsFileData);
        }

        private static TechObjectsExporter techObjectsExporter;
    }
}
