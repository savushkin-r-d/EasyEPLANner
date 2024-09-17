using Eplan.EplApi.ApplicationFramework;
using EplanDevice;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TechObject;
using static TechObject.BaseTechObjectManager;

namespace EasyEPlanner
{
    public interface IMarkdownReporter
    {

    }


    public class MarkdownReporter : IMarkdownReporter
    {
        public void Save(string path) 
        {
            var directory = Path.Combine(path, @"docs");

            if (Directory.Exists(directory) is false)
                Directory.CreateDirectory(directory);

            var readmePath = Path.Combine(directory, @"ProjectDescription.md");


            using (var readMe = new StreamWriter(readmePath, false, Encoding.UTF8))
            {
                readMe.Write("# Описание проекта\n");
                readMe.Write("\n");
                readMe.Write("  - [Список технологических объектов](#список-технологических-объектов)\n");
                readMe.Write("  - [Список устройств](#список-устройств)\n");
                readMe.Write("  - [Описание устройств](#описание-устройств)\n");
                readMe.Write("  - [Описание технологических объектов](#описание-технологических-объектов)\n");
                readMe.Write("\n");
                readMe.Write("---\n");
                readMe.Write("\n");
                readMe.Write("## Список технологических объектов\n");
                readMe.Write("\n");
                readMe.Write(GetObjectTree().Replace("\t", "    "));
                readMe.Write("\n");
                readMe.Write("## Список устройств\n");
                readMe.Write("\n");
                readMe.Write(GetDevicesTree().Replace("\t", "    "));
                readMe.Write("\n");
                readMe.Write("## Описание технологических объектов\n");
                readMe.Write("\n");
                readMe.Write(GetTechObjectDescription().Replace("\t", "    "));
                readMe.Write("\n");
                readMe.Write("## Описание устройств\n");
                readMe.Write("\n");
                readMe.Write(GetDevicesDescription().Replace("\t", "    "));
            }
        }

        public StringBuilder GetObjectTree()
        {
            var objectTree = new StringBuilder();

            foreach (var baseObjectGroup in from techObject in techObjectManager.TechObjects
                                            group techObject by techObject.BaseTechObject.Name)
            {
                objectTree
                    .Append("<details>\n")
                    .Append($"\t<summary>{baseObjectGroup.Key}</summary>\n")
                    .Append($"\t<ol>\n");

                foreach (var techObject in baseObjectGroup)
                {
                    objectTree.Append($"\t\t<a href=#tech_object_{techObject.GlobalNum}> <li> {techObject.Name} № {techObject.TechNumber} </li> </a>\n");
                }

                objectTree
                    .Append("\t</ol>\n")
                    .Append("</details>\n");
            }

            return objectTree;
        }


        public StringBuilder GetTechObjectDescription()
        {
            var rslt = new StringBuilder();

            foreach (var techObject in from baseObject in 
                                           from techObject in techObjectManager.TechObjects
                                           group techObject by techObject.BaseTechObject.Name
                                       from techObject in baseObject
                                       select techObject)
            {
                rslt.Append($"<table id=tech_object_{techObject.GlobalNum}>\n")
                    .Append($"\t<tr> <td align=center> <b><code>{techObject.Name} № {techObject.TechNumber}</code> (#{techObject.GlobalNum})</b>\n")
                    .Append($"\t<tr> <td>\n")
                    .Append($"\t\t<b>Базовый объект:</b> {techObject.BaseTechObject.Name} <br>\n")
                    .Append($"\t\t<b>Номер:</b> {techObject.TechNumber} <br>\n")
                    .Append($"\t\t<b>Тип:</b> {techObject.TechType} <br>\n")
                    .Append($"\t\t<b>ОУ:</b> {techObject.NameEplan} <br>\n")
                    .Append($"\t\t<b>Имя в monitor:</b> {techObject.NameBC} <br>\n");

                rslt.Append(GetOperations(techObject, "\t"));

                rslt.Append("</table>\n");
                rslt.Append("\n");
                rslt.Append("---\n");
                rslt.Append("\n");
            }

            return rslt;
        }

