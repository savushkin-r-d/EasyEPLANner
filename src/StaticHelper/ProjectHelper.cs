﻿using Eplan.EplApi.Base;
using Eplan.EplApi.DataModel;
using Eplan.EplApi.HEServices;
using System;

namespace StaticHelper
{
    public interface IProjectHelper
    {
        /// <summary>
        /// Получить текущий проект
        /// </summary>
        /// <returns>Проект</returns>
        Project GetProject();

        /// <summary>
        /// Получить свойство проекта.
        /// </summary>
        /// <param name="propertyName">Имя свойства</param>
        /// <returns></returns>
        string GetProjectProperty(string propertyName);
    }

    public class ProjectHelper : IProjectHelper
    {
        IApiHelper apiHelper;

        public ProjectHelper(IApiHelper apiHelper)
        {
            this.apiHelper = apiHelper;
        }

        public Project GetProject()
        {
            SelectionSet selection = apiHelper.GetSelectionSet();
            const bool useDialog = false;
            Project project = selection.GetCurrentProject(useDialog);
            return project;
        }

        public string GetProjectProperty(string propertyName)
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
