using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.ListView;

namespace InterprojectExchange
{
    public partial class InterprojectExchangeForm : Form
    {
        public InterprojectExchangeForm()
        {
            InitializeComponent();

            // Получение основных экземпляров классов, подписка на событие
            // обновления списков через фильтр
            interprojectExchange = InterprojectExchange.GetInstance();
            filterConfiguration = FilterConfiguration.GetInstance();
            filterConfiguration.SignalsFilterChanged += RefilterListViews;

            // Инициализация начальных списков
            currProjItems = new List<ListViewItem>();
            advProjItems = new List<ListViewItem>();

            // Установка имени текущего проекта
            string projectName = interprojectExchange.GetCurrentProjectName();
            currProjNameTextBox.Text = projectName;
        }

        private FilterConfiguration filterConfiguration;
        private InterprojectExchange interprojectExchange;

        private List<ListViewItem> currProjItems;
        private List<ListViewItem> advProjItems;

        /// <summary>
        /// Событие после закрытия формы
        /// </summary>
        private void InterprojectExchangeForm_FormClosed(object sender,
            FormClosedEventArgs e)
        {
            filterConfiguration.Dispose();
            Dispose();
        }

        /// <summary>
        /// Кнопка "Закрыть"
        /// </summary>
        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Событие при загрузке формы
        /// </summary>
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

        /// <summary>
        /// Кнопка "Фильтр"
        /// </summary>
        private void filterButton_Click(object sender, EventArgs e)
        {
            filterConfiguration.ShowForm();
        }

        /// <summary>
        /// Событие изменение выбранного элемента в списке сигналов
        /// альтернативного проекта
        /// </summary>
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

        /// <summary>
        /// Событие изменения выбранного элемента в списке сигналов
        /// текущего проекта
        /// </summary>
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

            var info = new string[] { selectedItem, oppositeItem };
            var item = new ListViewItem(info, itemGroup);
            bindedSignalsList.Items.Add(item);
            ClearAllListViewsSelection();

            //TODO: Обновить модель
        }

