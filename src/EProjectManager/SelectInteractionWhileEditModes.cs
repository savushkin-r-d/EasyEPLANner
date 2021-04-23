using Eplan.EplApi.DataModel;
using Eplan.EplApi.DataModel.Graphics;
using Eplan.EplApi.EServices.Ged;

namespace EasyEPlanner
{
    public class SelectInteractionWhileEditModes : Interaction
    {
        public override RequestCode OnStart(InteractionContext pContext)
        {
            EProjectManager.GetInstance().SetEditInteraction(this);

            return RequestCode.Select |
                   RequestCode.NoPreselect |
                   RequestCode.NoMultiSelect;
        }

        // Код для подсветки устройств и занесения в действие
        public override RequestCode OnSelect(
            StorableObject[] arrSelectedObjects,
            SelectionContext oContext)
        {
            base.OnSelect(arrSelectedObjects, oContext);

            if (EProjectManager.GetInstance().GetEditInteraction() != this)
            {
                return RequestCode.Stop;
            }

            Function oF = SearchSelectedObjectFunction(arrSelectedObjects[0]);
            if (oF != null)
            {
                Editor.ITreeViewItem newEditorItem = Editor.Editor
                    .GetInstance().EditorForm.GetActiveItem();
                if (newEditorItem == null)
                {
                    return RequestCode.Nothing;
                }

                if (newEditorItem != null)
                {
                    ExecuteForEditor(newEditorItem, oF);
                }
            }

            return RequestCode.Select |
                   RequestCode.NoPreselect |
                   RequestCode.NoMultiSelect;
        }

        /// <summary>
        /// Поиск функции выбранного объекта
        /// </summary>
        /// <param name="selectedObject">Выбранный объект</param>
        /// <returns></returns>
        private Function SearchSelectedObjectFunction(
            StorableObject selectedObject)
        {
            Function oF = null;
            if (selectedObject is Rectangle rect)
            {
                string oFName = rect.Properties.get_PROPUSER_TEST(1);
                oF = Function.FromStringIdentifier(oFName) as Function;
            }

            if (selectedObject is Function func)
            {
                oF = func;
            }

            return oF;
        }

        /// <summary>
        /// Обработка для нового редактора
        /// </summary>
        /// <param name="editorItem">Элемент из редактора</param>
        /// <param name="oF">Функция</param>
        private void ExecuteForEditor(Editor.ITreeViewItem editorItem,
            Function oF)
        {
            if (editorItem.IsUseDevList)
            {
                string devName;
                bool res = Device.DeviceManager.GetInstance()
                    .CheckDeviceName(oF.Name, out devName, out _, out _,
                    out _, out _);

                if (res)
                {
                    string checkedDevices = editorItem.EditText[1];
                    string newDevices = MakeNewCheckedDevices(devName,
                        checkedDevices);
                    Editor.Editor.GetInstance().EditorForm
                        .SetNewVal(newDevices);

                    // Обновление списка устройств при его наличии.
                    if (DFrm.GetInstance().IsVisible() == true)
                    {
                        DFrm.OnSetNewValue onSetNewValue = null;
                        DFrm.GetInstance().ShowDisplayObjects(editorItem,
                            onSetNewValue);
                    }
                }
            }
        }

        /// <summary>
        /// Сгенерировать новую строку устройств
        /// </summary>
        /// <param name="devName">Устройство</param>
        /// <param name="checkedDevices">Текущие устройства</param>
        /// <returns></returns>
        private string MakeNewCheckedDevices(string devName,
            string checkedDevices)
        {
            string oldDevices = " " + checkedDevices + " ";
            string newDevices;
            // Для корректного поиска отделяем пробелами.
            devName = " " + devName + " ";

            if (oldDevices.Contains(devName))
            {
                newDevices = oldDevices.Replace(devName, " ");
            }
            else
            {
                newDevices = oldDevices + devName.Trim();
            }

            return newDevices;
        }

        private delegate void DelegateWithParameters(int param1);

        public override void OnStop()
        {
            base.OnStop();
        }

        public void Stop()
        {
            IsFinish = true;
        }

        public bool IsFinish { get; set; } = false;
    }
}