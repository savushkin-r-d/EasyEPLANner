using Device;
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
        private IDeviceManager deviceManager;
        private IIOManager ioManager;
        private Dictionary<string, LinerecorderSensor> moduleTemplates;
        private Dictionary<string, LinerecorderSensor> deviceTemplates;
        private string assemblyPath;
        private string projectFilesPath;
        private string pathToDevicesFolder;
        private string pathToModulesFolder;

        private const string DevicesFolder = "Devices";
        private const string ModulesFolder = "Modules";
        private const string IolConfFolder = "IOL-Conf";
        private const string lrpExtension = "*.lrp";

        public PxcIolinkModulesConfiguration(string assemblyPath,
            string projectFilesPath, IDeviceManager deviceManager,
            IIOManager ioManager)
        {
            this.deviceManager = deviceManager;
            this.ioManager = ioManager;
            moduleTemplates = new Dictionary<string, LinerecorderSensor>();
            deviceTemplates = new Dictionary<string, LinerecorderSensor>();
            this.assemblyPath = assemblyPath;
            this.projectFilesPath = projectFilesPath;
            pathToDevicesFolder = string.Empty;
            pathToModulesFolder = string.Empty;
        }

        public void Run()
        {
            try
            {
                CreateFoldersIfNotExists();
                ReadTemplates();
            }
            catch
            {
                throw;
            }
        }

        private void CreateFoldersIfNotExists()
        {
            if (assemblyPath == null)
            {
                throw new ArgumentException(
                    "Невозможно проверить существование папок с шаблонами IOL-Conf.");
            }

            pathToDevicesFolder = Path.Combine(assemblyPath, IolConfFolder, DevicesFolder);
            pathToModulesFolder = Path.Combine(assemblyPath, IolConfFolder, ModulesFolder);

            Directory.CreateDirectory(pathToDevicesFolder);
            Directory.CreateDirectory(pathToModulesFolder);
        }

        private void ReadTemplates()
        {
            try
            {
                var readTasks = new List<Task>();
                var readDeviceTasks = ReadTemplatesInStore(pathToDevicesFolder, deviceTemplates);
                var readModuleTasks = ReadTemplatesInStore(pathToModulesFolder, moduleTemplates);

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
            var paths = directoryInfo.GetFiles(lrpExtension)
                .Select(x => x.FullName);
            foreach(var path in paths)
            {
                var task = Task
                    .Run(() => ReadTemplate(path, store));
                tasks.Add(task);
            }

            return tasks;
        }

        private void ReadTemplate(string path, 
            Dictionary<string, LinerecorderSensor> store)
        {
            var xml = File.ReadAllText(path, Encoding.UTF8);
            LinerecorderSensor recorder;
            try
            {
                recorder = DeserializeXmlTemplate(xml);
            }
            catch (Exception ex)
            {
                throw new Exception($"Шаблон {path} с ошибкой.",
                    ex.InnerException);
            }

            lock (store)
            {
                var devName = recorder.Sensor.ProductId;
                if (store.ContainsKey(devName))
                {
                    throw new InvalidDataException(
                        $"Шаблон {devName} уже существует.");
                }

                store.Add(devName, recorder);
            }
        }

        private LinerecorderSensor DeserializeXmlTemplate(string xml)
        {
            var serializer = new XmlSerializer(typeof(LinerecorderSensor));
            using (var reader = new StringReader(xml))
            {
                var info = (LinerecorderSensor)serializer.Deserialize(reader);
                return info;
            }
        }
    }
}
