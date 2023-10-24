﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Assemblies;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner
{
    [ExcludeFromCodeCoverage]
    public class AppConfiguarationManager
    {
        protected AppConfiguarationManager() { }

        private static Configuration InitConfig()
        {
            try
            {
                var original = ProjectManager.GetInstance().OriginalAssemblyPath;
                var location = Assembly.GetExecutingAssembly().Location;

                var path = Path.Combine(original, location.Split('\\').LastOrDefault());

                return ConfigurationManager.OpenExeConfiguration(path);
            }
            catch
            {
                return null;
            }
        }

        public static string GetAppSetting(string key)
        {
            KeyValueConfigurationElement element = config?.AppSettings.Settings[key];
            if (element != null)
            {
                string value = element.Value;
                if (!string.IsNullOrEmpty(value))
                    return value;
            }
            return string.Empty;
        }

        public static void SetAppSetting(string key, string value)
        {
            config.AppSettings.Settings.Remove(key);
            config.AppSettings.Settings.Add(key, value);
            config.Save();
        }

        private static Configuration config = InitConfig();
    }
}
