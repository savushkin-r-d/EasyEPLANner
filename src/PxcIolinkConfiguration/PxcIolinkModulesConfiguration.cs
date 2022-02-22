using IO;
using System.Collections.Generic;
using EasyEPlanner.PxcIolinkConfiguration.Models;
using System.IO;
using System;
using System.Threading.Tasks;
using System.Linq;
using EasyEPlanner.PxcIolinkConfiguration.Interfaces;

namespace EasyEPlanner.PxcIolinkConfiguration
{
    public class PxcIolinkModulesConfiguration : IPxcIolinkConfiguration
    {
        private IXmlSensorSerializer sensorSerializer;
        private IXmlTemplateReader templateReader;
        private ISensorDescriptionBuilder sensorDescriptionBuilder;
        private Dictionary<string, LinerecorderSensor> moduleTemplates;
        private Dictionary<string, LinerecorderSensor> deviceTemplates;
        private string devicesFolderPath;
        private string modulesFolderPath;
        private string createdIolConfPath;

        private const string DevicesFolderName = "Devices";
        private const string ModulesFolderName = "Modules";
        private const string IolConfFolderName = "IOL-Conf";
        private const string lrpExtension = ".lrp";

        public PxcIolinkModulesConfiguration(IXmlSensorSerializer sensorSerializer,
            IXmlTemplateReader templateReader,
            ISensorDescriptionBuilder sensorDescriptionBuilder)
        {
            moduleTemplates = new Dictionary<string, LinerecorderSensor>();
            deviceTemplates = new Dictionary<string, LinerecorderSensor>();
            devicesFolderPath = string.Empty;
            modulesFolderPath = string.Empty;
            createdIolConfPath = string.Empty;
            this.sensorSerializer = sensorSerializer;
            this.templateReader = templateReader;
            this.sensorDescriptionBuilder = sensorDescriptionBuilder;
        }

        public bool CreateFolders(string assemblyPath, string projectFilesPath)
        {
            bool invalidAssemblyPath = string.IsNullOrEmpty(assemblyPath);
            if (invalidAssemblyPath)
            {
                throw new InvalidDataException(
                    "Невозможно проверить существование папок с шаблонами IOL-Conf.");
            }

            bool invalidProjectFilesPath = string.IsNullOrEmpty(projectFilesPath);
            if (invalidProjectFilesPath)
            {
                throw new InvalidDataException(
                    "Невозможно найти генерируемые файлы проекта.");
            }

            devicesFolderPath = Path.Combine(assemblyPath, IolConfFolderName, DevicesFolderName);
            modulesFolderPath = Path.Combine(assemblyPath, IolConfFolderName, ModulesFolderName);
            createdIolConfPath = Path.Combine(projectFilesPath, IolConfFolderName);

            Directory.CreateDirectory(devicesFolderPath);
            Directory.CreateDirectory(modulesFolderPath);
            Directory.CreateDirectory(createdIolConfPath);

            return true;
        }

        public bool ReadTemplates(bool foldersCreated)
        {
            if (!foldersCreated)
            {
                throw new InvalidOperationException(
                    "Каталоги не созданы, сначала создайте каталоги.");
            }

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

            return true;
        }

        public void CreateModulesDescription(bool templatesLoaded,
            IIOManager ioManager)
        {
            if (!templatesLoaded)
            {
                throw new InvalidOperationException(
                    "Шаблоны не загружены, загрузите шаблоны.");
            }

            bool collectOnlyPxcIol = true;
            var plcModules = ioManager.IONodes
                .SelectMany(x => x.IOModules)
                .Where(x => x.IsIOLink(collectOnlyPxcIol));
            foreach(var plcModule in plcModules)
            {
                LinerecorderMultiSensor sensor = sensorDescriptionBuilder
                    .CreateModuleDescription(plcModule, templateReader.TemplateVersion,
                    moduleTemplates, deviceTemplates);

                if (sensor.IsEmpty()) continue;

                string fileName = string.Concat(plcModule.Name, lrpExtension);
                string pathToSerialize = Path.Combine(createdIolConfPath, fileName);
                sensorSerializer.Serialize(sensor, pathToSerialize);
            }
        }
    }
}
