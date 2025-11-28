using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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
        private readonly ITechObjectManager techObjectManager;

        /// <summary>
        /// Адаптируемый технологический объект
        /// </summary>
        private readonly TechObject.TechObject techObject;

        public MarkdownTechObject(ITechObjectManager techObjectManager, TechObject.TechObject techObject) 
        {
            this.techObjectManager = techObjectManager;
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
                Aggregates().WithOffset("    "),
                Operations().WithOffset("    "),
                Parameters().WithOffset("    "),
                Equipment().WithOffset("    "),
                SystemParameters().WithOffset("    "),
                $"</table>");
        }

        public ITextBuilder Aggregates()
        {
            if (!techObject.AttachedObjects.GetValueIndexes().Any())
                return new TextBuilder();


            return new TextBuilder().Lines(
                $"<tr> <td>",
                $"<details> <summary> <b>Привязанные агрегаты:</b> </summary>",
                $"<ul>",
                from aggregateIndex in techObject.AttachedObjects.GetValueIndexes()
                select techObjectManager.GetTObject(aggregateIndex) into aggregate
                select $"    <li> <a href=#tech_object_{aggregate.GlobalNum}>{aggregate.Name} №{aggregate.TechNumber} (#{aggregate.GlobalNum})</a>",
                $"</ul>"
                );
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

        public ITextBuilder Parameters()
        {
            var parametersManager = techObject.GetParamsManager();

            if ((parametersManager.Float is null || parametersManager.Float.Items.Count() == 0) &&
                (parametersManager.FloatRuntime is null || parametersManager.FloatRuntime.Items.Count() == 0))
                return new TextBuilder();

            return new TextBuilder().Lines(
                $"<tr> <td>",
                $"<details> <summary> <b>Параметры:</b> </summary>",
                $"<table>",
                $"    <tr> <th> № <th> Название <th> Значение",
                Params(parametersManager.Float).WithOffset("    "),
                Params(parametersManager.FloatRuntime).WithOffset("    "),
                $"</table>"
                );
        }

        public ITextBuilder SystemParameters()
        {
            if (techObject.SystemParams is null || techObject.SystemParams.Count == 0)
                return new TextBuilder();

            return new TextBuilder().Lines(
                $"<tr> <td>",
                $"<details> <summary> <b>Системные параметры:</b> </summary>",
                $"<table>",
                $"    <tr> <th> № <th> Название <th> Значение",
                from parameter in techObject.SystemParams.Parameters
                select $"    <tr> <td> {techObject.SystemParams.GetIdx(parameter)} <td> <b>{parameter.LuaName}</b> <br> <i>{parameter.Name}</i> <td> {parameter.Value.Value} {parameter.Meter}",
                $"</table>");
        }
        
        public ITextBuilder Equipment()
        {
            if (techObject.Equipment.Items.Count() == 0)
                return new TextBuilder();


            return new TextBuilder().Lines(
                $"<tr> <td>",
                $"<details> <summary> <b>Оборудование:</b> </summary>",
                $"<table>",
                from equip in techObject.Equipment.Items.OfType<BaseParameter>()
                select $"    <tr> <td> {equip.Name} <td> {equip.Value}",
                $"</table>");
        }

        public ITextBuilder BaseProperties()
        {
            if (techObject.BaseProperties.Count == 0)
                return new TextBuilder();

            return new TextBuilder().Lines(
                $"<tr> <td>",
                $"<details> <summary> <b>Доп. свойства:</b> </summary>",
                $"<table>",
                from equip in techObject.BaseProperties. Items.OfType<BaseParameter>()
                select $"    <tr> <td> {equip.Name} <td> {equip.Value}",
                $"</table>");
        }

        public ITextBuilder Params(Params parameters)
        {
            if (parameters is null || parameters.Items.Count() == 0)
                return new TextBuilder();

            return new TextBuilder().Lines(
                $"<tr> <td colspan=3> {parameters.Name}:",
                from parameter in parameters.Items.OfType<Param>()
                select $"<tr> <td> {parameter.GetParameterNumber} <td> <b>{parameter.GetNameLua()}</b> <br> <i>{parameter.GetName()}</i> <td> {parameter.GetValue()} {parameter.GetMeter()}");
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
                $"<tr> <td> {mode.GetModeNumber()} <td> {mode.Name} <td> {mode.BaseOperation?.Name ?? ""}",
                from state in mode.States
                where state.Steps.Count > 0 && !state.Steps.TrueForAll(s => s.actions.TrueForAll(a => ActionIsEmpty(a)))
                select new TextBuilder().Lines(
                    $"<tr> <td> <td colspan=2> <details> <summary>{state.Name}</summary>",
                    $"    <ul>",
                    Steps(state).WithOffset("        "),
                    $"    </ul>"));
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