        private StringBuilder GetOperations(TechObject.TechObject techObject, string offset)
        {
            var rslt = new StringBuilder();
            var operations = techObject.ModesManager.Modes;

            if (operations.Count == 0)
                return rslt;


            rslt.Append($"{offset}<tr> <td>\n")
                .Append($"{offset}<details> <summary> <b>Операции:</b> </summary>\n")
                .Append($"{offset}<table>\n");

            foreach (var operation in operations)
            {
                rslt.Append($"{offset}\t<tr> <td> {operations.IndexOf(operation) + 1} <td> {operation.Name} <td> {operation.BaseOperation?.Name ?? ""}\n")
                    .Append(GetStates(operation, offset + "\t"));
            }

            rslt.Append($"{offset}</table>\n");

            return rslt;
        }

        private bool ActionIsEmpty(IAction action)
        {
            if (action is GroupableAction)
            {
                return action.SubActions.TrueForAll(sa => sa.Empty || sa.SubActions?.TrueForAll(ssa => ssa.Empty) is true);
            }
            else return action.Empty;
        }

        private StringBuilder GetStates(Mode operation, string offset)
        {
            var rslt = new StringBuilder();


            foreach (var state in from state in operation.States 
                                  where state.Steps.Count > 0 && !state.Steps.TrueForAll(s => s.actions.TrueForAll(a => ActionIsEmpty(a)))
                                  select state)
            {
                rslt.Append($"{offset}<tr> <td> <td colspan=2> <details> <summary>{state.Name}</summary>\n")
                    .Append($"{offset}\t<ul>\n")
                    .Append(GetSteps(state, offset + "\t\t"))
                    .Append($"{offset}\t</ul>\n");
            }

            return rslt;
        }


        private StringBuilder GetSteps(State state, string offset)
        {
            var rslt = new StringBuilder();

            foreach (var step in state.Steps)
            {
                rslt.Append($"{offset}<details> <summary> {step.GetStepNumber()}. {step.GetStepName()}</summary>\n")
                    .Append($"{offset}\t<table>\n")
                    .Append(GetActions(step, offset + "\t\t"))
                    .Append($"{offset}\t</table>\n")
                    .Append($"{offset}</details>\n");


            }

            return rslt;
        }

        private StringBuilder GetActions(Step step, string offset)
        {
            var rslt = new StringBuilder();

            foreach (var action in from action in step.actions
                                   where !action.Empty
                                   select action)
            {
                rslt.Append(ProcessAction(action, offset));
            }

            return rslt;
        }

        private StringBuilder ProcessAction(IAction action, string offset, int level = 1)
        {
            var rslt = new StringBuilder();

            if (action is GroupableAction groupableAction)
            {
                rslt.Append(ProcessGroupableAction(groupableAction, offset, level));
            }
            else
            {
                rslt.AppendFormat(level == 1 ?
                        $"{offset}<tr> <td colspan=3> <b><code>{{0}}</code></b> <td> {{1}}\n" :
                        $"{offset}<tr> <td> <td align=right> - <td align=right> {{0}} <td> {{1}}\n",
                    action.Name, string.Join(", ", from device in action.DevicesNames
                                                   select $"<a href=#device_{device}>{device}</a>"));
            }


            return rslt;
        }

        private StringBuilder ProcessGroupableAction(GroupableAction action, string offset, int level)
        {
            var rslt = new StringBuilder();

            if (ActionIsEmpty(action))
                return rslt;

            rslt.AppendFormat(level == 1 ? 
                    $"{offset}<tr> <td colspan=4> <b><code>{{0}}</code></b>\n" :
                    $"{offset}<tr> <td> - <td> {{0}} <td colspan=2>\n",
                action.Name);

            foreach (var subAction in action.SubActions)
            {
                rslt.Append(ProcessAction(subAction, offset, level + 1));
            }

            foreach (var parameter in action.Parameters ?? Enumerable.Empty<BaseParameter>())
            {
                rslt.AppendFormat(level == 1 ?
                        $"{offset}<tr> <td> - <td colspan=2> {{0}} <td> {{1}}\n" :
                        $"{offset}<tr> <td> <td align=right> - <td align=right> {{0}} <td> {{1}}\n",
                    parameter.Name, parameter.DisplayText[1]);
            }

            return rslt;
        }


