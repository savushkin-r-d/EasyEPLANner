using System.Diagnostics;
using System.IO;

namespace EasyEPlanner
{
    /// <summary>
    /// Тестер работоспособности проекта
    /// </summary>
    public class ProjectHealthChecker
    {
        public ProjectHealthChecker()
        {
            projectManager = ProjectManager.GetInstance();
            eProjectManager = EProjectManager.GetInstance();
        }

        /// <summary>
        /// Протестировать текущий проект на наличие ошибок.
        /// </summary>
        public string Check()
        {
            bool validLUA = CheckLua();
            if (validLUA)
            {
                string errors;
                bool validProject = CheckProject(out errors);
                if (validProject)
                {
                    string message = "Тестирование проекта прошло успешно. " +
                        "Проект работоспособен. ";
                    return message;
                }
                else
                {
                    string message = "Тестирование проекта завершилось с " +
                        "ошибками. Проект не работоспособен. ";
                    return errors + StaticHelper.CommonConst.NewLine + message;
                }
            }
            else
            {
                string message = "В системе не найден LUA, тестирование " +
                    "проекта невозможно. ";
                return message;
            }
        }

        /// <summary>
        /// Проверка доступности LUA в системе
        /// </summary>
        /// <returns></returns>
        private bool CheckLua()
        {
            bool isValid = false;
            string arguments = GetArgumentsForCheckLua();

            var cmdProcess = new Process();
            var startInfo = new ProcessStartInfo()
            {
                FileName = cmdFileName,
                Arguments = arguments,
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Hidden,
            };
            cmdProcess.StartInfo = startInfo;

            cmdProcess.Start();
            cmdProcess.WaitForExit();

            int exitCode = cmdProcess.ExitCode;
            if (exitCode == 0)
            {
                isValid = true;
            }

            return isValid;

        }

        // <summary>
        // Проверка работоспособности проекта
        // </summary>
        // <returns></returns>
        private bool CheckProject(out string errors)
        {
            bool isValid = false;
            string arguments = GetArgumentsForCheckProject();
            string workingDirectory = GetWorkingDirectory();

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
            cmdProcess.WaitForExit();

            int exitCode = cmdProcess.ExitCode;
            if (exitCode == 0)
            {
                isValid = true;
                errors = "";
            }
            else
            {
                StreamReader reader = cmdProcess.StandardOutput;
                string output = reader.ReadToEnd();
                errors = output;
            }

            return isValid;
        }

        /// <summary>
        /// Получить рабочий каталог с проектом.
        /// </summary>
        /// <returns></returns>
        private string GetWorkingDirectory()
        {
            string result;

            string projName = eProjectManager.GetModifyingCurrentProjectName();
            string directory = projectManager.GetPtusaProjectsPath(projName);
            result = Path.Combine(directory, projName);

            return result;
        }
        
        /// <summary>
        /// Получить аргументы командной строки для проверки доступности LUA
        /// </summary>
        /// <returns></returns>
        private string GetArgumentsForCheckLua()
        {
            string result = "";
            // "/C lua -v";
            return result;
        }

        /// <summary>
        /// Получить аргументы командной строки для проверки проекта.
        /// </summary>
        /// <returns></returns>
        private string GetArgumentsForCheckProject()
        {
            string result = "";
            // "/C chcp1251&lua ..\\..\\spec\\main.lua -o TAP";
            return result;
        }

        private readonly string cmdFileName = "cmd.exe";
        private ProjectManager projectManager;
        private EProjectManager eProjectManager;
    }
}
