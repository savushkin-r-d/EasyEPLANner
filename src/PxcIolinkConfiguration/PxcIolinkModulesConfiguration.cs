using IO;
using System.Collections.Generic;
using EasyEPlanner.PxcIolinkConfiguration.Models;
using System.IO;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace EasyEPlanner.PxcIolinkConfiguration
{
    internal class PxcIolinkModulesConfiguration : IPxcIolinkConfiguration
    {
        private IIOManager ioManager;
        private ISensorSerializer sensorSerializer;
        private ITemplateReader templateReader;
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
        private const string lrpExtension = ".lrp";

        public PxcIolinkModulesConfiguration(string assemblyPath,
            string projectFilesPath, IIOManager ioManager,
            ISensorSerializer sensorSerializer,
            ITemplateReader templateReader)
        {
            this.ioManager = ioManager;
            moduleTemplates = new Dictionary<string, LinerecorderSensor>();
            deviceTemplates = new Dictionary<string, LinerecorderSensor>();
            this.assemblyPath = assemblyPath;
            this.projectFilesPath = projectFilesPath;
            devicesFolderPath = string.Empty;
            modulesFolderPath = string.Empty;
            createdIolConfPath = string.Empty;
            this.sensorSerializer = sensorSerializer;
            this.templateReader = templateReader;
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
                var readTasks = new List<Task>();
                List<Task> readDeviceTasks = templateReader.Read(devicesFolderPath, deviceTemplates);
                List<Task> readModuleTasks = templateReader.Read(modulesFolderPath, moduleTemplates);

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
                sensorSerializer.Serialize(sensor, pathToSerialize);
            }
        }

        private LinerecorderMultiSensor CreateModuleDescription(IOModule module)
        {
            Models.Device moduleDescription = CreateModuleFromTemplate(module);
            List<Models.Device> devicesDescription = CreateDevicesFromTemplate(module);
            moduleDescription.Add(devicesDescription);

            var sensor = new LinerecorderMultiSensor();
            if (moduleDescription.IsEmpty()) return sensor;

            sensor.Version = templateReader.TemplateVersion;
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
    }
}
