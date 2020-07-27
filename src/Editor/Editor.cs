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
                frm.Dispose();
            }
        }

        private Editor() { }

        /// <summary>    
        /// Вызов окна редактирования технологических объектов.
        /// </summary>        
        public string Edit(ITreeViewItem data)
        {
            if (frm.wasInit == false)
            {
                frm.Init(data);
                frm.wasInit = true;
            }

            frm.ShowDlg();

            return "";
        }

        public bool IsShown()
        {
            return frm.IsShown;
        }

        public void CloseEditor()
        {
            frm.CloseEditor();
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
                instance.frm = new EditorCtrl();
            }
            return instance;
        }

        public EditorCtrl EForm
        {
            get
            {
                return frm;
            }
        }

        private EditorCtrl frm;      ///Окно редактора.
        private static Editor instance; ///Экземпляр класса.
    }
}