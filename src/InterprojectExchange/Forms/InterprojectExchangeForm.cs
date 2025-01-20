using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.ListView;

namespace InterprojectExchange
{
    /// <summary>
    /// Форма межпроектного обмена сигналами
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class InterprojectExchangeForm : Form
    {
        public InterprojectExchangeForm()
        {
            InitializeComponent();

            // Получение основных экземпляров классов, подписка на событие
            // обновления списков через фильтр
            interprojectExchange = InterprojectExchange.GetInstance();

            FilterConfiguration.ResetFilter();
            filterConfiguration = FilterConfiguration.GetInstance();
            filterConfiguration.SignalsFilterChanged += RefilterListViews;

            // Инициализация начальных списков
            currProjItems = new List<ListViewItem>();
            advProjItems = new List<ListViewItem>();

            // Установлен первый элемент в списке "Источник >> Приемник"
            modeComboBox.SelectedValueChanged -= 
                modeComboBox_SelectedValueChanged;
            modeComboBox.SelectedIndex = 0;
            interprojectExchange.ChangeEditMode(modeComboBox.SelectedIndex);
            modeComboBox.SelectedValueChanged += 
                modeComboBox_SelectedValueChanged;
        }

        private FilterConfiguration filterConfiguration;
        private IInterprojectExchange interprojectExchange;

        private List<ListViewItem> currProjItems;
        private List<ListViewItem> advProjItems;

        /// <summary>
        /// Индекс предыдущего выбранного проекта в <see cref="advProjNameComboBox"/>
        /// </summary>
        private int prevSelectedIndex = 0;

        /// <summary>
        /// Шрифт для <see cref="advProjNameComboBox"/>
        /// </summary>
        private static readonly Font advCmbBxFont = new Font("Arial", 8, FontStyle.Regular);

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
        [ExcludeFromCodeCoverage]
        private void InterprojectExchangeForm_Load(object sender, EventArgs e)
        {
            // Установка имени текущего проекта
            string currentProjectName = interprojectExchange.MainProjectName;
            currProjNameTextBox.Text = currentProjectName;

            // Заполнение названий моделей в списке
            advProjNameComboBox.Items.Add("");
            advProjNameComboBox.Items.AddRange(interprojectExchange
                .LoadedAdvancedModelNames);

            LoadCurrentProjectDevices();
            CheckBindingSignals();

            var frstLoadedPrj = interprojectExchange.LoadedAdvancedModelNames
                .FirstOrDefault(m => interprojectExchange.GetModel(m).Loaded);
            if (advProjNameComboBox.Items.Count > 0)
            {
                advProjNameComboBox.SelectedIndex = string.IsNullOrEmpty(frstLoadedPrj)? 
                    0 : advProjNameComboBox.Items.IndexOf(frstLoadedPrj);
            }

            interprojectExchange.MainModel.SelectedAdvancedProject =
                interprojectExchange.SelectedModel?.ProjectName;
            ReloadListViewWithSignals();
        }

        /// <summary>
        /// Загрузка устройств текущего проекта.
        /// </summary>
        private void LoadCurrentProjectDevices()
        {
            List<DeviceInfo> currentProjectDevices = interprojectExchange
                .GetModel(currProjNameTextBox.Text).Devices;
            foreach (var devInfo in currentProjectDevices)
            {
                var dev = new string[] { devInfo.Description, devInfo.Name };
                var item = new ListViewItem(dev);
                item.Tag = devInfo.Type;
                currProjItems.Add(item);
            }

            bool hardRefilter = true;
            RefilterListViews(hardRefilter);
        }

        /// <summary>
        /// Проверка соответсвия количества связанных сигналов
        /// </summary>
        /// <returns>
        /// true - есть ошибка
        /// false - ошибки нет
        /// </returns>
        private void CheckBindingSignals()
        {
            string err = interprojectExchange.CheckBindingSignals();

            if (string.IsNullOrEmpty(err)) 
                return;

            ShowErrorMessage($"Несоответствие количества каналов:\n{err}");
        }

        /// <summary>
        /// Обновить списки под фильтр
        /// </summary>
        /// <param name="selectedCurrProjDev">Выбранное устройство в основном
        /// проекте</param>
        /// <param name="selectedAdvProjDev">Выбранное устройства в
        /// альтернативном проекте</param>
        /// <param name="hardRefilter">Принудительная фильтрация</param>
        private void RefilterListViews(bool hardRefilter = false,
            string selectedCurrProjDev = null,
            string selectedAdvProjDev = null)
        {
            void Refilter()
            {
                advProjSearchBox_TextChanged(this, new EventArgs());
                currProjSearchBox_TextChanged(this, new EventArgs());

                bool useGroupsFilter = filterConfiguration.UseDeviceGroups;
                bindedSignalsList.ShowGroups = useGroupsFilter;
            }

            if (hardRefilter)
            {
                Refilter();
                return;
            }

            bool addSignals = selectedCurrProjDev != null &&
                selectedAdvProjDev != null;
            if (addSignals)
            {
                if (filterConfiguration.HideBindedSignals)
                {
                    currentProjSignalsList
                        .FindItemWithText(selectedCurrProjDev)?.Remove();
                    advancedProjSignalsList
                        .FindItemWithText(selectedAdvProjDev)?.Remove();
                }
            }
            else
            {
                if (filterConfiguration.HideBindedSignals)
                {
                    Refilter();
                }
            }
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
            string advProjDev = e.Item.SubItems[0].Text;
            SelectedListViewItemCollection currProjDevs =
                currentProjSignalsList.SelectedItems;

            bool needChange = (advProjDev != null &&
                currProjDevs.Count != 0 && e.IsSelected);
            if (!needChange)
            {
                return;
            }

            bool needAddNewElement = bindedSignalsList.SelectedItems.Count == 0;
            if (needAddNewElement)
            {
                string currProjDev = currProjDevs[0].SubItems[1].Text;
                string currProjDevType = currProjDevs[0].Tag.ToString();
                string advProjDevType = e.Item.Tag.ToString();
                AddToBindedSignals(currProjDevType, currProjDev, advProjDevType,
                    advProjDev);
            }
            else
            {
                ListViewItem selectedRow = bindedSignalsList.SelectedItems[0];
                bool notIgnoreEdit = !filterConfiguration.HideBindedSignals;
                if (selectedRow != null && notIgnoreEdit)
                {
                    bool mainProject = false;
                    string groupName = selectedRow.Group.Name;
                    bool success = interprojectExchange.UpdateProjectBinding(
                        groupName, selectedRow.SubItems[1].Text, advProjDev,
                        mainProject, out bool needSwap);
                    if (success)
                    {
                        ReplaceSignal(needSwap, advProjDev, selectedRow, 1);
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
            string currProjDev = e.Item.SubItems[1].Text;
            SelectedListViewItemCollection advProjDevs =
                advancedProjSignalsList.SelectedItems;

            bool needChange = (currProjDev != null &&
                advProjDevs.Count != 0 && e.IsSelected);
            if (!needChange)
            {
                return;
            }

            bool needAddNewElement = bindedSignalsList.SelectedItems.Count == 0;
            if (needAddNewElement)
            {
                string currProjDevType = e.Item.Tag.ToString();
                string advProjDev = advProjDevs[0].SubItems[0].Text;
                string advProjDevType = advProjDevs[0].Tag.ToString();
                AddToBindedSignals(currProjDevType, currProjDev, advProjDevType,
                    advProjDev);
            }
            else
            {
                var selectedRow = bindedSignalsList.SelectedItems[0];
                bool notIgnoreEdit = !filterConfiguration.HideBindedSignals;
                if (selectedRow != null && notIgnoreEdit)
                {
                    bool mainProject = true;
                    string groupName = selectedRow.Group.Name;
                    bool success = interprojectExchange.UpdateProjectBinding(
                        groupName, selectedRow.SubItems[0].Text, currProjDev,
                        mainProject, out bool needSwap);
                    if(success)
                    {
                        ReplaceSignal(needSwap, currProjDev, selectedRow, 0);
                    }
                    else
                    {
                        ShowErrorMessage("Ошибка изменения связи");
                    }
                }
            }
        }
        
        /// <summary>
        /// Замена сигнала при редактировании пар сигналов в графическом
        /// отображении
        /// </summary>
        /// <param name="needSwap">Надо поменять местами со старым</param>
        /// <param name="newDev">Новый сигнал</param>
        /// <param name="selectedRow">Нажатая строка в списке пар</param>
        /// <param name="subItemIndex">Индекс подэлемента для замены</param>
        private void ReplaceSignal(bool needSwap, string newDev, 
            ListViewItem selectedRow, int subItemIndex)
        {
            if (needSwap)
            {
                foreach (ListViewItem item in bindedSignalsList.Items)
                {
                    if (item.SubItems[subItemIndex].Text == newDev)
                    {
                        item.SubItems[subItemIndex].Text = selectedRow
                            .SubItems[subItemIndex].Text;
                    }
                }
            }

            selectedRow.SubItems[subItemIndex].Text = newDev;
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
            bool ignoreEqualSignalGroups = filterConfiguration
                .DisableCheckSignalsPairs;
            // Если сигналы равны и содержатся в списке сигналов (AI, AO, DI,DO)
            bool devicesInvalid = 
                (currentProjectDeviceType == advancedProjectDeviceType &&
                interprojectExchange.DeviceChannelsNames
                .Contains(currentProjectDeviceType) &&
                ignoreEqualSignalGroups == false);
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

                        bool hardRefilter = false;
                        RefilterListViews(hardRefilter, currentProjectDevice,
                            advancedProjectDevice);
                    }
                    else
                    {
                        string message = "Ошибка связки сигналов. " +
                            "Попытка связать уже связанный(-е) сигнал(-ы).";
                        ShowErrorMessage(message);
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

            if (filterConfiguration.DisableCheckSignalsPairs)
            {
                bool allowedToBind =
                    (currProjDevType == "AI" && advProjDevType == "AI") ||
                    (currProjDevType == "AO" && advProjDevType == "AO") ||
                    (currProjDevType == "DI" && advProjDevType == "DI") ||
                    (currProjDevType == "DO" && advProjDevType == "DO");
                if(allowedToBind)
                {
                    return currProjDevType;
                }
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
            SelectedListViewItemCollection selectedItems =
                    bindedSignalsList.SelectedItems;
            ListViewItem selectedItem;
            if (selectedItems?.Count > 0)
            {
                selectedItem = selectedItems[0];
            }
            else
            {
                return;
            }

            bool endEdit = e.KeyCode == Keys.Escape ||
                e.KeyCode == Keys.Enter;
            if (endEdit)
            {
                ClearAllListViewsSelection();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Delete)
            {
                DeleteItemFromBindedSignals(selectedItem);

                if (bindedSignalsList.Items.Count == 0)
                {
                    ClearAllListViewsSelection();
                    e.Handled = true;
                }
            }
            else if (e.KeyCode == Keys.Up && e.Shift)
            {
                MoveInGroup(selectedItem, MoveDirection.UP);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Down && e.Shift)
            {
                MoveInGroup(selectedItem, MoveDirection.DOWN);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Двигать элемент в группе
        /// </summary>
        /// <param name="item">Элемент</param>
        /// <param name="direction">Направление</param>
        public void MoveInGroup(ListViewItem item, MoveDirection direction)
        {
            string currProjSignal = item.SubItems[0].Text;
            string advProjSignal = item.SubItems[1].Text;
            string signalType = item.Group.Name;

            bool successMove = interprojectExchange.MoveSignalsBind(signalType,
                currProjSignal, advProjSignal, (int)direction);
            if (successMove)
            {
                ListViewGroup group = item.Group;
                int itemIndex = group.Items.IndexOf(item);
                SwapListViewItems(itemIndex, itemIndex + (int)direction,
                    group.Items);
            }
        }

        /// <summary>
        /// Поменять местами объекты ListViewItem.
        /// </summary>
        /// <param name="oldId">Старый индекс</param>
        /// <param name="newId">Новый индекс</param>
        /// <param name="items">Коллекция объектов</param>
        public void SwapListViewItems(int oldId, int newId, 
            ListViewItemCollection items)
        {
            string cache;
            for (int i = 0; i < items[oldId].SubItems.Count; i++)
            {
                cache = items[newId].SubItems[i].Text;
                items[newId].SubItems[i].Text =
                  items[oldId].SubItems[i].Text;
                items[oldId].SubItems[i].Text = cache;
            }
            items[newId].Selected = true;
        }

        public enum MoveDirection
        {
            UP = -1,
            DOWN = 1,
        }

        /// <summary>
        /// Удаление элемента из списка связанных сигналов
        /// </summary>
        /// <param name="selectedItem">Выбранный элемент в списке</param>
        private void DeleteItemFromBindedSignals(ListViewItem selectedItem)
        {
            int selectedItemIndex = selectedItem.Index;
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
                    RefilterListViews();

                    if (bindedSignalsList.Items.Count > selectedItemIndex)
                    {
                        var newSelectedItem = bindedSignalsList
                            .Items[selectedItemIndex];
                        newSelectedItem.Selected = true;
                    }
                    else if (bindedSignalsList.Items.Count == selectedItemIndex)
                    {
                        // Выбираем индекс, который выше в списке
                        var newSelectedItem = bindedSignalsList
                            .Items[selectedItemIndex - 1];
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
                ListViewItem selectedItem = bindedSignalsList.SelectedItems[0];
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
                currentProjSignalsList.ItemSelectionChanged -=
                    currentProjSignalsList_ItemSelectionChanged;
                advancedProjSignalsList.ItemSelectionChanged -=
                    advancedProjSignalsList_ItemSelectionChanged;

                string currProjDevText = selectedItem.SubItems[0].Text;
                string advProjDevText = selectedItem.SubItems[1].Text;

                ListViewItem currProjItem = currentProjSignalsList
                    .FindItemWithText(currProjDevText);
                ListViewItem advProjItem = advancedProjSignalsList
                    .FindItemWithText(advProjDevText);
                if (currProjItem != null && advProjItem != null)
                {
                    currProjItem.Selected = true;
                    currProjItem.EnsureVisible();
                    advProjItem.Selected = true;
                    advProjItem.EnsureVisible();
                }
                else
                {
                    currentProjSignalsList.SelectedIndices.Clear();
                    advancedProjSignalsList.SelectedIndices.Clear();
                }

                currentProjSignalsList.ItemSelectionChanged +=
                    currentProjSignalsList_ItemSelectionChanged;
                advancedProjSignalsList.ItemSelectionChanged +=
                    advancedProjSignalsList_ItemSelectionChanged;
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
            currentProjSignalsList.ItemSelectionChanged -=
                currentProjSignalsList_ItemSelectionChanged;

            ListViewItem[] filteredThroughType = filterConfiguration.FilterOut(
                currProjItems, FilterConfiguration.FilterList.CurrentProject);
            SearchSubstringInListView(currentProjSignalsList,
                currProjSearchBox.Text, filteredThroughType);

            currentProjSignalsList.ItemSelectionChanged +=
                currentProjSignalsList_ItemSelectionChanged;
        }

        /// <summary>
        /// Событие изменения текста в поисковой строке по сигналам
        /// альтернативного проекта
        /// </summary>
        private void advProjSearchBox_TextChanged(object sender, EventArgs e)
        {
            advancedProjSignalsList.ItemSelectionChanged -=
                advancedProjSignalsList_ItemSelectionChanged;

            ListViewItem[] filteredThroughType = filterConfiguration.FilterOut(
                advProjItems, FilterConfiguration.FilterList.AdvancedProject);
            SearchSubstringInListView(advancedProjSignalsList,
                advProjSearchBox.Text, filteredThroughType);

            advancedProjSignalsList.ItemSelectionChanged +=
                advancedProjSignalsList_ItemSelectionChanged;
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
                string lowerSubString = subString.ToLower();
                ListViewItem[] filteredItems = projItems
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

            if (!canDelete)
                return;

            string projName = advProjNameComboBox.Text;
            string message = $"Удалить обмен с проектом \"{projName}\".";
            
            if (DialogResult.No == ShowWarningMessage(message, MessageBoxButtons.YesNo))
                return;

            try
            {
                interprojectExchange.DeleteExchangeWithProject(projName);
            }
            catch (Exception exception)
            {
                ShowErrorMessage(exception.Message);
                return;
            }

            advProjNameComboBox.Items.Remove(projName);
            prevSelectedIndex = 0;
            advProjNameComboBox.SelectedIndex = 0;
            
            bindedSignalsList.Items.Clear();
        }

        /// <summary>
        /// Кнопка "Добавить" (+)
        /// </summary>
        private void addAdvProjButton_Click(object sender, EventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.SelectedPath = interprojectExchange
                .DefaultPathWithProjects;
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
                bool alreadyExchanging =
                    advProjNameComboBox.Items.Contains(dirInfo.Name) ||
                    currProjNameTextBox.Text.Contains(dirInfo.Name);
                if (alreadyExchanging)
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
                        selectedPath, out string errors);
                        if (loaded)
                        {
                            AddAndSelectModelToList(dirInfo);
                        }
                        else
                        {
                            string message = $"Данные по проекту " +
                                $"\"{dirInfo.Name}\" не загружены.\n";
                            if (!string.IsNullOrEmpty(errors))
                            {
                                message += "Дополнительные ошибки:\n" + errors;
                            }

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
            int selectItem = advProjNameComboBox.Items.IndexOf(dirInfo.Name);
            advProjNameComboBox.SelectedIndex = selectItem;
        }


        /// <summary>
        /// Событие изменение текста в списке с именами загруженных проектов
        /// </summary>
        private void advProjNameComboBox_SelectedItemChanged(object sender, 
            EventArgs e)
        {
            int selectedIndex = advProjNameComboBox.SelectedIndex;

            var model = interprojectExchange.GetModel(advProjNameComboBox.Items[selectedIndex].ToString());

            if (selectedIndex >= 0 &&
                selectedIndex != prevSelectedIndex &&
                model?.Loaded is true)
            {
                LoadAdvProjData(advProjNameComboBox.Text);
                prevSelectedIndex = selectedIndex;
            }
            else
            {
                advProjNameComboBox.SelectedIndex = prevSelectedIndex;
            }
        }

        /// <summary>
        /// Загрузка данных по проекту в форму
        /// </summary>
        /// <param name="projName">Имя проекта</param>
        private void LoadAdvProjData(string projName)
        {
            advProjItems.Clear();
            IProjectModel model = interprojectExchange.GetModel(projName);
            if (!model.Selected)
            {
                List<DeviceInfo> devices = model.Devices;
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
                bool hardRefilter = true;
                RefilterListViews(hardRefilter);
            }         
        }

        /// <summary>
        /// Перезагрузка сигналов в ListView
        /// </summary>
        private void ReloadListViewWithSignals()
        {
            if(string.IsNullOrEmpty(advProjNameComboBox.SelectedItem?.ToString()))
                return;

            bindedSignalsList.Items.Clear();

            var signals = interprojectExchange.GetBindedSignals();

            foreach (var signalType in signals.Keys)
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

        /// <summary>
        /// Изменение режима
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void modeComboBox_SelectedValueChanged(object sender, 
            EventArgs e)
        {
            interprojectExchange.ChangeEditMode(modeComboBox.SelectedIndex);
            
            bool haveProjects = advProjNameComboBox.Items.Count > 0;
            if (haveProjects)
            {
                ReloadListViewWithSignals();
                ClearAllListViewsSelection();
                currProjSearchBox_TextChanged(this, e);
                advProjSearchBox_TextChanged(this, e);
            }
        }

        /// <summary>
        /// Настройки PAC
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pacSetUpBtn_Click(object sender, EventArgs e)
        {
            var form =new PACSettingsForm(InterprojectExchange.GetInstance()
                .EditMode);
            form.ShowDialog();

            string selectedModelName = advProjNameComboBox.Text;
            if (!string.IsNullOrEmpty(selectedModelName))
            {
                IProjectModel selectedModel = interprojectExchange.GetModel(
                    selectedModelName);
                interprojectExchange.SelectModel(selectedModel);
            }
        }

        /// <summary>
        /// Сохранить изменения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveButton_Click(object sender, EventArgs e)
        {
            interprojectExchange.Save();
            Close();
        }

        /// <summary>
        /// Стиль элементов в <see cref="advProjNameComboBox"/>: <br/>
        /// Проект загружен    - обычный шрифт <br/>
        /// Проект не загружен - серый текст
        /// </summary>
        private void advProjNameComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index > advProjNameComboBox.Items.Count)
                return;

            var model = interprojectExchange.GetModel(advProjNameComboBox.Items[e.Index].ToString());

            if (model?.Loaded is false) //We are disabling item based on Index, you can have your logic here
            {
                e.Graphics.DrawString(advProjNameComboBox.Items[e.Index].ToString(), advCmbBxFont, Brushes.LightGray, e.Bounds);
            }
            else
            {
                e.DrawBackground();
                e.Graphics.DrawString(advProjNameComboBox.Items[e.Index].ToString(), advCmbBxFont, Brushes.Black, e.Bounds);
                e.DrawFocusRectangle();
            }
        }
    }
}
