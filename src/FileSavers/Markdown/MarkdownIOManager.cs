using IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner.FileSavers.Markdown
{
    /// <summary>
    /// Markdown конструктор - адаптер <see cref="IIOManager"/>
    /// </summary>
    public interface IMarkdownIOManager
    {
        /// <summary>
        /// Текст списка узлов и модулей
        /// </summary>
        ITextBuilder Cabs();

        /// <summary>
        /// Текст описания узлов и модулей
        /// </summary>
        ITextBuilder Description();
    }


    /// <summary>
    /// Markdown конструктор - адаптер <see cref="IIOManager"/>
    /// </summary>
    public class MarkdownIOManager : IMarkdownIOManager
    {
        /// <summary>
        /// Адаптируемый менеджер узлов и модулей ввода-вывода
        /// </summary>
        private readonly IIOManager ioManager;

        /// <summary>
        /// Название раздела со списком узлов и модулей
        /// </summary>
        public static readonly string TreeOfContentName = "Список узлов и модулей ввода-вывода";

        /// <summary>
        /// Название раздела с описание модулей
        /// </summary>
        public static readonly string DescriptionName = "Описание модулей ввода вывода";

        /// <summary>
        /// Markdown ссылка на раздел со списком узлов и модулей
        /// </summary>
        public static string TreeOfContentLink => TreeOfContentName.ToLower().Split(' ').Aggregate((a, b) => $"{a}-{b}");

        /// <summary>
        /// Markdown-ссылка на раздел с описание модулей
        /// </summary>
        public static string DescriptionLink => DescriptionName.ToLower().Split(' ').Aggregate((a, b) => $"{a}-{b}");


        public MarkdownIOManager(IIOManager ioManager) 
        {
            this.ioManager = ioManager;
        }


        public ITextBuilder Cabs()
        {
            if (ioManager.IONodes.Count == 0)
                return new TextBuilder();

            return new TextBuilder().Lines(
                $"",
                $"## {TreeOfContentName}",
                $"",
                from node in ioManager.IONodes
                select new TextBuilder().Lines(
                    $"<details>",
                    $"    <summary>{node.Location} (-{node.Name})</summary>",
                    $"    <table>",
                    $"        <tr> <td colspan=4 align=center> <code><b>-{node.Name}</b></code> {node.TypeStr}{(node.IsCoupler ? " - <u>Каплер</u>" : "")}",
                    $"        <tr> <td colspan=4>",
                    $"            <b>IP</b>: {node.IP} <br>",
                    from module in node.IOModules
                    select new TextBuilder().WithOffset("        ").Lines(
                        $"<tr> <td> <a href=#module_{module.Name}>-{module.Name}</a> <td> {module.Info.Description} <td> {module.Info.TypeName} <td> {module.Info.Number}"),
                    $"    </table>",
                    $"</details>"));
        }


        public ITextBuilder Description()
        {
            if (ioManager.IONodes.Count == 0)
                return new TextBuilder();

            return new TextBuilder().Lines(
                $"",
                $"## {DescriptionName}",
                $"",
                from node in ioManager.IONodes
                select new TextBuilder().Lines(
                    $"---",
                    $"",
                    $"{node.Location}-{node.Name}",
                    $"===",
                    (from module in node.IOModules
                     select new TextBuilder().Lines(
                         "",
                         new MarkdownModule(node, module).Table(),
                         ""))
                     .DefaultIfEmpty(new TextBuilder())
                     .Aggregate((a, b) => a.Lines("---", b))));
        }
    }
}