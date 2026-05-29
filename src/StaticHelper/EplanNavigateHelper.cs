using Eplan.EplApi.DataModel;
using System;
using System.Diagnostics.CodeAnalysis;

namespace StaticHelper
{
    /// <summary>
    /// Переход на страницу ФСА по функции EPLAN.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class EplanNavigateHelper
    {
        public static void OpenFunctionPage(Function function)
        {
            var project = function.Page?.Project
                ?? throw new InvalidOperationException(
                    "Не найден проект страницы выбранного объекта.");

            var edit = new Eplan.EplApi.HEServices.Edit();
            edit.OpenPageWithName(project.ProjectLinkFilePath, function.Page.Name);
        }

        public static bool TryGetFunction(IEplanFunction eplanFunction, out Function function)
        {
            function = null;
            if (eplanFunction is not EplanFunction functionWrapper ||
                functionWrapper.Function?.IsValid != true ||
                functionWrapper.Function.Page is null)
            {
                return false;
            }

            function = functionWrapper.Function;
            return true;
        }
    }
}
