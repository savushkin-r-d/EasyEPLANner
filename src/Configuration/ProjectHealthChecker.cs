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
            const string luaCheckScriptFileName = "TestLuaAvailability.txt";
            string arguments = GetArguments(luaCheckScriptFileName);

            var cmdProcess = new Process();
            var startInfo = new ProcessStartInfo()
            {
                FileName = cmdFileName,
                Arguments = arguments,
                UseShellExecute = false,
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
            const string projectCheckScriptFileName = "TestProjectScript.txt";
            string arguments = GetArguments(projectCheckScriptFileName);
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
        private string GetArguments(string fileName)
        {
            string pathToFile = 
                Path.Combine(projectManager.OriginalCMDFilesPath, fileName);

            string[] arguments = File.ReadAllLines(pathToFile,
                System.Text.Encoding.GetEncoding(1251));

            return string.Join(connectCommandsChar, arguments);
        }

        private readonly string connectCommandsChar = "&";
        private readonly string cmdFileName = "cmd.exe";
        private ProjectManager projectManager;
        private EProjectManager eProjectManager;
    }
}
