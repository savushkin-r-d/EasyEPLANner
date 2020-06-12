using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyEPlanner
{
    public partial class FilterForm : Form
    {
        public FilterForm()
        {
            InitializeComponent();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void FilterForm_Load(object sender, EventArgs e)
        {
            LoadDeviceLists();
            //TODO: загрузка из конфигурации параметров фильтров.
        }

        /// <summary>
        /// Загрузка списка устройств
        /// </summary>
        private void LoadDeviceLists()
        {
            var types = Enum.GetValues(typeof(Device.DeviceType));
            foreach (var type in types)
            {
                if (type.ToString() != "NONE") 
                {
                    currProjDevList.Items.Add(type);
                    advProjDevList.Items.Add(type);
                }
            }
        }

        private void acceptButton_Click(object sender, EventArgs e)
        {
            //TODO: Применение изменений, сохранение измененных состояний,
            // фильтрация устройств
        }

        private void FilterForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //TODO: Сохранение параметров фильтров в файл.
            this.Dispose();
        }
    }
}
