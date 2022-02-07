using IO;
using System.Collections.Generic;
using EasyEPlanner.PxcIolinkConfiguration.Models;
using System.IO;
using System;
using System.Xml.Serialization;
using System.Threading.Tasks;
using System.Text;
using System.Linq;

namespace EasyEPlanner.PxcIolinkConfiguration
{
    internal class PxcIolinkModulesConfiguration : IPxcIolinkConfiguration
    {
        private IIOManager ioManager;
        private Dictionary<string, LinerecorderSensor> moduleTemplates;
        private Dictionary<string, LinerecorderSensor> deviceTemplates;
        private string assemblyPath;
        private string projectFilesPath;
        private string devicesFolderPath;
        private string modulesFolderPath;
        private string createdIolConfPath;

        private const string DevicesFolderName = "Devices";
        private const string ModulesFolderName = "Modules";
        private const string IolConfFolderName = "IOL-Conf";
        private const string lrpFileSearchPattern = "*.lrp";
        private const string lrpExtension = ".lrp";

        private string templateVersion;

        public PxcIolinkModulesConfiguration(string assemblyPath,
            string projectFilesPath, IIOManager ioManager)
        {
            this.ioManager = ioManager;
            moduleTemplates = new Dictionary<string, LinerecorderSensor>();
            deviceTemplates = new Dictionary<string, LinerecorderSensor>();
            this.assemblyPath = assemblyPath;
            this.projectFilesPath = projectFilesPath;
            devicesFolderPath = string.Empty;
            modulesFolderPath = string.Empty;
            createdIolConfPath = string.Empty;
            templateVersion = string.Empty;
        }

        public void Run()
        {
            try
            {
                //Think about strict sequence.
                CreateFoldersIfNotExists();
                ReadTemplates();
                CreateModulesDescription();
            }
            catch
            {
                throw;
            }
        }

        private void CreateFoldersIfNotExists()
        {
            bool invalidAssemblyPath = string.IsNullOrEmpty(assemblyPath);
            if (invalidAssemblyPath)
            {
                throw new ArgumentException(
                    "Невозможно проверить существование папок с шаблонами IOL-Conf.");
            }

            bool invalidProjectFilesPath = string.IsNullOrEmpty(projectFilesPath);
            if (invalidProjectFilesPath)
            {
                throw new ArgumentException(
                    "Невозможно найти генерируемые файлы проекта.");
            }

            devicesFolderPath = Path.Combine(assemblyPath, IolConfFolderName, DevicesFolderName);
            modulesFolderPath = Path.Combine(assemblyPath, IolConfFolderName, ModulesFolderName);
            createdIolConfPath = Path.Combine(projectFilesPath, IolConfFolderName);

            Directory.CreateDirectory(devicesFolderPath);
            Directory.CreateDirectory(modulesFolderPath);
            Directory.CreateDirectory(createdIolConfPath);
        }

        private void ReadTemplates()
        {
            try
            {
                //TODO: Create template reader?
                var readTasks = new List<Task>();
                var readDeviceTasks = ReadTemplatesInStore(devicesFolderPath, deviceTemplates);
                var readModuleTasks = ReadTemplatesInStore(modulesFolderPath, moduleTemplates);

                if (readDeviceTasks.Count == 0)
                {
                    throw new Exception("Отсутствуют описания устройств.");
                }

                if (readModuleTasks.Count == 0)
                {
                    throw new Exception("Отсутствуют описания модулей ввода-вывода.");
                }

                readTasks.AddRange(readDeviceTasks);
                readTasks.AddRange(readModuleTasks);

                Task.WhenAll(readTasks).Wait();
            }
            catch (AggregateException ae)
            {
                throw ae.Flatten();
            }
            catch
            {
                throw;
            }
        }

