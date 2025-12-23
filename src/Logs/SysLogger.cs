using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyEPlanner
{
    /// <summary>
    /// Класс для формирования программных логов в файл
    /// </summary>
    public static class SysLogger
    {
        private static readonly BlockingCollection<string> blockingCollection;
        private static readonly string sysLogsFileName = "systemLog.txt";
        private static readonly Task task;

        static SysLogger()
        {
            blockingCollection = new BlockingCollection<string>();

            task = Task.Factory.StartNew(() =>
            {
                var sysLogsFilePath = Path.Combine(ProjectManager.GetInstance().OriginalAssemblyPath, sysLogsFileName);
                using (var streamWriter = new StreamWriter(sysLogsFilePath, false, Encoding.UTF8))
                {
                    streamWriter.AutoFlush = true;

                    foreach (var s in blockingCollection.GetConsumingEnumerable())
                        streamWriter.WriteLine(s);
                }
            },
            TaskCreationOptions.LongRunning);
        }

        public static void WriteLog(object target, params string[] messages)
        {
            blockingCollection.Add($"{DateTime.Now:dd.MM.yyyy HH:mm:ss.fff}: {target}\n" +
                $"{messages.Aggregate("", (res, msg) => res += $"\t{msg}\n")} ");
        }

        public static void Flush()
        {
            blockingCollection.CompleteAdding();
            task.Wait();
        }
    }
}
