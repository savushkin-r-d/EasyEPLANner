using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.ListView;

namespace InterprojectExchange
{
    /// <summary>
    /// Форма межпроектного обмена сигналами
    /// </summary>
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

            // Установлен первый элемент в списке "Источник >> Приемник"
            modeComboBox.SelectedValueChanged -= 
                modeComboBox_SelectedValueChanged;
            modeComboBox.SelectedIndex = 0;
            modeComboBox.SelectedValueChanged += 
                modeComboBox_SelectedValueChanged;
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
            // Установка имени текущего проекта
            string currentProjectName = interprojectExchange.CurrentProjectName;
            currProjNameTextBox.Text = currentProjectName;

            // Заполнение названий моделей в списке
            string[] projects = interprojectExchange.Models
                .Where(x => x.ProjectName != currentProjectName)
                .Select(x => x.ProjectName).ToArray();
            advProjNameComboBox.Items.AddRange(projects);
            if(advProjNameComboBox.Items.Count >= 0)
            {
                advProjNameComboBox.SelectedIndex = 0;
            }

            LoadCurrentProjectDevices();
        }

        /// <summary>
        /// Загрузка устройств текущего проекта.
        /// </summary>
        private void LoadCurrentProjectDevices()
        {
            var currentProjDevs = interprojectExchange
                .GetModel(currProjNameTextBox.Text).Devices;
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
            string advancedProjectDevice = e.Item.Text;
            SelectedListViewItemCollection currentProjectDevices =
                currentProjSignalsList.SelectedItems;

            bool needChange = (advancedProjectDevice != null &&
                currentProjectDevices.Count != 0 &&
                e.IsSelected);
            bool needAddNewElement = bindedSignalsList.SelectedItems.Count == 0;
            if (!needChange)
            {
                return;
            }

            if (needAddNewElement)
            {
                var currentProjectDevice = currentProjectDevices[0]
                    .SubItems[1];
                string currentProjectDeviceType = currentProjectDevices[0]
                    .Tag.ToString();
                AddToBindedSignals(currentProjectDeviceType,
                    currentProjectDevice.Text, e.Item.Tag.ToString(),
                    advancedProjectDevice);
            }
            else
            {
                var selectedRow = bindedSignalsList.SelectedItems[0];
                if (selectedRow != null)
                {
                    string groupName = selectedRow.Group.Name;
                    bool success = interprojectExchange
                        .UpdateAdvancedProjectBinding(groupName,
                        selectedRow.SubItems[1].Text,
                        advancedProjectDevice);
                    if (success)
                    {
                        selectedRow.SubItems[1].Text = advancedProjectDevice;
                    }
                    else
                    {
                        ShowErrorMessage("Ошибка изменения связи");
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
            string currentProjectDevice = e.Item.SubItems[1].Text;
            SelectedListViewItemCollection advancedProjectDevices =
                advancedProjSignalsList.SelectedItems;

            bool needChange = (currentProjectDevice != null &&
                advancedProjectDevices.Count != 0 &&
                e.IsSelected);
            if (!needChange)
            {
                return;
            }

            bool needAddNewElement = bindedSignalsList.SelectedItems
                .Count == 0;
            if (needAddNewElement)
            {
                ListViewItem advancedProjectDevice =
                    advancedProjectDevices[0];
                AddToBindedSignals(e.Item.Tag.ToString(),
                    currentProjectDevice,
                    advancedProjectDevice.Tag.ToString(),
                    advancedProjectDevice.Text);
            }
            else
            {
                var selectedRow = bindedSignalsList.SelectedItems[0];
                if (selectedRow != null)
                {
                    string groupName = selectedRow.Group.Name;
                    bool success = interprojectExchange
                        .UpdateCurrentProjectBinding(groupName, 
                        selectedRow.SubItems[0].Text, currentProjectDevice);
                    if(success)
                    {
                        selectedRow.SubItems[0].Text = currentProjectDevice;
                    }
                    else
                    {
                        ShowErrorMessage("Ошибка изменения связи");
                    }
                }
            }
        }

        /// <summary>
        /// Добавить связь между устройствами
        /// </summary>
        /// <param name="currentProjectDeviceType">Тип устройства в текущем 
        /// проекте</param>
        /// <param name="currentProjectDevice">Устройство в текущем проекте
        /// </param>
        /// <param name="advancedProjectDeviceType">Тип устройства в 
        /// альтернативном проекте</param>
        /// <param name="advancedProjectDevice">Устройство в альтернативном 
        /// проекте</param>
        private void AddToBindedSignals(string currentProjectDeviceType, 
            string currentProjectDevice, string advancedProjectDeviceType, 
            string advancedProjectDevice)
        {
            // Если сигналы равны и содержатся в списке сигналов (AI, AO, DI,DO)
            bool devicesInvalid = 
                (currentProjectDeviceType == advancedProjectDeviceType &&
                interprojectExchange.DeviceChannelsNames
                .Contains(currentProjectDeviceType));
            if (devicesInvalid)
            {
                ShowWarningMessage("Устройства имеют одинаковый тип сигнала", 
                    MessageBoxButtons.OK);
                ClearAllListViewsSelection();
                return;
            }
            else
            {
                string itemGroupName = GetItemGroupName(
                    currentProjectDeviceType, advancedProjectDeviceType);
                if (itemGroupName != null)
                {
                    var info = new string[]
                    {
                        currentProjectDevice,
                        advancedProjectDevice
                    };
                    ListViewGroup itemGroup = bindedSignalsList
                        .Groups[itemGroupName];
                    var item = new ListViewItem(info, itemGroup);

                    bool success = interprojectExchange.BindSignals(
                        itemGroup.Name, currentProjectDevice, 
                        advancedProjectDevice);
                    if (success)
                    {
                        bindedSignalsList.Items.Add(item);
                    }
                    else
                    {
                        ShowErrorMessage("Не удалось связать сигналы");
                    }

                }

                ClearAllListViewsSelection();
            }
        }

        /// <summary>
        /// Получить имя группы для добавления устройств
        /// </summary>
        /// <param name="currProjDevType">Тип устройства в текущем
        /// проекте</param>
        /// <param name="advProjDevType">Тип устройства в 
        /// альтернативном проекте</param>
        /// <returns></returns>
        private string GetItemGroupName(string currProjDevType,
            string advProjDevType)
        {
            string itemGroup;

            // Если разные сигналы (цифровой-аналоговый, наоборот)
            char currDevSignalType = currProjDevType[0];
            char advDevSignalType = advProjDevType[0];
            bool needCheckSignalTypeDiff = 
                (currDevSignalType == 'A' || currDevSignalType == 'D') &&
                (advDevSignalType == 'A' || advDevSignalType == 'D');
            if (needCheckSignalTypeDiff)
            {
                bool differentSignalsTypes = 
                    currDevSignalType != advDevSignalType;
                if (differentSignalsTypes)
                {
                    itemGroup = null;
                    ShowWarningMessage("Разные типы сигналов (попытка " +
                        "связать дискретные с аналоговыми или наоборот)",
                        MessageBoxButtons.OK);
                    return itemGroup;
                }
            }

            // Сигналы известны
            bool allTypesCorrected =
                (currProjDevType == "AI" && advProjDevType == "AO") ||
                (currProjDevType == "AO" && advProjDevType == "AI") ||
                (currProjDevType == "DI" && advProjDevType == "DO") ||
                (currProjDevType == "DO" && advProjDevType == "DI");
            if (allTypesCorrected)
            {
                itemGroup = currProjDevType;
                return itemGroup;
            }

            // Один из сигналов неизвестен
            if (interprojectExchange.DeviceChannelsNames
                .Contains(currProjDevType) &&
                !interprojectExchange.DeviceChannelsNames
                .Contains(advProjDevType))
            {
                itemGroup = currProjDevType;
                return itemGroup;
            }
            
            if (!interprojectExchange.DeviceChannelsNames
                .Contains(currProjDevType) &&
                interprojectExchange.DeviceChannelsNames
                .Contains(advProjDevType))
            {
                // Выбираем противоположную группу т.к известен сигнал с
                // альтернативного проекта, а нам нужен с текущего
                if (advProjDevType[1] == 'I')
                {
                    itemGroup = advProjDevType.Replace('I', 'O');
                }
                else
                {
                    itemGroup = advProjDevType.Replace('O', 'I');
                }
                return itemGroup;
            }

            // Оба сигнала неизвестны
            var form = new UnknownDevTypeForm();
            form.ShowDialog();
            itemGroup = form.GroupForAddingDeviceName;
            form.Close();

            return itemGroup;
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
        /// <param name="selectedItem">Выбранный элемент в списке</param>
        private void DeleteItemFromBindedSignals(ListViewItem selectedItem)
        {
            var selectedItemIndex = selectedItem.Index;

            string currentProjectDevice = selectedItem.SubItems[0].Text;
            string advancedProjectdevice = selectedItem.SubItems[1].Text;
            string signalType = selectedItem.Group.Name;
            bool success = interprojectExchange.DeleteSignalsBind(signalType,
                currentProjectDevice, advancedProjectdevice);
            if (success)
            {
                bindedSignalsList.Items.Remove(selectedItem);

                if (selectedItemIndex >= 0 && bindedSignalsList.Items.Count > 0)
                {
                    if (bindedSignalsList.Items.Count > selectedItemIndex)
                    {
                        var newSelectedItem = bindedSignalsList.Items[
                        selectedItemIndex];
                        newSelectedItem.Selected = true;
                    }
                    else if (bindedSignalsList.Items.Count == selectedItemIndex)
                    {
                        var newSelectedItem = bindedSignalsList.Items[
                                selectedItemIndex - 1];
                        newSelectedItem.Selected = true;
                    }
                }
            } 
            else
            {
                ShowErrorMessage("Ошибка удаления связи");
            }
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
                    interprojectExchange.DeleteExchangeWithProject(projName);
                }
                catch (Exception exception)
                {
                    ShowErrorMessage(exception.Message);
                    return;
                }

                int selectedIndex = advProjNameComboBox.Items.IndexOf(projName);
                advProjNameComboBox.Items.Remove(projName);
                if(advProjNameComboBox.Items.Count > 0)
                {
                    if(selectedIndex > 0) 
                    {
                        advProjNameComboBox.SelectedIndex = selectedIndex - 1;
                    }
                    else
                    {
                        advProjNameComboBox.SelectedIndex = selectedIndex;
                    }
                }
                else
                {
                    advancedProjSignalsList.Items.Clear();
                    bindedSignalsList.Items.Clear();
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
                .PathWithProjects;
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
                    bool canRestore = interprojectExchange
                        .RestoreModel(dirInfo.Name);
                    if (canRestore)
                    {
                        AddAndSelectModelToList(dirInfo);
                        return;
                    }
                    else
                    {
                        bool loaded = interprojectExchange.LoadProjectData(
                        selectedPath);
                        if (loaded)
                        {
                            AddAndSelectModelToList(dirInfo);
                        }
                        else
                        {
                            string message = $"Данные по проекту " +
                                $"\"{dirInfo.Name}\" не загружены.";
                            ShowErrorMessage(message);
                        }
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
        /// Добавить модель в список и выбрать её
        /// </summary>
        /// <param name="dirInfo">Информация о каталоге с проектом</param>
        private void AddAndSelectModelToList(DirectoryInfo dirInfo)
        {
            advProjNameComboBox.Items.Add(dirInfo.Name);
            int selectItem = advProjNameComboBox.Items
                .IndexOf(dirInfo.Name);
            advProjNameComboBox.SelectedIndex = selectItem;
        }

        /// <summary>
        /// Событие изменение текста в списке с именами загруженных
        /// проектов
        /// </summary>
        private void advProjNameComboBox_SelectedItemChanged(object sender, 
            EventArgs e)
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
            var model = interprojectExchange.GetModel(projName);
            if (!model.Selected)
            {
                var devices = model.Devices;
                foreach (var devInfo in devices)
                {
                    var info = new string[] 
                    { 
                        devInfo.Name, 
                        devInfo.Description 
                    };
                    var item = new ListViewItem(info);
                    item.Tag = devInfo.Type;
                    advProjItems.Add(item);
                }

                interprojectExchange.SelectModel(model);

                ReloadListViewWithSignals();
                RefilterListViews();
            }         
        }

        /// <summary>
        /// Перезагрузка сигналов в ListView
        /// </summary>
        private void ReloadListViewWithSignals()
        {
            bindedSignalsList.Items.Clear();
            Dictionary<string, List<string[]>> signals = interprojectExchange
                .GetBindedSignals();
            foreach(var signalType in signals.Keys)
            {
                ListViewGroup signalGroup = bindedSignalsList
                    .Groups[signalType];
                foreach(var signalPairs in signals[signalType])
                {
                    var item = new ListViewItem(signalPairs, signalGroup);
                    bindedSignalsList.Items.Add(item);
                }
            }
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

        private void modeComboBox_SelectedValueChanged(object sender, 
            EventArgs e)
        {
            interprojectExchange.ChangeEditMode(modeComboBox.SelectedIndex);
            ReloadListViewWithSignals();
            ClearAllListViewsSelection();
        }

        private void pacSetUpBtn_Click(object sender, EventArgs e)
        {
            var form = new PACSettingsForm();
            form.ShowDialog();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            interprojectExchange.Save();
            Close();
        }
    }
}