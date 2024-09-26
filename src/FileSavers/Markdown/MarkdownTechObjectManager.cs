using EplanDevice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechObject;

namespace EasyEPlanner.FileSavers.Markdown
{
    /// <summary>
    /// Markdown конструктор - адаптер <see cref="ITechObjectManager"/>
    /// </summary>
    public interface IMarkdownTechObjectManager
    {
        /// <summary>
        /// Текст списка технологических объектов
        /// </summary>
        ITextBuilder TreeOfContent();

        /// <summary>
        /// Текст описания технологических объектов
        /// </summary>
        ITextBuilder Description();
    }

    /// <summary>
    /// Markdown конструктор - адаптер <see cref="ITechObjectManager"/>
    /// </summary>
    public class MarkdownTechObjectManager : IMarkdownTechObjectManager
    {
        /// <summary>
        /// Адаптируемый менеджер технологических объектов
        /// </summary>
        private readonly ITechObjectManager techObjectManager;

        /// <summary>
        /// Название раздела со списком технологических объектов
        /// </summary>
        public static readonly string TreeOfContentName = "Список технологических объектов";

        /// <summary>
        /// Название раздела с описанием технологических объектов
        /// </summary>
        public static readonly string DescriptionName = "Описание технологических объектов";

        /// <summary>
        /// Markdown-ссылка на раздел со списком технологических объектов
        /// </summary>
        public static string TreeOfContentLink => TreeOfContentName.ToLower().Split(' ').Aggregate((a, b) => $"{a}-{b}");

        /// <summary>
        /// Markdown-ссылка на раздел с описанием технологических объектов
        /// </summary>
        public static string DescriptionLink => DescriptionName.ToLower().Split(' ').Aggregate((a, b) => $"{a}-{b}");

        /// <summary>
        /// Сгруппированные технологические объекты
        /// </summary>
        private List<(string BaseObject, List<IMarkdownTechObject> TechObjects)> groupedTechObjects;

        public MarkdownTechObjectManager(ITechObjectManager techObjectManager) 
        {
            this.techObjectManager = techObjectManager;

            groupedTechObjects = (
                from techObject in techObjectManager.TechObjects
                group techObject by techObject.BaseTechObject.Name into baseObjectGroup
                select (
                    BaseObject: baseObjectGroup.Key,
                    TechObjects: (
                        from techObject in baseObjectGroup
                        select new MarkdownTechObject(techObject) as IMarkdownTechObject)
                        .ToList()))
                .ToList();
        }

        public ITextBuilder TreeOfContent()
        {
            if (techObjectManager.TechObjects.Count == 0)
                return new TextBuilder();

            return new TextBuilder().Lines(
                $"",
                $"## {TreeOfContentName}",
                $"",
                from baseObjectGroup in groupedTechObjects
                select new TextBuilder().Lines(
                    $"<details>",
                    $"    <summary>{baseObjectGroup.BaseObject}</summary>",
                    $"    <ol>",
                    new TextBuilder().WithOffset("    ").Lines(
                        from techObject in baseObjectGroup.TechObjects
                        select techObject.Link,
                    $"    </ol>",
                    $"</details>")
                ));
        }

        public ITextBuilder Description()
        {
            if (techObjectManager.TechObjects.Count == 0)
                return new TextBuilder();

            return new TextBuilder().Lines(
                $"",
                $"## {DescriptionName}",
                $"",
                (from baseObjectGroup in groupedTechObjects
                 from techObject in baseObjectGroup.TechObjects
                 select new TextBuilder().Lines(
                     "",
                     techObject.Table(),
                     ""))
                 .Aggregate((a, b) => a.Lines("---", b)));
        }
    }
}