        private StringBuilder GetDevicesTree()
        {
            var rslt = new StringBuilder();

            foreach (var ObjectGroup in from device in deviceManager.Devices
                                        orderby device.ObjectName, device.ObjectNumber
                                        group device by device.ObjectName + (device.ObjectNumber == 0 ? "-" : $"{device.ObjectNumber}"))
            {
                rslt.Append("<details>\n")
                    .Append($"\t<summary>{ObjectGroup.Key}</summary>\n")
                    .Append($"\t<ul>\n");

                foreach (var TypeGroup in from device in ObjectGroup
                                          group device by device.DeviceType)
                {
                    rslt.Append("\t\t<details>\n")
                        .Append($"\t\t\t<summary>{TypeGroup.Key}</summary>\n")
                        .Append($"\t\t\t<ul>\n");
                    foreach(var device in from device in TypeGroup
                                          orderby device.ObjectName, device.ObjectNumber, device.DeviceNumber
                                          select device)
                    {
                        rslt.Append($"\t\t\t\t<li> <a href=#device_{device.Name}>{device.Name}</a>\n");
                    }
                    rslt.Append("\t\t\t</ul>\n")
                        .Append("\t\t</details>\n");
                }

                rslt.Append("\t</ul>\n")
                    .Append("</details>\n");
            }


            return rslt;
        }

        private StringBuilder GetDevicesDescription()
        {
            var rslt = new StringBuilder();

            foreach (var device in from device in deviceManager.Devices
                                   orderby device.ObjectName, device.ObjectNumber, device.DeviceType, device.DeviceNumber
                                   select device)
            {
                rslt.Append($"<table id=device_{device.Name}>\n")
                    .Append($"\t<tr> <td align=center> <b><code>{device.Name}</code></b>\n")
                    .Append($"\t<tr> <td>\n")
                    .Append($"\t\t<b>Подтип:</b> {device.DeviceSubType} <br>\n")
                    .Append($"\t\t<b>Описание:</b> {device.Description} <br>\n")
                    .Append($"\t\t<b>Изделие:</b> {device.ArticleName} <br>\n")
                    .Append(GetDeviceChannels(device, "\t"))
                    .Append(GetDeviceParameters(device, "\t"))
                    .Append(GetDeviceProperties(device, "\t"))
                    .Append(GetDeviceRuntimeParameters(device, "\t"))
                    .Append("</table>\n")
                    .Append("\n");
            }

            return rslt;
        }

        private StringBuilder GetDeviceChannels(IODevice device, string offset)
        {
            var rslt = new StringBuilder();

            if (device.Channels.Count == 0)
                return rslt;

           rslt.Append($"{offset}<tr> <td>\n")
               .Append($"{offset}<ul>\n");

            foreach (var channel in device.Channels)
            {
                rslt.Append($"{offset}\t<li> <code>{channel.Name}</code> {channel.Comment} (-A{channel.FullModule}:{channel.ModuleOffset})\n");
            }

            rslt.Append($"{offset}</ul>\n");


            return rslt;
        }

        private StringBuilder GetDeviceParameters(IODevice device, string offset)
        {
            var rslt = new StringBuilder();

            if (device.Parameters.Count == 0)
                return rslt;

            rslt.Append($"{offset}<tr> <td>\n")
                .Append($"{offset}<table>\n");

            foreach (var parameter in device.Parameters)
            {
                rslt.Append($"{offset}<tr> <td> {parameter.Key.Name} <td> {parameter.Key.Description} <td> {string.Format(parameter.Key.Format, parameter.Value)}\n");
            }

            rslt.Append($"{offset}</table>\n");

            return rslt;
        }

        private StringBuilder GetDeviceRuntimeParameters(IODevice device, string offset)
        {
            var rslt = new StringBuilder();

            if (device.RuntimeParameters.Count == 0)
                return rslt;

            rslt.Append($"{offset}<tr> <td>\n")
                .Append($"{offset}<table>\n");

            foreach (var parameter in device.RuntimeParameters)
            {
                rslt.Append($"{offset}<tr> <td> {parameter.Key} <td> {parameter.Value}\n");
            }

            rslt.Append($"{offset}</table>\n");

            return rslt;
        }

        private StringBuilder GetDeviceProperties(IODevice device, string offset)
        {
            var rslt = new StringBuilder();

            if (device.Properties.Count == 0)
                return rslt;

            rslt.Append($"{offset}<tr> <td>\n")
                .Append($"{offset}<table>\n");

            foreach (var property in device.Properties)
            {
                rslt.Append($"{offset}<tr> <td> {property.Key} <td> {property.Value}\n");
            }

            rslt.Append($"{offset}</table>\n");

            return rslt;
        }

        private readonly ITechObjectManager techObjectManager = TechObjectManager.GetInstance();
        private readonly IDeviceManager deviceManager = DeviceManager.GetInstance();
    }
}
