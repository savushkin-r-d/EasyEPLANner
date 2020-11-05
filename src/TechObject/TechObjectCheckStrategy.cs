using System.Linq;

/// <summary>
/// Стратегии проверки полей объектов
/// </summary>
namespace TechObject.CheckStrategy
{
    /// <summary>
    /// Интерфейс стратегии проверки полей объектов на совпадение
    /// </summary>
    interface IFieldEqualityStrategy
    {
        /// <summary>
        /// Найти совпадающие номера объектов
        /// </summary>
        /// <param name="comparableObject">Сравниваемый объект</param>
        /// <param name="techObjectManager">Менеджер объектов</param>
        /// <returns></returns>
        int[] FindEqual(TechObject comparableObject,
            ITechObjectManager techObjectManager);

        /// <summary>
        /// Сравниваемое поле
        /// </summary>
        string FieldName { get; }
    }

    /// <summary>
    /// Стратегия проверки поля ОУ в объектах.
    /// </summary>
    class EplanNameFieldEqualStrategy : IFieldEqualityStrategy
    {
        public int[] FindEqual(TechObject comparableObject,
            ITechObjectManager techObjectManager)
        {
            int[] matches = techObjectManager.TechObjects
                .Where(x => x.NameEplan == comparableObject.NameEplan &&
                x.TechNumber == comparableObject.TechNumber &&
                x.BaseTechObject?.EplanName ==
                comparableObject.BaseTechObject?.EplanName)
                .Select(x => techObjectManager.GetTechObjectN(x))
                .ToArray();

            return matches;
        }

        public string FieldName
        {
            get
            {
                return "ОУ";
            }
        }
    }

    /// <summary>
    /// Стратегия проверки поля "Имя объекта монитор" в объектах.
    /// </summary>
    class MonitorFieldEqualStrategy : IFieldEqualityStrategy
    {
        public int[] FindEqual(TechObject comparableObject,
            ITechObjectManager techObjectManager)
        {
            int[] matches = techObjectManager.TechObjects
                .Where(x => x.NameBC == comparableObject.NameBC &&
                x.TechNumber == comparableObject.TechNumber)
                .Select(x => techObjectManager.GetTechObjectN(x))
                .ToArray();

            return matches;
        }

        public string FieldName
        {
            get
            {
                return "Имя объекта Monitor";
            }
        }
    }

    /// <summary>
    /// Стратегия проверка поля "Тип" в объектах.
    /// </summary>
    class TypeFieldEqualStrategy : IFieldEqualityStrategy
    {
        public int[] FindEqual(TechObject comparableObject,
            ITechObjectManager techObjectManager)
        {
            int[] matches = techObjectManager.TechObjects
                .Where(x => x.TechType == comparableObject.TechType &&
                x.TechNumber == comparableObject.TechNumber)
                .Select(x => techObjectManager.GetTechObjectN(x))
                .ToArray();

            return matches;
        }

        public string FieldName
        {
            get
            {
                return "Тип";
            }
        }
    }
}