        /// <summary>
        /// Нажатие кнопок при активном списке со связью сигналов
        /// </summary>
        private void bindedSignalsList_KeyDown(object sender, KeyEventArgs e)
        {
            bool endEdit = e.KeyCode == Keys.Escape ||
                e.KeyCode == Keys.Enter;
            if (endEdit)
            {
                ClearAllListViewsSelection();
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
                    ClearAllListViewsSelection();
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

            //TODO: Обновить модель
        }

        /// <summary>
        /// Очищение выделение в списках.
        /// </summary>
        private void ClearAllListViewsSelection()
        {
            bindedSignalsList.SelectedIndices.Clear();
            currentProjSignalsList.SelectedIndices.Clear();
            advancedProjSignalsList.SelectedIndices.Clear();
        }

        /// <summary>
        /// Клик мышью на списке связанных сигналов
        /// </summary>
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

        /// <summary>
        /// Нажатие (не клик т.к это 1 щелк короткий) мышью на списке
        /// связанных сигналов
        /// </summary>
        private void bindedSignalsList_MouseDown(object sender, 
            MouseEventArgs e)
        {
            ListViewItem item = bindedSignalsList.GetItemAt(e.X, e.Y);
            if (item == null)
            {
                ClearAllListViewsSelection();
            }
        }

        /// <summary>
        /// Изменение выбранного элемента в списке связанных сигналов
        /// </summary>
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
                    "элементов в списках.";
                ShowErrorMessage(message);
            }
        }

        /// <summary>
        /// Нажатие кнопок клавиатуры в списке сигналов текущего проекта
        /// </summary>
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

        /// <summary>
        /// Нажатие кнопок клавиатуры в списке сигналов альтернативного проекта
        /// </summary>
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

        /// <summary>
        /// Событие изменения текста в поисковой строке по сигналам текущего
        /// проекта
        /// </summary>
        private void currProjSearchBox_TextChanged(object sender, EventArgs e)
        {
            var filteredThroughType = filterConfiguration.FilterOut(
                currProjItems, FilterConfiguration.FilterList.Current);
            SearchSubstringInListView(currentProjSignalsList,
                currProjSearchBox.Text, filteredThroughType);
        }

        /// <summary>
        /// Событие изменения текста в поисковой строке по сигналам
        /// альтернативного проекта
        /// </summary>
        private void advProjSearchBox_TextChanged(object sender, EventArgs e)
        {
            var filteredThroughType = filterConfiguration.FilterOut(
                advProjItems, FilterConfiguration.FilterList.Advanced);
            SearchSubstringInListView(advancedProjSignalsList,
                advProjSearchBox.Text, filteredThroughType);
        }

        /// <summary>
        /// Поиск подстроки в ListView (поиск совпадений и их отображение)
        /// </summary>
        /// <param name="listView">ListView</param>
        /// <param name="subString">Подстрока</param>
        /// <param name="projItems">Массив элементов в проекте</param>
        private void SearchSubstringInListView(ListView listView,
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

        /// <summary>
        /// Кнопка "Удалить" (-)
        /// </summary>
        private void delAdvProjButton_Click(object sender, EventArgs e)
        {
            bool canDelete = advProjNameComboBox.Items.Count > 0 &&
                advProjNameComboBox.SelectedIndex > -1;
            if (canDelete)
            {
                string projName = advProjNameComboBox.Text;
                string message = $"Удалить обмен с проектом \"{projName}\".";
                DialogResult delete = ShowWarningMessage(message, 
                    MessageBoxButtons.YesNo);
                if (delete == DialogResult.No)
                {
                    return;
                }

                try
                {
                    interprojectExchange.MarkToDelete(projName);
                }
                catch(Exception except)
                {
                    ShowErrorMessage(except.Message);
                }

                int selectedIndex = advProjNameComboBox.SelectedIndex;
                advProjNameComboBox.Items.Remove(projName);
                if(advProjNameComboBox.Items.Count > 0)
                {
                    advProjNameComboBox.SelectedIndex = selectedIndex - 1;
                }
                else
                {
                    advancedProjSignalsList.Clear();
                    bindedSignalsList.Items.Clear();
                    advProjNameComboBox.SelectedIndex = -1;
                }
            }
        }

        /// <summary>
        /// Кнопка "Добавить" (+)
        /// </summary>
        private void addAdvProjButton_Click(object sender, EventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.SelectedPath = interprojectExchange
                .GetPathWithProjects;
            DialogResult result = folderBrowserDialog.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            string selectedPath = folderBrowserDialog.SelectedPath;
            bool correctedPath = interprojectExchange
                .CheckPathToProjectFiles(selectedPath);
            if (correctedPath)
            {
                var dirInfo = new DirectoryInfo(selectedPath);
                if (advProjNameComboBox.Items.Contains(dirInfo.Name))
                {
                    string message = $"Проект \"{dirInfo.Name}\" уже " +
                        $"обменивается с этим проектом сигналами";
                    ShowInfoMessage(message);
                }
                else
                {
                    bool loaded = interprojectExchange.LoadProjectData(
                        selectedPath);
                    if (loaded)
                    {
                        advProjNameComboBox.Items.Add(dirInfo.Name);
                        int selectItem = advProjNameComboBox.Items
                            .IndexOf(dirInfo.Name);
                        advProjNameComboBox.SelectedIndex = selectItem;
                    }
                    else
                    {
                        string message = $"Данные по проекту " +
                            $"\"{dirInfo.Name}\" не загружены.";
                        ShowErrorMessage(message);
                    }
                }
            }
            else
            {
                string message = $"По указанному пути не найдены файлы " +
                    $"проекта.";
                ShowWarningMessage(message, MessageBoxButtons.OK);
            }
        }

        /// <summary>
        /// Событие изменение текста в списке с именами загруженных
        /// проектов
        /// </summary>
        private void advProjNameComboBox_TextChanged(object sender, EventArgs e)
        {
            int selectedIndex = advProjNameComboBox.SelectedIndex;
            if (selectedIndex >= 0)
            {
                LoadAdvProjData(advProjNameComboBox.Text);
            }
        }

        /// <summary>
        /// Загрузка данных по проекту в форму
        /// </summary>
        /// <param name="projName">Имя проекта</param>
        private void LoadAdvProjData(string projName)
        {
            advProjItems.Clear();
            var devices = interprojectExchange.GetProjectDevices(projName);
            foreach (var devInfo in devices)
            {
                var info = new string[] { devInfo.Name, devInfo.Description };
                var item = new ListViewItem(info);
                item.Tag = devInfo.Type;
                advProjItems.Add(item);
            }

            //TODO: Отобразить связи в списке как-то

            RefilterListViews();
        }

        /// <summary>
        /// Показать окно с сообщением об ошибке
        /// </summary>
        /// <param name="message">Сообщение</param>
        public DialogResult ShowErrorMessage(string message)
        {
            return MessageBox.Show(message, "Ошибка", MessageBoxButtons.OK, 
                MessageBoxIcon.Error);
        }

        /// <summary>
        /// Показать окно с предупреждением
        /// </summary>
        public DialogResult ShowWarningMessage(string message, 
            MessageBoxButtons boxButtons)
        {
            return MessageBox.Show(message, "Внимание", boxButtons, 
                MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Показать окно с информацией.
        /// </summary>
        /// <param name="message">Сообщение</param>
        public DialogResult ShowInfoMessage(string message)
        {
            return MessageBox.Show(message, "Информация", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}
