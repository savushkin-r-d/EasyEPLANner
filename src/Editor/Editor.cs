using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

namespace Editor
{
    /// <summary>    
    /// Класс, реализующий редактор.
    /// </summary>
    public class Editor : IEditor, System.IDisposable
    {
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                editorForm.Dispose();
            }
        }

        private Editor() { }

        /// <summary>    
        /// Вызов окна редактирования технологических объектов.
        /// </summary>
        /// <param name="objectTree">Дерево объектов</param>
        public void OpenEditor(ITreeViewItem objectTree)
        {
            if (editorForm.wasInit == false)
            {
                editorForm.Init(objectTree);
                objectTree.AddParent(null);
                editorForm.wasInit = true;
            }

            editorForm.ShowDlg();
        }

        public bool IsShown()
        {
            return editorForm.IsShown;
        }

        public void CloseEditor()
        {
            editorForm.CloseEditor();
        }

        public void RefreshEditor()
        {
            editorForm.RefreshTree();
        }

        [ExcludeFromCodeCoverage]
        public DialogResult DialogResetExtraProperties()
        {
            return MessageBox.Show(
                "Сбросить доп.свойства привязанных агрегатов и базовые шаги?",
                "EPlaner",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Exclamation);
        }

        [ExcludeFromCodeCoverage]
        public DialogResult DialogDeletingGenericGroup()
        {
            return MessageBox.Show(
                "Удалить также все технологические объекты в группе?",
                "Внимание",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
        }

        [ExcludeFromCodeCoverage]
        public bool Editable => editorForm.Editable;

        /// <summary>
        /// Получение экземпляра класса.
        /// </summary>
        /// <returns>Единственный экземпляр класса.</returns>
        public static Editor GetInstance()
        {
            if (null == instance)
            {
                instance = new Editor();
                instance.editorForm = new NewEditorControl();
            }
            return instance;
        }

        /// <summary>
        /// Форма редактора технологических объектов.
        /// </summary>
        public NewEditorControl EditorForm
        {
            get
            {
                return editorForm;
            }
        }

        /// <summary>
        /// Форма редактора технологических объектов
        /// </summary>
        private NewEditorControl editorForm;

        /// <summary>
        /// Экземпляр класса редактора.
        /// </summary>
        private static Editor instance;
    }
}