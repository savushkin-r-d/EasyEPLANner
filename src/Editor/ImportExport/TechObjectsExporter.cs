using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TechObject;

namespace Editor
{
    /// <summary>
    /// Класс, экспортирующий описание объектов проекта.
    /// </summary>
    public class TechObjectsExporter
    {
        private TechObjectsExporter() 
        {
            techObjectManager = TechObjectManager.GetInstance();
        }

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
        /// Глобальный список объектов.
        /// </summary>
        public List<ITreeViewItem> Objects
        {
            get
            {
                var items = new List<ITreeViewItem>();
                foreach(var item in techObjectManager.TechObjects)
                {
                    items.Add(item);
                }

                return items;
            }
        }

        /// <summary>
        /// Список объектов корня дерева объектов в редакторе.
        /// </summary>
        public ITreeViewItem[] RootItems
        {
            get
            {
                return (techObjectManager as ITreeViewItem).Items;
            }
        }

        /// <summary>
        /// Название проекта
        /// </summary>
        public string ProjectName
        {
            get
            {
                return (techObjectManager as ITreeViewItem).DisplayText[0];
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
            List<TechObject.TechObject> objects = techObjectManager.TechObjects;
            foreach (var obj in objects)
            {
                int globalNum = techObjectManager.GetTechObjectN(obj);
                bool needExporting = objectsNums.Contains(globalNum);
                if (needExporting)
                {
                    TechObject.TechObject exportingObject = obj.Clone(
                        techObjectManager.GetTechObjectN, obj.TechNumber,
                        globalNum, globalNum);

                    // Убираем привязку при экспорте.
                    exportingObject.AttachedObjects.SetValue("");

                    // Обходим нулевое значение т.к объект ни находится ни в
                    // каком списке объектов.
                    string description = exportingObject
                        .SaveAsLuaTable("\t\t", globalNum);
                    description = description
                        .Replace("[ 0 ]", $"[ {globalNum} ]");
                    string restriction = exportingObject
                        .SaveRestrictionAsLua("\t", globalNum);
                    restriction = restriction
                        .Replace("[ 0 ]", $"[ {globalNum} ]");

                    objectsDescription += description;
                    objectsRestriction += restriction;
                }
                else
                {
                    objectsDescription += $"\t[ {globalNum} ] = {{0}},\n";
                }
            }

            try
            {
                var fileWriter = new StreamWriter(path, false, Encoding.UTF8);
                
                WriteObjectsDescription(fileWriter, objectsDescription);
                fileWriter.WriteLine("\n");
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
            string filePattern = EasyEPlanner.Properties.Resources
                .ResourceManager.GetString("mainObjectsPattern");
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
            string filePattern = EasyEPlanner.Properties.Resources
                .ResourceManager.GetString("mainRestrictionsPattern");
            string restrictionsFileData = string.Format(filePattern, "not used", 
                restrictions);
            fileWriter.Write(restrictionsFileData);
        }

        private static TechObjectsExporter techObjectsExporter;
        private ITechObjectManager techObjectManager;
    }
}
