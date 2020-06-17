using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EasyEPlanner
{
    public partial class FilterForm : Form
    {
        public FilterForm()
        {
            InitializeComponent();
            var devices = FilterConfiguration.GetInstance().GetDevicesList();
            LoadDeviceLists(devices);
            SetUpFilterCheckBoxes(FilterConfiguration.GetInstance()
                .FilterParameters);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Hide();
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

        private void acceptButton_Click(object sender, EventArgs e)
        {
            FilterConfiguration.GetInstance().Save();
            Hide();
        }

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
        }

        /// <summary>
        /// Установка параметров значений CheckBox
        /// </summary>
        /// <param name="filterParameters">Параметры фильтра</param>
        private void SetUpFilterCheckBoxes(
            Dictionary<string, Dictionary<string,bool>> filterParameters)
        {
            if (filterParameters.Count == 0)
            {
                return;
            }

            // Отключили обработчики изменений состояний чекбоксов.
            currProjDevList.ItemCheck -= currProjDevList_ItemCheck;
            advProjDevList.ItemCheck -= advProjDevList_ItemCheck;
            groupAsPairsCheckBox.CheckStateChanged -= 
                groupAsPairsCheckBox_CheckStateChanged;

            foreach (var devPair in filterParameters[currProjDevList.Name])
            {
                int itemNum = currProjDevList.FindStringExact(devPair.Key);
                currProjDevList.SetItemChecked(itemNum, devPair.Value);
            }

            foreach (var devPair in filterParameters[advProjDevList.Name])
            {
                int itemNum = advProjDevList.FindStringExact(devPair.Key);
                advProjDevList.SetItemChecked(itemNum, devPair.Value);
            }

            bool isChecked = filterParameters[bindedGridGroupBox.Name]
                [groupAsPairsCheckBox.Name];
            groupAsPairsCheckBox.Checked = isChecked;

            // Включили обработчики изменений состояний чекбоксов
            currProjDevList.ItemCheck += currProjDevList_ItemCheck;
            advProjDevList.ItemCheck += advProjDevList_ItemCheck;
            groupAsPairsCheckBox.CheckStateChanged +=
                groupAsPairsCheckBox_CheckStateChanged;
        }

        private void groupAsPairsCheckBox_CheckStateChanged(object sender, 
            EventArgs e)
        {
            FilterConfiguration.GetInstance()
                .FilterParameters[bindedGridGroupBox.Name]
                [groupAsPairsCheckBox.Name] = groupAsPairsCheckBox.Checked;
        }

        private void currProjDevList_ItemCheck(object sender, 
            ItemCheckEventArgs e)
        {
            var itemName = currProjDevList.Items[e.Index].ToString();
            bool isChecked = e.NewValue == CheckState.Checked ? true : false;
            FilterConfiguration.GetInstance()
                .FilterParameters[currProjDevList.Name][itemName] = isChecked;
        }

        private void advProjDevList_ItemCheck(object sender, 
            ItemCheckEventArgs e)
        {
            var itemName = currProjDevList.Items[e.Index].ToString();
            bool isChecked = e.NewValue == CheckState.Checked ? true : false;
            FilterConfiguration.GetInstance()
                .FilterParameters[advProjDevList.Name][itemName] = isChecked;
        }
    }
}
