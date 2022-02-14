using Eplan.EplApi.Base;
using Eplan.EplApi.DataModel;
using Eplan.EplApi.HEServices;
using StaticHelper;
using System;

namespace StaticHelper
{
    public static class ProjectHelper
    {
        /// <summary>
        /// Получить текущий проект
        /// </summary>
        /// <returns>Проект</returns>
        public static Project GetProject()
        {
            SelectionSet selection = ApiHelper.GetSelectionSet();
            const bool useDialog = false;
            Project project = selection.GetCurrentProject(useDialog);
            return project;
        }

        /// <summary>
        /// Получить свойство проекта.
        /// </summary>
        /// <param name="propertyName">Имя свойства</param>
        /// <returns></returns>
        public static string GetProjectProperty(string propertyName)
        {
            var project = GetProject();
            if (project.Properties[propertyName].IsEmpty)
            {
                string errMsg = $"Не задано свойство {propertyName}\n";
                throw new Exception(errMsg);
            }

            string result = project.Properties[propertyName]
                .ToString(ISOCode.Language.L___);
            return result;
        }
    }
}
