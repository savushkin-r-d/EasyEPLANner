using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace InterprojectExchange
{
    /// <summary>
    /// Форма настройки фильтра
    /// </summary>
    public partial class FilterForm : Form
    {
        public FilterForm()
        {
            InitializeComponent();
            filterConfiguration = FilterConfiguration.GetInstance();
            var devices = filterConfiguration.GetDevicesList();
            LoadDeviceLists(devices);
            SetUpFilterCheckBoxes(filterConfiguration);
        }

        private FilterConfiguration filterConfiguration;

        /// <summary>
        /// Кнопка "Отмена"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Загрузка списка устройств
        /// </summary>
        /// <param name="devices">Устройства для отображения</param>
        private void LoadDeviceLists(List<string> devices)
        {
            foreach (var type in devices)
            {
                currProjDevList.Items.Add(type);
                advProjDevList.Items.Add(type);
            }
        }

        /// <summary>
        /// Кнопка "Применить"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void acceptButton_Click(object sender, EventArgs e)
        {
            FilterConfiguration.GetInstance().Save();
            Hide();
        }

        /// <summary>
        /// Кнопка "Очистить"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearButton_Click(object sender, EventArgs e)
        {
            for(int i = 0; i < currProjDevList.Items.Count; i++)
            {
                currProjDevList.SetItemCheckState(i, CheckState.Unchecked);
            }

            for(int i = 0; i < advProjDevList.Items.Count; i++)
            {
                advProjDevList.SetItemCheckState(i, CheckState.Unchecked);
            }

            groupAsPairsCheckBox.Checked = false;
            hideBindedSignalsCheckBox.Checked = false;
            disableCheckSignalsPairsCheckBox.Checked = false;
        }

        /// <summary>
        /// Установка параметров значений CheckBox
        /// </summary>
        /// <param name="filterParameters">Параметры фильтра</param>
        private void SetUpFilterCheckBoxes(FilterConfiguration 
            filterConfiguration)
        {
            if (filterConfiguration.FilterParameters.Count == 0)
            {
                return;
            }

            // Отключили обработчики изменений состояний чекбоксов.
            currProjDevList.ItemCheck -= currProjDevList_ItemCheck;
            advProjDevList.ItemCheck -= advProjDevList_ItemCheck;
            groupAsPairsCheckBox.CheckStateChanged -= 
                groupAsPairsCheckBox_CheckStateChanged;
            hideBindedSignalsCheckBox.CheckStateChanged -=
                hideBindedSignalsCheckBox_CheckStateChanged;
            disableCheckSignalsPairsCheckBox.CheckStateChanged -=
                disableCheckSignalsPairsCheckBox_CheckStateChanged;

            string[] currentProjectSelectedDevices = filterConfiguration
                .CurrentProjectSelectedDevices;
            string[] advancedProjectSelectedDevices = filterConfiguration
                .AdvancedProjectSelectedDevices;
            List<string> allDevices = filterConfiguration.GetDevicesList();
            SetUpCheckedListBox(currProjDevList, currentProjectSelectedDevices, 
                allDevices);
            SetUpCheckedListBox(advProjDevList, advancedProjectSelectedDevices, 
                allDevices);

            bool isChecked = filterConfiguration.UseDeviceGroups;
            groupAsPairsCheckBox.Checked = isChecked;

            isChecked = filterConfiguration.DisableCheckSignalsPairs;
            disableCheckSignalsPairsCheckBox.Checked = isChecked;

            isChecked = filterConfiguration.HideBindedSignals;
            hideBindedSignalsCheckBox.Checked = isChecked;

            // Включили обработчики изменений состояний чекбоксов
            currProjDevList.ItemCheck += currProjDevList_ItemCheck;
            advProjDevList.ItemCheck += advProjDevList_ItemCheck;
            groupAsPairsCheckBox.CheckStateChanged +=
                groupAsPairsCheckBox_CheckStateChanged;
            disableCheckSignalsPairsCheckBox.CheckStateChanged +=
                disableCheckSignalsPairsCheckBox_CheckStateChanged;
            hideBindedSignalsCheckBox.CheckStateChanged +=
                hideBindedSignalsCheckBox_CheckStateChanged;
        }

        /// <summary>
        /// Настройка списка с CheckBox
        /// </summary>
        /// <param name="checkedListBox">Ссылка на элемент управления</param>
        /// <param name="selectedDevices">Массив выбранных устройств</param>
        private void SetUpCheckedListBox(CheckedListBox checkedListBox, 
            string[] selectedDevices, List<string> allDevices)
        {
            foreach (var item in allDevices)
            {
                int itemNum = checkedListBox.FindStringExact(item.ToString());
                if (selectedDevices.Contains(item.ToString()))
                {
                    checkedListBox.SetItemChecked(itemNum, true);
                }
                else
                {
                    checkedListBox.SetItemChecked(itemNum, false);
                }
            }
        }

        private void groupAsPairsCheckBox_CheckStateChanged(object sender, 
            EventArgs e)
        {
            filterConfiguration.SetFilterParameter(bindedSignalsList.Name,
                groupAsPairsCheckBox.Name, groupAsPairsCheckBox.Checked); 
        }

        private void currProjDevList_ItemCheck(object sender, 
            ItemCheckEventArgs e)
        {
            var itemName = currProjDevList.Items[e.Index].ToString();
            bool isChecked = e.NewValue == CheckState.Checked ? true : false;
            filterConfiguration.SetFilterParameter(currProjDevList.Name,
                itemName, isChecked);
        }

        private void advProjDevList_ItemCheck(object sender, 
            ItemCheckEventArgs e)
        {
            var itemName = currProjDevList.Items[e.Index].ToString();
            bool isChecked = e.NewValue == CheckState.Checked ? true : false;
            filterConfiguration.SetFilterParameter(advProjDevList.Name, 
                itemName, isChecked);
        }

        private void hideBindedSignalsCheckBox_CheckStateChanged(object sender,
            EventArgs e)
        {
            filterConfiguration.SetFilterParameter(bindedSignalsList.Name,
                hideBindedSignalsCheckBox.Name,
                hideBindedSignalsCheckBox.Checked);
        }

        private void disableCheckSignalsPairsCheckBox_CheckStateChanged(
            object sender, EventArgs e)
        {
            filterConfiguration.SetFilterParameter(bindedSignalsList.Name,
                disableCheckSignalsPairsCheckBox.Name,
                disableCheckSignalsPairsCheckBox.Checked);
        }
    }
}
