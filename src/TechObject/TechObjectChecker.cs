using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Класс проверки на корректность объектов в дереве объектов.
    /// </summary>
    public class TechObjectChecker
    {
        public TechObjectChecker(ITechObjectManager techObjectManager)
        {
            this.techObjectManager = techObjectManager;
        }

        /// <summary>
        /// Проверка технологического объекта
        /// на правильность ввода и др.
        /// </summary>
        /// <returns>Строка с ошибками</returns>
        public string Check()
        {
            var errors = string.Empty;

            errors += CheckTypeField();
            errors += CheckObjectMonitorField();

            foreach (var obj in techObjectManager.TechObjects)
            {
                errors += obj.Check();
            }

            return errors;

        }

        /// <summary>
        /// Проверить поле тип у объекта.
        /// </summary>
        private string CheckTypeField()
        {
            var errorsList = new List<string>();
            foreach (var obj in techObjectManager.TechObjects)
            {
                var matches = techObjectManager.TechObjects
                    .Where(x => x.TechType == obj.TechType &&
                    x.TechNumber == obj.TechNumber)
                    .Select(x => techObjectManager.GetTechObjectN(x))
                    .ToArray();

                if (matches.Count() > 1)
                {
                    errorsList.Add($"У объектов {string.Join(",", matches)} " +
                        $"совпадает поле \"Тип\"\n");
                }
            }

            errorsList = errorsList.Distinct().ToList();
            return string.Join("", errorsList);
        }

        /// <summary>
        /// Проверить поле имени объекта Monitor у объекта.
        /// </summary>
        private string CheckObjectMonitorField()
        {
            var errorsList = new List<string>();
            foreach (var obj in techObjectManager.TechObjects)
            {
                var matches = techObjectManager.TechObjects
                    .Where(x => x.NameBC == obj.NameBC &&
                    x.TechNumber == obj.TechNumber)
                    .Select(x => techObjectManager.GetTechObjectN(x))
                    .ToArray();

                if (matches.Count() > 1)
                {
                    errorsList.Add($"У объектов {string.Join(",", matches)} " +
                        $"совпадает поле \"Имя объекта Monitor\"\n");
                }
            }

            errorsList = errorsList.Distinct().ToList();
            return string.Join("", errorsList);
        }

        private ITechObjectManager techObjectManager;
    }
}
