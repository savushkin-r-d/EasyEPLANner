using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.DataModel;
using Eplan.EplApi.HEServices;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

namespace StaticHelper
{
    /// <summary>
    /// Показ функции на схеме EPLAN: открытие страницы, выделение и приближение к объекту.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class EplanNavigateHelper
    {
        private static readonly string[] ZoomToSelectionActionNames =
        {
            "XGedZoomMarkedObjectsAction",
            "XGedZoomMarkedAction",
            "gedZoomMarkedObjects",
        };

        public static void OpenFunctionPage(Function function)
        {
            var project = function.Page?.Project
                ?? throw new InvalidOperationException(
                    "Не найден проект страницы выбранного объекта.");

            var edit = new Edit();
            OpenPageAndSelect(edit, function, project.ProjectLinkFilePath);
            edit.SetFocusToGED();
            TryZoomToSelection();
        }

        public static void OpenFunctionPageWithError(Function function)
        {
            try
            {
                OpenFunctionPage(function);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, FasNavigationTexts.ErrorCaption,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        private static void OpenPageAndSelect(Edit edit, Function function, string projectPath)
        {
            try
            {
                edit.OpenPageWithPlacement(function);
                return;
            }
            catch
            {
                // fallback below
            }

            if (!string.IsNullOrEmpty(function.VisibleName))
            {
                edit.OpenPageWithNameAndDeviceName(
                    projectPath, function.Page.Name, function.VisibleName);
                return;
            }

            edit.OpenPageWithName(projectPath, function.Page.Name);
        }

        private static void TryZoomToSelection()
        {
            try
            {
                var cli = new CommandLineInterpreter();
                foreach (var actionName in ZoomToSelectionActionNames)
                {
                    try
                    {
                        if (cli.Execute(actionName))
                            return;
                    }
                    catch
                    {
                        // пробуем следующее имя внутреннего действия EPLAN
                    }
                }
            }
            catch
            {
                // приближение необязательно: страница уже открыта и объект выделен
            }
        }
    }
}
