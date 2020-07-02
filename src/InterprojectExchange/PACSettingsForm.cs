using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace InterprojectExchange
{
    public partial class PACSettingsForm : Form
    {
        public PACSettingsForm()
        {
            InitializeComponent();
        }

        private InterprojectExchange interprojectExchange;
        private Dictionary<string, PacDTO> projectsSendingFromMain;
        private Dictionary<string, PacDTO> projectsSendingToMain;
        private string selectedProjectBeforeOpeningThisForm = "";

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void PACSettingsForm_FormClosed(object sender, 
            FormClosedEventArgs e)
        {
            this.Dispose();
        }

        private void acceptBtn_Click(object sender, EventArgs e)
        {
            if (projectsListView.SelectedItems.Count > 0)
            {
                string projectName = projectsListView.SelectedItems[0].Text;
                SaveIntermediateData(projectName);
                SaveDataToModels();
            }

            this.Close();
        }

        /// <summary>
        /// Сохранение измененных данных
        /// </summary>
        private void SaveDataToModels()
        {
            foreach (var model in interprojectExchange.Models)
            {
                if (model.ProjectName != interprojectExchange.CurrentProjectName)
                {
                    string projectName = model.ProjectName;
                    PacDTO intermediateSettings = projectsSendingFromMain[projectName];
                    model.PacInfo = intermediateSettings.Clone();
                }
            }

            var loadedModels = interprojectExchange.Models
                .Where(x => x.ProjectName !=
                interprojectExchange.CurrentProjectName)
                .Select(x => x.ProjectName).ToArray();

            var mainModel = interprojectExchange.GetModel(
                interprojectExchange.CurrentProjectName) as CurrentProjectModel;
            foreach (string modelName in loadedModels)
            {
                mainModel.SelectedAdvancedProject = modelName;
                PacDTO intermediateSettings = projectsSendingToMain[modelName];
                mainModel.PacInfo = intermediateSettings.Clone();
                mainModel.SelectedAdvancedProject =
                    selectedProjectBeforeOpeningThisForm;
            }
        }

        private void PACSettingsForm_Load(object sender, EventArgs e)
        {
            modeComboBox.SelectedValueChanged -= 
                modeComboBox_SelectedValueChanged;
            modeComboBox.SelectedIndex = 0;
            modeComboBox.SelectedValueChanged += 
                modeComboBox_SelectedValueChanged;

            interprojectExchange = InterprojectExchange.GetInstance();
            projectsSendingFromMain = new Dictionary<string, PacDTO>();
            projectsSendingToMain = new Dictionary<string, PacDTO>();
            LoadProjectsToListView();
            selectedProjectBeforeOpeningThisForm = interprojectExchange.Models
                .Where(x => x.Selected == true)
                .Select(x => x.ProjectName)
                .FirstOrDefault();
        }

        /// <summary>
        /// Загрузить проекты в ListView
        /// </summary>
        private void LoadProjectsToListView()
        {
            var loadedModels = interprojectExchange.Models
                .Where(x => x.ProjectName != 
                interprojectExchange.CurrentProjectName)
                .Select(x => x.ProjectName).ToArray();

            foreach (var model in interprojectExchange.Models)
            {
                if(model.ProjectName != interprojectExchange.CurrentProjectName)
                {
                    string projName = model.ProjectName;
                    projectsListView.Items.Add(projName);
                    projectsSendingFromMain.Add(projName, model.PacInfo
                        .Clone());
                }
            }

            var mainModel = interprojectExchange.GetModel(
                interprojectExchange.CurrentProjectName) as CurrentProjectModel;
            foreach(string modelName in loadedModels)
            {
                mainModel.SelectedAdvancedProject = modelName;
                projectsSendingToMain.Add(modelName, mainModel.PacInfo);
                mainModel.SelectedAdvancedProject = 
                    selectedProjectBeforeOpeningThisForm;
            }
        }

        private void projectsListView_ItemSelectionChanged(object sender, 
            ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                LoadProjectDataToFields(e.Item.Text);
            }
            else
            {
                SaveIntermediateData(e.Item.Text);
            }
        }

        /// <summary>
        /// Загрузка данных проекта в поля
        /// </summary>
        /// <param name="projectName">Имя проекта</param>
        private void LoadProjectDataToFields(string projectName)
        {
            PacDTO pacInfo;
            if(EditMode == 0)
            {
                pacInfo = projectsSendingFromMain[projectName];
            }
            else
            {
                pacInfo = projectsSendingToMain[projectName];
            }

            projNameTextBox.Text = projectName;
            ipAddressTextBox.Text = pacInfo.IP;
            emulatorIPTextBox.Text = pacInfo.IPEmulator;

            if (pacInfo.EmulationEnabled)
            {
                enableEmulationBtn_Click(this, new EventArgs());
            }
            else
            {
                disableEmulationBtn_Click(this, new EventArgs());

            }

            cycletimeTextBox.Text = pacInfo.CycleTime.ToString();
            timeoutTextBox.Text = pacInfo.TimeOut.ToString();
            portTextBox.Text = pacInfo.Port.ToString();

            if (pacInfo.GateEnabled)
            {
                enableGateBtn_Click(this, new EventArgs());
            }
            else
            {
                disableGateBtn_Click(this, new EventArgs());
            }

            stationNumberTextBox.Text = pacInfo.Station.ToString();
        }

        /// <summary>
        /// Сохранение промежуточных данных
        /// </summary>
        /// <param name="projectName">Имя проекта</param>
        private void SaveIntermediateData(string projectName)
        {
            try
            {
                PacDTO pacInfo;
                if (EditMode == 0)
                {
                    pacInfo = projectsSendingFromMain[projectName];
                }
                else
                {
                    pacInfo = projectsSendingToMain[projectName];
                }

                pacInfo.IPEmulator = emulatorIPTextBox.Text;
                pacInfo.EmulationEnabled = 
                    enableEmulationBtn.Checked ? true : false;
                pacInfo.CycleTime = int.Parse(cycletimeTextBox.Text);
                pacInfo.TimeOut = int.Parse(timeoutTextBox.Text);
                pacInfo.Port = int.Parse(portTextBox.Text);
                pacInfo.GateEnabled = enableGateBtn.Checked ? true : false;
                pacInfo.Station = int.Parse(stationNumberTextBox.Text);
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения промежуточных данных",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }           
        }

        private void enableEmulationBtn_Click(object sender,
            EventArgs e)
        {
            disableEmulationBtn.Checked = false;
            enableEmulationBtn.Checked = true;
        }

        private void disableEmulationBtn_Click(object sender,
            EventArgs e)
        {
            enableEmulationBtn.Checked = false;
            disableEmulationBtn.Checked = true;
        }

        private void enableGateBtn_Click(object sender, EventArgs e)
        {
            disableGateBtn.Checked = false;
            enableGateBtn.Checked = true;
        }

        private void disableGateBtn_Click(object sender, EventArgs e)
        {
            enableGateBtn.Checked = false;
            disableGateBtn.Checked = true;
        }

        private void stationNumberTextBox_KeyPress(object sender, 
            KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == BackSpace))
            {
                e.Handled = true;
            }
        }

        private void portTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == BackSpace))
            {
                e.Handled = true;
            }
        }

        private void timeoutTextBox_KeyPress(object sender, 
            KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == BackSpace))
            {
                e.Handled = true;
            }
        }

        private void cycletimeTextBox_KeyPress(object sender, 
            KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == BackSpace))
            {
                e.Handled = true;
            }
        }

        private void emulatorIPTextBox_KeyPress(object sender, 
            KeyPressEventArgs e)
        {
            bool allowedKeys = (e.KeyChar == '.' ||
                char.IsDigit(e.KeyChar) ||
                e.KeyChar == BackSpace);
            if (!allowedKeys)
            {
                e.Handled = true;
            }
        }

        char BackSpace = '\b';

        /// <summary>
        /// Режим редактирования (0 - источник, 1 - приемник)
        /// </summary>
        public int EditMode { get; set; }

        /// <summary>
        /// Изменить режим редактирования
        /// </summary>
        public void ChangedEditMode()
        {
            if(EditMode == 0)
            {
                EditMode = 1;
            }
            else
            {
                EditMode = 0;
            }
        }

        private void modeComboBox_SelectedValueChanged(object sender, 
            EventArgs e)
        {
            if(projectsListView.SelectedItems.Count > 0 )
            {
                string project = projectsListView.SelectedItems[0].Text;
                SaveIntermediateData(project);
                ChangedEditMode();
                LoadProjectDataToFields(project);
            }
            else
            {
                ChangedEditMode();
            }
        }
    }
}
