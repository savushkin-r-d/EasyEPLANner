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