using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Editor;

namespace TechObject
{
    /// <summary>
    /// Параметр технологической операции.
    /// </summary>
    public class OperationParam : TreeViewItem
    {
        public OperationParam(Param par)
        {
            this.par = par;

            items =
            [
                par.ValueItem,
                par.MeterItem,
            ];
        }

        public Param Param => par;

        #region Реализация ITreeViewItem
        override public string[] DisplayText => par.DisplayText;

        public override bool IsEditable => true;

        public override string[] EditText => par.EditText;

        public override int[] EditablePart => [0, 1];

        override public ITreeViewItem[] Items => [.. items];
        #endregion

        private List<ITreeViewItem> items;
        private Param par;
    }
}
