using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
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
            filterConfiguration = FilterConfiguration.GetInstance();
            filterConfiguration.SignalsFilterChanged += RefilterListViews;

            currProjItems = new List<ListViewItem>();
            advProjItems = new List<ListViewItem>();
        }

        private FilterConfiguration filterConfiguration;
        private InterprojectExchange interprojectExchange;

        private List<ListViewItem> currProjItems;
        private List<ListViewItem> advProjItems;

        private void InterprojectExchangeForm_FormClosed(object sender,
            FormClosedEventArgs e)
        {
            filterConfiguration.Dispose();
            Dispose();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
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
                item.Tag = devInfo.Type;
                currProjItems.Add(item);
            }

            //Mock для заполнение якобы других проектов
            foreach (var devInfo in currentProjDevs)
            {
                var item = new ListViewItem(
                    new string[] { devInfo.Name, devInfo.Description });
                item.Tag = devInfo.Type;
                advProjItems.Add(item);
            }

            RefilterListViews();
        }

        /// <summary>
        /// Обновить списки под фильтр
        /// </summary>
        private void RefilterListViews()
        {
            currentProjSignalsList.Items.Clear();
            advancedProjSignalsList.Items.Clear();

            var filteredCurrProjItems = filterConfiguration.FilterOut(
                currProjItems, FilterConfiguration.FilterList.Current);
            var filteredAdvProjItems = filterConfiguration.FilterOut(
                advProjItems, FilterConfiguration.FilterList.Advanced);
            currentProjSignalsList.Items.AddRange(filteredCurrProjItems);
            advancedProjSignalsList.Items.AddRange(filteredAdvProjItems);

            bool useGroupsFilter = filterConfiguration.FilterParameters[
                "bindedSignalsList"]["groupAsPairsCheckBox"];
            bindedSignalsList.ShowGroups = useGroupsFilter;
        }

        private void filterButton_Click(object sender, EventArgs e)
        {
            filterConfiguration.ShowForm();
        }

        private void advancedProjSignalsList_ItemSelectionChanged(object sender,
            ListViewItemSelectionChangedEventArgs e)
        {
            string selectedItem = e.Item.Text;
            SelectedListViewItemCollection oppositeItems =
                currentProjSignalsList.SelectedItems;

            bool needChange = (selectedItem != null &&
                oppositeItems.Count != 0 &&
                e.IsSelected);
            bool needAddNewElement = bindedSignalsList.SelectedItems.Count == 0;
            if (needChange)
            {
                if (needAddNewElement)
                {
                    var oppositeItem = oppositeItems[0].SubItems[1];
                    object oppositeItemTag = oppositeItems[0].Tag;
                    AddToBindedSignals(oppositeItemTag, oppositeItem.Text, 
                        selectedItem);
                }
                else
                {
                    var selectedRow = bindedSignalsList.SelectedItems[0];
                    if (selectedRow != null)
                    {
                        selectedRow.SubItems[1].Text = selectedItem;
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
            if (needChange)
            {
                bool needAddNewElement = bindedSignalsList.SelectedItems
                    .Count == 0;
                if (needAddNewElement)
                {
                    string oppositeItem = oppositeItems[0].Text;
                    AddToBindedSignals(e.Item.Tag, selectedItem, oppositeItem);
                }
                else
                {
                    var selectedRow = bindedSignalsList.SelectedItems[0];
                    if (selectedRow != null)
                    {
                        selectedRow.SubItems[0].Text = selectedItem;
                    }
                }
            }
        }

        /// <summary>
        /// Добавить связь между объектами
        /// </summary>
        private void AddToBindedSignals(object type, string selectedItem, 
            string oppositeItem)
        {
            ListViewGroup itemGroup;
            switch(type.ToString())
            {
                case "AO":
                    itemGroup = bindedSignalsList.Groups["AO"];
                    break;
                case "AI":
                    itemGroup = bindedSignalsList.Groups["AI"];
                    break;
                case "DO":
                    itemGroup = bindedSignalsList.Groups["DO"];
                    break;
                case "DI":
                    itemGroup = bindedSignalsList.Groups["DI"];
                    break;
                default:
                    itemGroup = bindedSignalsList.Groups["Other"];
                    break;
            }

            var item = new ListViewItem(
                new string[] { selectedItem, oppositeItem }, itemGroup);
            bindedSignalsList.Items.Add(item);
            ClearAllGridAndListViewsSelection();
        }

        private void bindedSignalsList_KeyDown(object sender, KeyEventArgs e)
        {
            bool endEdit = e.KeyCode == Keys.Escape ||
                e.KeyCode == Keys.Enter;
            if (endEdit)
            {
                ClearAllGridAndListViewsSelection();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Delete)
            {
                var selectedItems = bindedSignalsList.SelectedItems;
                if(selectedItems != null && selectedItems.Count > 0)
                {
                    var selectedItem = selectedItems[0];
                    DeleteItemFromBindedSignals(selectedItem);                                   
                }

                if (bindedSignalsList.Items.Count == 0)
                {
                    ClearAllGridAndListViewsSelection();
                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// Удаление элемента из списка связанных сигналов
        /// </summary>
        /// <param name="selectedItem"></param>
        private void DeleteItemFromBindedSignals(ListViewItem selectedItem)
        {
            var selectedItemIndex = selectedItem.Index;
            bindedSignalsList.Items.Remove(selectedItem);

            if (selectedItemIndex >= 0 && bindedSignalsList.Items.Count > 0)
            {
                if (bindedSignalsList.Items.Count > selectedItemIndex)
                {
                    var newSelectedItem = bindedSignalsList.Items[
                    selectedItemIndex];
                    newSelectedItem.Selected = true;
                }
                else if(bindedSignalsList.Items.Count == selectedItemIndex)
                {
                    var newSelectedItem = bindedSignalsList.Items[
                            selectedItemIndex - 1];
                    newSelectedItem.Selected = true;
                }
            } 
        }

        /// <summary>
        /// Очищает выделение в списках и таблице.
        /// </summary>
        private void ClearAllGridAndListViewsSelection()
        {
            bindedSignalsList.SelectedIndices.Clear();
            currentProjSignalsList.SelectedIndices.Clear();
            advancedProjSignalsList.SelectedIndices.Clear();
        }

        private void bindedSignalsList_MouseClick(object sender, 
            MouseEventArgs e)
        {
            ListViewItem item = bindedSignalsList.GetItemAt(e.X, e.Y);
            if (item != null)
            {
                var selectedItem = bindedSignalsList.SelectedItems[0];
                HighlightObjectsInListViews(selectedItem);
            }
        }

        private void bindedSignalsList_MouseDown(object sender, 
            MouseEventArgs e)
        {
            ListViewItem item = bindedSignalsList.GetItemAt(e.X, e.Y);
            if (item == null)
            {
                ClearAllGridAndListViewsSelection();
            }
        }

        private void bindedSignalsList_ItemSelectionChanged(object sender,
            ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                HighlightObjectsInListViews(e.Item);
            }
        }

        /// <summary>
        /// Подсветить объекты в списках, которые находятся в выделенной строке
        /// таблицы
        /// </summary>
        /// <param name="selectedItem">Выделенная в таблице строка</param>
        private void HighlightObjectsInListViews(ListViewItem selectedItem)
        {
            try
            {
                var currProjDevText = selectedItem.SubItems[0].Text;
                var advProjDevText = selectedItem.SubItems[1].Text;

                var currProjItem = currentProjSignalsList
                    .FindItemWithText(currProjDevText);
                var advProjItem = advancedProjSignalsList
                    .FindItemWithText(advProjDevText);
                if (currProjItem != null && advProjItem != null)
                {
                    currProjItem.Selected = true;
                    currProjItem.EnsureVisible();
                    advProjItem.Selected = true;
                    advProjItem.EnsureVisible();
                }
            }
            catch
            {
                string message = "Ошибка подсветки выделенных " +
                    "элементов в списках";
                MessageBox.Show(message);
            }
        }

        private void currentProjSignalsList_KeyPress(object sender,
            KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                return;
            }

            ProcessingKeysFromListViewToTextBox(e, currProjSearchBox);
        }

        private void advancedProjSignalsList_KeyPress(object sender,
            KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                return;
            }

            ProcessingKeysFromListViewToTextBox(e, advProjSearchBox);
        }

        /// <summary>
        /// Обработка нажатий кнопок на ListView и внесение их в TextBox
        /// </summary>
        /// <param name="e">Событие обработки кнопок</param>
        /// <param name="textBox">ТекстБокс для изменений</param>
        private void ProcessingKeysFromListViewToTextBox(KeyPressEventArgs e,
            TextBox textBox)
        {
            bool deleteChar = e.KeyChar == (char)Keys.Delete ||
                e.KeyChar == (char)Keys.Back;
            bool addChar = char.IsLetterOrDigit(e.KeyChar) ||
                char.IsWhiteSpace(e.KeyChar);
            if (deleteChar)
            {
                if (textBox.Text.Length > 0)
                {
                    string currentText = textBox.Text;
                    string newText = currentText
                        .Remove(currentText.Length - 1);
                    textBox.Text = newText;
                }
            }
            else if (addChar)
            {
                textBox.Text += e.KeyChar;
            }
        }

        private void currProjSearchBox_TextChanged(object sender, EventArgs e)
        {
            var filteredThroughType = filterConfiguration.FilterOut(
                currProjItems, FilterConfiguration.FilterList.Current);
            SearchingSubStringInListView(currentProjSignalsList,
                currProjSearchBox.Text, filteredThroughType);
        }

        private void advProjSearchBox_TextChanged(object sender, EventArgs e)
        {
            var filteredThroughType = filterConfiguration.FilterOut(
                advProjItems, FilterConfiguration.FilterList.Advanced);
            SearchingSubStringInListView(advancedProjSignalsList,
                advProjSearchBox.Text, filteredThroughType);
        }

        /// <summary>
        /// Поиска подстроки в ListView
        /// </summary>
        /// <param name="listView">ListView</param>
        /// <param name="subString">Подстрока</param>
        /// <param name="projItems">Массив элементов в проекте</param>
        private void SearchingSubStringInListView(ListView listView,
            string subString, ListViewItem[] projItems)
        {
            if (subString != "")
            {
                var lowerSubString = subString.ToLower();
                var filteredItems = projItems
                    .Where(x => x.SubItems[0].Text.ToLower()
                    .Contains(lowerSubString) || x.SubItems[1].Text.ToLower()
                    .Contains(lowerSubString))
                    .ToArray();
                listView.Items.Clear();
                listView.Items.AddRange(filteredItems);
            }
            else
            {
                listView.Items.Clear();
                listView.Items.AddRange(projItems);
            }
        }

        private void delAdvProjButton_Click(object sender, EventArgs e)
        {
            if (advProjNameComboBox.Items.Count > 0)
            {
                DialogResult delete = MessageBox.Show($"Удалить обмен с проектом " +
                    $"\"{advProjNameComboBox.Text}\"", "Внимание",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (delete == DialogResult.No)
                {
                    return;
                }

                //TODO: Пометка связи на удаление
                //TODO: Удаление из списка
                //TODO: Без физического удаления
            }
        }

        private void addAdvProjButton_Click(object sender, EventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog();
            DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                //TODO: Проверить, добавлен ли такой проект
                //TODO: Если да - ошибка
                //TODO: Если нет - записываем и читаем данные в модель
            }
        }
    }
}
