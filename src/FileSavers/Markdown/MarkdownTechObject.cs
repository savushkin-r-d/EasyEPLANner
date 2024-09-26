using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechObject;

namespace EasyEPlanner.FileSavers.Markdown
{
    /// <summary>
    /// Markdown конструктор - адаптер <see cref="TechObject.TechObject"/>
    /// </summary>
    public interface IMarkdownTechObject
    {
        /// <summary>
        /// HTML-таблица с описанием технологического объекта
        /// </summary>
        ITextBuilder Table();

        /// <summary>
        /// HTML-таблица с описанием операций
        /// </summary>
        ITextBuilder Operations();

        /// <summary>
        /// HTML-таблица с описанием операции и ее состояний
        /// </summary>
        /// <param name="mode"></param>
        ITextBuilder Operation(Mode mode);

        /// <summary>
        /// HTML-таблица с описанием шагов
        /// </summary>
        /// <param name="state"></param>
        ITextBuilder Steps(State state);

        /// <summary>
        /// HTML-таблица описания действий
        /// </summary>
        /// <param name="step"></param>
        ITextBuilder Actions(Step step);

        /// <summary>
        /// Обработка описания действия
        /// </summary>
        /// <param name="action">Действие</param>
        /// <param name="level">Уровень действия(поддействия)</param>
        ITextBuilder ProcessAction(IAction action, int level = 1);

        /// <summary>
        /// Обработка группового действия (с поддействиями)
        /// </summary>
        /// <param name="action">Действие</param>
        /// <param name="level">Уровень действия(поддействия)</param>
        ITextBuilder ProcessGroupableAction(GroupableAction action, int level);

        /// <summary>
        /// HTML-ссылка на технологический объект с его названием
        /// </summary>
        string Link { get; }
    }


    /// <summary>
    /// Markdown конструктор - адаптер <see cref="TechObject.TechObject"/>
    /// </summary>
    public class MarkdownTechObject : IMarkdownTechObject
    {
        /// <summary>
        /// Адаптируемый технологический объект
        /// </summary>
        private readonly TechObject.TechObject techObject;

        public MarkdownTechObject(TechObject.TechObject techObject) 
        {
            this.techObject = techObject;
        }

        public string Link => $"<a href=#tech_object_{techObject.GlobalNum}> <li> {techObject.Name} № {techObject.TechNumber} </li> </a>";

        public ITextBuilder Table()
        {
            return new TextBuilder().Lines(
                $"<table id=tech_object_{techObject.GlobalNum}>",
                $"    <tr> <td align=center> <b><code>{techObject.Name} № {techObject.TechNumber}</code> (#{techObject.GlobalNum})</b>",
                $"    <tr> <td>",
                $"        <b>Базовый объект:</b> {techObject.BaseTechObject.Name} <br>",
                $"        <b>Номер:</b> {techObject.TechNumber} <br>",
                $"        <b>Тип:</b> {techObject.TechType} <br>",
                $"        <b>ОУ:</b> {techObject.NameEplan} <br>",
                $"        <b>Имя в monitor:</b> {techObject.NameBC} <br>",
                Operations().WithOffset("    "),
                $"</table>");
        }

        public ITextBuilder Operations()
        {
            if (techObject.ModesManager.Modes.Count == 0)
                return new TextBuilder();

            return new TextBuilder().Lines(
                $"<tr> <td>",
                $"<details> <summary> <b>Операции:</b> </summary>",
                $"<table>",
                from mode in techObject.ModesManager.Modes
                select Operation(mode).WithOffset("    "),
                $"</table>");
        }

        private bool ActionIsEmpty(IAction action)
        {
            if (action is GroupableAction)
            {
                return action.SubActions.TrueForAll(sa => sa.Empty || sa.SubActions?.TrueForAll(ssa => ssa.Empty) is true);
            }
            else return action.Empty;
        }

        public ITextBuilder Operation(Mode mode)
        {
            return new TextBuilder().Lines(
                $"<tr> <td> {mode.Owner.Modes.IndexOf(mode)} <td> {mode.Name} <td> {mode.BaseOperation?.Name ?? ""}",
                from state in mode.States
                where state.Steps.Count > 0 && !state.Steps.TrueForAll(s => s.actions.TrueForAll(a => ActionIsEmpty(a)))
                select new TextBuilder().Lines(
                    $"<tr> <td> <td colspan=2> <details> <summary>{state.Name}</summary>",
                    $"    <ul>",
                    Steps(state).WithOffset("        "),
                    $"    </ul>")
                );
        }

        public ITextBuilder Steps(State state)
        {
            return new TextBuilder().Lines(
                from step in state.Steps
                select new TextBuilder().Lines(
                    $"<details> <summary> {step.GetStepNumber()}. {step.GetStepName()} </summary>",
                    $"    <table>",
                    Actions(step).WithOffset("        "),
                    $"    </table>",
                    $"</details>"));
        }

        public ITextBuilder Actions(Step step)
        {
            return new TextBuilder().Lines(
                from action in step.actions
                where !action.Empty
                select ProcessAction(action));
        }

        public ITextBuilder ProcessAction(IAction action, int level = 1)
        {
            if (action is GroupableAction groupableAction)
            {
                return ProcessGroupableAction(groupableAction, level);
            }
            else
            {
                return new TextBuilder().Lines(
                    string.Format(level == 1 ?
                        $"<tr> <td colspan=3> <code>{{0}}</code> <td> {{1}}" :
                        $"<tr> <td> <td align=right> - <td align=right> {{0}} <td> {{1}}",
                        action.Name, string.Join(", ", from device in action.DevicesNames
                                                       select $"<a href=#device_{device}>{device}</a>")));
            }
        }

        public ITextBuilder ProcessGroupableAction(GroupableAction action, int level)
        {
            if (ActionIsEmpty(action))
                return new TextBuilder();

            return new TextBuilder().Lines(
                string.Format( level == 1 ?
                    $"<tr> <td colspan=4> <code>{{0}}</code>" :
                    $"<tr> <td> - <td> {{0}} <td colspan=2>",
                    action.Name),
                from subAction in action.SubActions
                select ProcessAction(subAction, level + 1),
                from parameter in action.Parameters ?? Enumerable.Empty<BaseParameter>()
                select string.Format(level == 1 ?
                    $"<tr> <td> - <td colspan=2> {{0}} <td> {{1}}" :
                    $"<tr> <td> <td align=right> - <td align=right> {{0}} <td> {{1}}",
                    parameter.Name, parameter.DisplayText[1]));
        }
    }
}
