using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechObject;

namespace EasyEPlanner
{
    /// <summary>
    /// Класс, экспортирующий описание объектов проекта.
    /// </summary>
    public class TechObjectsExporter
    {
        private TechObjectsExporter() { }

        public static TechObjectsExporter GetInstance()
        {
            if (techObjectsExporter == null)
            {
                techObjectsExporter = new TechObjectsExporter();
            }
            return techObjectsExporter;
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
            foreach(var num in objectsNums)
            {
                var techObj = TechObjectManager.GetInstance().GetTObject(num);
                if (techObj == null)
                {
                    continue;
                }
                objectsDescription += techObj.SaveAsLuaTable("\t");
                objectsRestriction += techObj.SaveRestrictionAsLua("\t");
            }

            try
            {
                var fileWriter = new StreamWriter(path, false, Encoding.UTF8);
                WriteObjectsDescription(fileWriter, objectsDescription);
                fileWriter.WriteLine("--");
                WriteObjectsRestriction(fileWriter, objectsRestriction);
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
            string stringForSaving = "objects =\n";
            stringForSaving += "{\n" + description + "}\n";
            fileWriter.Write(stringForSaving);
        }

        /// <summary>
        /// Записать ограничения.
        /// </summary>
        /// <param name="streamWriter">Поток для записи</param>
        /// <param name="restrictions">Ограничения</param>
        private void WriteObjectsRestriction(StreamWriter fileWriter, 
            string restrictions)
        {
            string stringForSaving = "restrictions =\n";
            stringForSaving += "\t{" + restrictions + "\n\t}";
            fileWriter.Write(stringForSaving);
        }

        private static TechObjectsExporter techObjectsExporter;
    }
}
