using EasyEPlanner.FileSavers.Markdown;
using Eplan.EplApi.ApplicationFramework;
using EplanDevice;
using IO;
using StaticHelper;
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
        public static List<string> ListBuild(int tabPrefix = 0, params object[] values)
        {
            var rslt = new List<string>();
            var prefix = new string(' ', tabPrefix * 4);

            foreach (var v in values)
            {
                switch (v)
                {
                    case string line:
                        rslt.Add(prefix + line);
                        break;

                    case IEnumerable<string> enumerable:
                        rslt.AddRange(enumerable.Select(line => prefix + line));
                        break;
                }
            }

            return rslt;
        }


        public void Save(string path) 
        {
            var directory = Path.Combine(path, @"docs");

            if (Directory.Exists(directory) is false)
                Directory.CreateDirectory(directory);

            var readmePath = Path.Combine(directory, @"ProjectDescription.md");


            var mdDevises = new MarkdownDeviceManager(DeviceManager.GetInstance());
            var mdModules = new MarkdownIOManager(IOManager.GetInstance());
            var mdTechObjects = new MarkdownTechObjectManager(TechObjectManager.GetInstance());

            using (var readMe = new StreamWriter(readmePath, false, Encoding.UTF8))
            {
                readMe.Write($"# Описание проекта {EProjectManager.GetInstance().GetCurrentProjectName()}\n");
                readMe.Write("\n");
                readMe.Write($"<b>Диапазон(-ы) IP-адресов</b>:\n");
                readMe.Write(IPRanges());
                readMe.Write("\n");
                readMe.Write("\n");
                readMe.Write($"Содержание:\n");
                readMe.Write($"  - [{MarkdownIOManager.TreeOfContentName}](#{MarkdownIOManager.TreeOfContentLink})\n");
                readMe.Write($"  - [{MarkdownTechObjectManager.TreeOfContentName}](#{MarkdownTechObjectManager.TreeOfContentLink})\n");
                readMe.Write($"  - [{MarkdownDeviceManager.TreeOfContentName}](#{MarkdownDeviceManager.TreeOfContentLink})\n");
                readMe.Write($"  - [{MarkdownTechObjectManager.DescriptionName}](#{MarkdownTechObjectManager.DescriptionLink})\n");
                readMe.Write($"  - [{MarkdownDeviceManager.DescriptionName}](#{MarkdownDeviceManager.DescriptionLink})\n");
                readMe.Write($"  - [{MarkdownIOManager.DescriptionName}](#{MarkdownIOManager.DescriptionLink})\n");
                readMe.Write("\n");
                readMe.Write("---\n");
                readMe.Write(mdModules.Cabs().Result());
                readMe.Write(mdTechObjects.TreeOfContent().Result());
                readMe.Write(mdDevises.TreeOfContent().Result());
                readMe.Write(mdTechObjects.Description().Result());
                readMe.Write("<br><br><br>\n");
                readMe.Write(mdDevises.Description().Result());
                readMe.Write("<br><br><br>\n");
                readMe.Write(mdModules.Description().Result());
            }
        }

        public string IPRanges()
        {
            return string.Join("\n", from ipRange in ProjectConfiguration.GetInstance().RangesIP
                                     select $"  - {IPConverter.ToString(ipRange.Item1)} - {IPConverter.ToString(ipRange.Item2)};");
        }
    }
}
