using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Device;
using static System.Windows.Forms.ListView;

namespace EasyEPlanner
{
    public partial class InterprojectExchangeForm : Form
    {
        public InterprojectExchangeForm()
        {
            InitializeComponent();

            string projectName = EProjectManager.GetInstance()
                .GetCurrentProjectName();
            currProjNameTextBox.Text = projectName;

            interprojectExchange = InterprojectExchange.GetInstance();
        }

        private void InterprojectExchangeForm_FormClosed(object sender, 
            FormClosedEventArgs e)
        {
            if (filterForm != null && filterForm.IsDisposed == false)
            {
                filterForm.Close();
            }
            this.Dispose();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void InterprojectExchangeForm_Load(object sender, EventArgs e)
        {
            LoadCurrentProjectDevices();                        
        }

        /// <summary>
        /// Загрузка устройств текущего проекта.
        /// </summary>
        private void LoadCurrentProjectDevices()
        {
            var currentProjDevs = interprojectExchange
                .GetProjectDevices(currProjNameTextBox.Text);
            foreach (var devInfo in currentProjDevs)
            {
                var dev = new string[] { devInfo.Description, devInfo.Name };
                var item = new ListViewItem(dev);
                currentProjSignalsList.Items.Add(item);
            }

            //Mock для заполнение якобы других проектов
            foreach (var devInfo in currentProjDevs)
            {
                var item = new ListViewItem(
                    new string[] { devInfo.Name, devInfo.Description });
                advancedProjSignalsList.Items.Add(item);
            }
        }

        private void filterButton_Click(object sender, EventArgs e)
        {
            if (filterForm == null || filterForm.IsDisposed)
            {
                filterForm = new FilterForm();
            }

            filterForm.ShowDialog();
        }

        private FilterForm filterForm;
        private InterprojectExchange interprojectExchange;

        private void advancedProjSignalsList_ItemSelectionChanged(object sender, 
            ListViewItemSelectionChangedEventArgs e)
        {
            string selectedItem = e.Item.Text;
            SelectedListViewItemCollection oppositeItems = 
                currentProjSignalsList.SelectedItems;

            bool needChange = (selectedItem != null &&
                oppositeItems.Count != 0 &&
                e.IsSelected);
            bool needAddNewElement = bindedSignalsGrid.SelectedRows.Count == 0;
            if(needChange)
            {
                if(needAddNewElement)
                {
                    string oppositeItem = oppositeItems[0].SubItems[1].Text;
                    bindedSignalsGrid.Rows.Add(oppositeItem, selectedItem);
                    ClealAllGridAndListViewsSelection();
                }
                else
                {
                    var selectedRow = bindedSignalsGrid.SelectedRows[0];
                    if (selectedRow != null)
                    {
                        var advProjDev = selectedRow.Cells[1];
                        advProjDev.Value = selectedItem;
                    }
                }
            }
        }

        private void currentProjSignalsList_ItemSelectionChanged(object sender, 
            ListViewItemSelectionChangedEventArgs e)
        {
            string selectedItem = e.Item.SubItems[1].Text;
            SelectedListViewItemCollection oppositeItems = 
                advancedProjSignalsList.SelectedItems;

            bool needChange = (selectedItem != null &&
                oppositeItems.Count != 0 &&
                e.IsSelected);
            if(needChange)
            {
                bool needAddNewElement = bindedSignalsGrid.SelectedRows
                    .Count == 0;
                if (needAddNewElement)
                {
                    string oppositeItem = oppositeItems[0].Text;
                    bindedSignalsGrid.Rows.Add(selectedItem, oppositeItem);
                    ClealAllGridAndListViewsSelection();
                }
                else
                {
                    var selectedRow = bindedSignalsGrid.SelectedRows[0];
                    if (selectedRow != null)
                    {
                        var currProjDev = selectedRow.Cells[0];
                        currProjDev.Value = selectedItem;
                    }
                }
            }
        }

        private void bindedSignalsGrid_KeyDown(object sender, KeyEventArgs e)
        {
            bool endEdit = e.KeyCode == Keys.Escape ||
                e.KeyCode == Keys.Enter;
            if (endEdit)
            {
                ClealAllGridAndListViewsSelection();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Delete)
            {
                var selectedRow = bindedSignalsGrid.SelectedRows[0];
                bindedSignalsGrid.Rows.Remove(selectedRow);
                if (bindedSignalsGrid.Rows.Count == 0)
                {
                    ClealAllGridAndListViewsSelection();
                }
                e.Handled = true;
            }
        }

        private void bindedSignalsGrid_MouseClick(object sender, MouseEventArgs e)
        {
            var hitTestInfo = bindedSignalsGrid.HitTest(e.X, e.Y);
            if (hitTestInfo.Type == DataGridViewHitTestType.None)
            {
                ClealAllGridAndListViewsSelection();
            }
            else if (hitTestInfo.Type == DataGridViewHitTestType.Cell)
            {
                var selectedRow = bindedSignalsGrid.SelectedRows[0];
                HighlightObjectsInListViews(selectedRow);           
            }
        }

        /// <summary>
        /// Очищает выделение в списках и таблице.
        /// </summary>
        private void ClealAllGridAndListViewsSelection()
        {
            bindedSignalsGrid.ClearSelection();
            currentProjSignalsList.SelectedIndices.Clear();
            advancedProjSignalsList.SelectedIndices.Clear();
        }

        /// <summary>
        /// Подсветить объекты в списках, которые находятся в выделенной строке
        /// таблицы
        /// </summary>
        /// <param name="selectedRow">Выделенная в таблице строка</param>
        private void HighlightObjectsInListViews(DataGridViewRow selectedRow)
        {
            try
            {
                var currProjDev = selectedRow.Cells[0];
                var advProjDev = selectedRow.Cells[1];

                var currProjDevText = currProjDev.Value.ToString();
                var advProjDevText = advProjDev.Value.ToString();

                var currProjItem = currentProjSignalsList
                    .FindItemWithText(currProjDevText);
                var advProjItem = advancedProjSignalsList
                    .FindItemWithText(advProjDevText);
                if (currProjItem != null && advProjItem != null)
                {
                    currProjItem.Selected = true;
                    advProjItem.Selected = true;
                }
            }
            catch
            {
                string message = "Ошибка подсветки выделенных " +
                    "элементов в списках";
                MessageBox.Show(message);
            }
        }

        private void bindedSignalsGrid_RowStateChanged(object sender, 
            DataGridViewRowStateChangedEventArgs e)
        {
            if (e.Row.Selected)
            {
                HighlightObjectsInListViews(e.Row);
            }
        }
    }
}
