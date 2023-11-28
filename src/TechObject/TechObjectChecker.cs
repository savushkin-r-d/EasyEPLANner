using System.Collections.Generic;
using System.Linq;
using TechObject.CheckStrategy;

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

            errors += ObjectsFieldEquality(new TypeFieldEqualStrategy());
            errors += ObjectsFieldEquality(new MonitorFieldEqualStrategy());
            errors += ObjectsFieldEquality(new EplanNameFieldEqualStrategy());

            foreach (var obj in techObjectManager.GenericTechObjects)
            {
                errors += obj.Check();
            }

            foreach (var obj in techObjectManager.TechObjects)
            {
                errors += obj.Check();
            }

            return errors;

        }

        /// <summary>
        /// Проверить поля объектов на совпадение согласно стратегии
        /// </summary>
        /// <param name="strategy">Стратегия проверки</param>
        /// <returns></returns>
        private string ObjectsFieldEquality(IFieldEqualityStrategy strategy)
        {
            var errorsList = new List<string>();
            foreach (var obj in techObjectManager.TechObjects)
            {
                int[] matches = strategy.FindEqual(obj, techObjectManager);

                if (matches.Count() > 1)
                {
                    errorsList.Add($"У объектов {string.Join(",", matches)} " +
                        $"совпадает поле \"{strategy.FieldName}\"\n");
                }
            }

            errorsList = errorsList.Distinct().ToList();
            return string.Join("", errorsList);
        }

        private ITechObjectManager techObjectManager;
    }
}