        private List<Task> ReadTemplatesInStore(string pathToFolder,
            Dictionary<string, LinerecorderSensor> store)
        {
            var tasks = new List<Task>();
            var directoryInfo = new DirectoryInfo(pathToFolder);
            var paths = directoryInfo
                .GetFiles(lrpFileSearchPattern)
                .Select(x => new
                {
                    Path = x.FullName,
                    TemplateName = x.Name.Substring(0, x.Name.Length - x.Extension.Length)
                });
            foreach(var fileInfo in paths)
            {
                var task = Task
                    .Run(() => ReadTemplate(fileInfo.Path, fileInfo.TemplateName, store));
                tasks.Add(task);
            }

            return tasks;
        }

        private void ReadTemplate(string path, string templateName,
            Dictionary<string, LinerecorderSensor> store)
        {
            string xml = File.ReadAllText(path, Encoding.UTF8);
            LinerecorderSensor recorder;
            try
            {
                recorder = DeserializeSensor(xml);
            }
            catch (Exception ex)
            {
                throw new Exception($"Шаблон {path} с ошибкой.",
                    ex.InnerException);
            }

            lock (store)
            {
                if (store.ContainsKey(templateName))
                {
                    throw new InvalidDataException(
                        $"Шаблон {templateName} уже существует.");
                }

                store.Add(templateName, recorder);

                if (string.IsNullOrEmpty(templateVersion))
                {
                    templateVersion = recorder.Version;
                }
            }
        }

        private void CreateModulesDescription()
        {
            bool collectOnlyPxcIol = true;
            var plcModules = ioManager.IONodes
                .SelectMany(x => x.IOModules)
                .Where(x => x.IsIOLink(collectOnlyPxcIol));
            foreach(var plcModule in plcModules)
            {
                //TODO: Description generator class?
                LinerecorderMultiSensor sensor = CreateModuleDescription(plcModule);
                if (sensor.IsEmpty()) continue;

                string fileName = string.Concat(plcModule.Name, lrpExtension);
                string pathToSerialize = Path.Combine(createdIolConfPath, fileName);
                SerializeMultiSensor(sensor, pathToSerialize);
            }
        }

        private LinerecorderMultiSensor CreateModuleDescription(IOModule module)
        {
            Models.Device moduleDescription = CreateModuleFromTemplate(module);
            List<Models.Device> devicesDescription = CreateDevicesFromTemplate(module);
            moduleDescription.Add(devicesDescription);

            var sensor = new LinerecorderMultiSensor();
            if (moduleDescription.IsEmpty()) return sensor;

            sensor.Version = templateVersion;
            sensor.Add(moduleDescription);

            return sensor;
        }

        private Models.Device CreateModuleFromTemplate(IOModule module)
        {
            var moduleDescription = new Models.Device();
            LinerecorderSensor template;
            if (moduleTemplates.ContainsKey(module.ArticleName))
            {
                template = moduleTemplates[module.ArticleName];
            }
            else
            {
                return moduleDescription;
            }

            moduleDescription.Sensor = template.Sensor.Clone() as Sensor;
            moduleDescription.Parameters = template.Parameters.Clone() as Parameters;

            //var channels = module.DevicesChannels.SelectMany(x => x)
            //    .Where(x => x != null);
            // TODO: Change module parameters

            return moduleDescription;
        }

        private List<Models.Device> CreateDevicesFromTemplate(IOModule module)
        {
            var deviceList = new List<Models.Device>();

            //TODO: Generate device info
            //TODO: Change device parameters somehow

            return deviceList;
        }

        private LinerecorderSensor DeserializeSensor(string xml)
        {
            var serializer = new XmlSerializer(typeof(LinerecorderSensor));
            using (var reader = new StringReader(xml))
            {
                var info = (LinerecorderSensor)serializer.Deserialize(reader);
                return info;
            }
        }

        private void SerializeMultiSensor(LinerecorderMultiSensor template, string path)
        {
            var serializer = new XmlSerializer(typeof(LinerecorderMultiSensor));
            using(var fs = new FileStream(path, FileMode.Create))
            {
                serializer.Serialize(fs, template);
            }
        }
    }
}
