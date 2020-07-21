using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EasyEPlanner
{
    /// <summary>
    /// Тестер работоспособности проекта
    /// </summary>
    public class ProjectHealthChecker
    {
        /// <summary>
        /// Протестировать текущий проект на наличие ошибок.
        /// </summary>
        public void Test()
        {
            bool validLUA = CheckLua();
            if(validLUA)
            {
                bool validProject = CheckProject();
                if(validProject)
                {
                    string message = "Тестирование проекта прошло успешно. " +
                        "Проект работоспособен.";
                    Logs.AddMessage(message);
                }
                else
                {
                    string message = "Тестирование проекта завершилось с " +
                        "ошибками. Проект не работоспособен.";
                    Logs.AddMessage(message);
                }
            }
            else
            {
                string message = "В системе не найден LUA, тестирование " +
                    "проекта невозможно.";
                Logs.AddMessage(message);
            }
        }

        /// <summary>
        /// Проверка доступности LUA в системе
        /// </summary>
        /// <returns></returns>
        private bool CheckLua()
        {
            bool isValid = false;
            string arguments = "/C lua -v";

            var cmdProcess = new Process();
            var startInfo = new ProcessStartInfo()
            {
                FileName = cmdFileName,
                Arguments = arguments,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            cmdProcess.StartInfo = startInfo;
            cmdProcess.Start();
            cmdProcess.Close();

            if(cmdProcess.ExitCode == 0)
            {
                isValid = true;
            }

            return isValid;

        }

        /// <summary>
        /// Проверка работоспособности проекта
        /// </summary>
        /// <returns></returns>
        private bool CheckProject()
        {
            bool isValid = false;
            string arguments = "";
            string workingDirectory = "";

            var cmdProcess = new Process();
            var startInfo = new ProcessStartInfo()
            {
                FileName = cmdFileName,
                Arguments = arguments,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardOutput = true,
                WorkingDirectory = workingDirectory,
            };
            cmdProcess.StartInfo = startInfo;
            cmdProcess.Start();
            cmdProcess.Close();

            if(cmdProcess.ExitCode == 0)
            {
                isValid = true;
            }

            return isValid;
        }

        private readonly string cmdFileName = "cmd.exe";
    }
}
