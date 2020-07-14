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
        private Dictionary<string, PacInfo> projectsSendingFromMain;
        private Dictionary<string, PacInfo> projectsSendingToMain;
        private string projectBeforeOpenForm = "";
        private EditMode editMode;

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            CurrentProjectModel mainModel = interprojectExchange.MainModel;
            mainModel.SelectedAdvancedProject = projectBeforeOpenForm;
            Close();
        }

        private void PACSettingsForm_FormClosed(object sender, 
            FormClosedEventArgs e)
        {
            Dispose();
        }

        private void acceptBtn_Click(object sender, EventArgs e)
        {
            if (projectsListView.SelectedItems.Count != 0)
            {
                string projectName = projectsListView.SelectedItems[0].Text;
                SaveIntermediateData(projectName);
            }

            WorkWithProjectsData(false);
            Close();
        }

        private void PACSettingsForm_Load(object sender, EventArgs e)
        {
            // Установка стандартного значения режима
            modeComboBox.SelectedValueChanged -= 
                modeComboBox_SelectedValueChanged;
            modeComboBox.SelectedIndex = 0;
            editMode = EditMode.MainSource;
            modeComboBox.SelectedValueChanged += 
                modeComboBox_SelectedValueChanged;

            interprojectExchange = InterprojectExchange.GetInstance();
            projectsSendingFromMain = new Dictionary<string, PacInfo>();
            projectsSendingToMain = new Dictionary<string, PacInfo>();
            WorkWithProjectsData(true);

            projectBeforeOpenForm = interprojectExchange.Models
                .Where(x => x.Selected == true)
                .Select(x => x.ProjectName)
                .FirstOrDefault();
        }

        /// <summary>
        /// Работа над данными проекта. True - загрузка, False - сохранение.
        /// </summary>
        /// <param name="loadProjectsData">Загрузка данных, или сохранение
        /// </param>
        private void WorkWithProjectsData(bool loadProjectsData)
        {
            var loadedModels = interprojectExchange.LoadedAdvancedModelNames;
            CurrentProjectModel mainModel = interprojectExchange.MainModel;
            foreach (var modelName in loadedModels)
            {
                IProjectModel model = interprojectExchange.GetModel(modelName);
                string projName = model.ProjectName;
                mainModel.SelectedAdvancedProject = modelName;
                if(loadProjectsData)
                {
                    LoadModelData(projName, model, mainModel);
                }
                else
                {
                    SaveModelData(projName, model, mainModel);
                }
                mainModel.SelectedAdvancedProject = projectBeforeOpenForm;
            }
        }

        /// <summary>
        /// Сохранить данные модели
        /// </summary>
        /// <param name="projectName">Имя проекта/модели</param>
        /// <param name="model">Модель</param>
        /// <param name="mainModel">Главная модель</param>
        private void SaveModelData(string projectName, IProjectModel model,
            IProjectModel mainModel)
        {
            PacInfo intermediateSettings = projectsSendingFromMain[projectName];
            model.PacInfo = intermediateSettings.Clone();

            intermediateSettings = projectsSendingToMain[projectName];
            mainModel.PacInfo = intermediateSettings.Clone();
        }

        /// <summary>
        /// Загрузить данные модели
        /// </summary>
        /// <param name="projectName">Имя проекта/модели</param>
        /// <param name="model">Модель</param>
        /// <param name="mainModel">Главная модель</param>
        private void LoadModelData(string projectName, IProjectModel model,
            IProjectModel mainModel)
        {
            projectsListView.Items.Add(projectName);
            projectsSendingFromMain.Add(projectName, model.PacInfo.Clone());
            projectsSendingToMain.Add(projectName, mainModel.PacInfo.Clone());
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
            PacInfo pacInfo;
            if(editMode == EditMode.MainSource)
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
                PacInfo pacInfo;
                if (editMode == EditMode.MainSource)
                {
                    pacInfo = projectsSendingFromMain[projectName];
                }
                else
                {
                    pacInfo = projectsSendingToMain[projectName];
                }

                pacInfo.IPEmulator = emulatorIPTextBox.Text;
                pacInfo.EmulationEnabled = enableEmulationBtn.Checked;
                pacInfo.CycleTime = int.Parse(cycletimeTextBox.Text);
                pacInfo.TimeOut = int.Parse(timeoutTextBox.Text);
                pacInfo.Port = int.Parse(portTextBox.Text);
                pacInfo.GateEnabled = enableGateBtn.Checked;
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
            CheckKeyPress(e);
        }

        private void portTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            CheckKeyPress(e);
        }

        private void timeoutTextBox_KeyPress(object sender, 
            KeyPressEventArgs e)
        {
            CheckKeyPress(e);
        }

        private void cycletimeTextBox_KeyPress(object sender, 
            KeyPressEventArgs e)
        {
            CheckKeyPress(e);
        }

        const char backSpace = '\b';

        private void emulatorIPTextBox_KeyPress(object sender, 
            KeyPressEventArgs e)
        {
            bool allowedKeys = (e.KeyChar == '.' ||
                char.IsDigit(e.KeyChar) ||
                e.KeyChar == backSpace);
            if (!allowedKeys)
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Проверка вводимых символов в поля для настройки PAC
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool CheckKeyPress(KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == backSpace))
            {
                e.Handled = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Режим редактирования главного проекта
        /// </summary>
        public enum EditMode
        {
            MainSource,
            MainReceiver,
        }

        /// <summary>
        /// Изменить режим редактирования
        /// </summary>
        public void ChangeEditMode()
        {
            if(editMode == EditMode.MainSource)
            {
                editMode = EditMode.MainReceiver;
            }
            else
            {
                editMode = EditMode.MainSource;
            }
        }

        private void modeComboBox_SelectedValueChanged(object sender, 
            EventArgs e)
        {
            if(projectsListView.SelectedItems.Count != 0 )
            {
                string project = projectsListView.SelectedItems[0].Text;
                SaveIntermediateData(project);
                ChangeEditMode();
                LoadProjectDataToFields(project);
            }
            else
            {
                ChangeEditMode();
            }
        }
    }
}
