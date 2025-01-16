using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Forms;

namespace InterprojectExchange
{
    public partial class PACSettingsForm : Form
    {
        public PACSettingsForm(EditMode editMode)
        {
            this.editMode = editMode;
            InitializeComponent();
        }

        /// <summary>
        /// Межконтроллерный обмен
        /// </summary>
        private IInterprojectExchange interprojectExchange;

        /// <summary>
        /// PAC-инфо о проектах, сигналы которым отправляются из главного
        /// </summary>
        private Dictionary<string, PacInfo> projectsSendingFromMain;

        /// <summary>
        /// PAC-инфо о проектах, сигналы которые отправляются в главный
        /// </summary>
        private Dictionary<string, PacInfo> projectsSendingToMain;

        /// <summary>
        /// Имя проекта до открытия формы
        /// </summary>
        private string projectBeforeOpenForm = "";

        /// <summary>
        /// Режим редактирования PAC-инфо
        /// </summary>
        private EditMode editMode;

        /// <summary>
        /// Главная модель проекта.
        /// </summary>
        private ICurrentProjectModel mainModel;

        /// <summary>
        /// Пустая ли таблица сигналов, которые отправляются из главного
        /// проекта, разделенная по именам проектов.
        /// </summary>
        private Dictionary<string, bool> signalsSendingFromMainEmpty;

        /// <summary>
        /// Пустая ли таблица сигналов, которые присылаются в главный проект,
        /// разделенная по именам проектов.
        /// </summary>
        private Dictionary<string, bool> signalsSendingToMainEmpty;

        private void cancelBtn_Click(object sender, EventArgs e)
        {
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

            bool validStations = CheckPACStationNumbers();
            if (validStations)
            {
                WorkWithProjectsData(false);
                Close();
            }
            else
            {
                MessageBox.Show("Проверьте номера станций главного объекта " +
                    "в режиме \"Источник\", дублирование номеров.",
                    "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Проверка номеров станции на коллизию.
        /// </summary>
        /// <returns></returns>
        public bool CheckPACStationNumbers()
        {
            var checkList = new List<int>();
            foreach (var pacInfo in projectsSendingFromMain)
            {
                int station = pacInfo.Value.Station; 
                if (station > 0)
                {
                    if (checkList.Contains(station))
                    {
                        return false;
                    }
                    else
                    {
                        checkList.Add(station);
                    }
                }
            }

            return true;
        }

        private void PACSettingsForm_Load(object sender, EventArgs e)
        {
            interprojectExchange = InterprojectExchange.GetInstance();
            mainModel = interprojectExchange.MainModel;
            projectsSendingFromMain = new Dictionary<string, PacInfo>();
            projectsSendingToMain = new Dictionary<string, PacInfo>();
            signalsSendingFromMainEmpty = new Dictionary<string, bool>();
            signalsSendingToMainEmpty = new Dictionary<string, bool>();
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
            signalsSendingFromMainEmpty.Add(projectName,
                mainModel.SourceSignals.Count == 0);
            signalsSendingToMainEmpty.Add(projectName,
                mainModel.ReceiverSignals.Count == 0);
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
                ClearDataFromForm();
            }
        }

        private void ClearDataFromForm()
        {
            projNameTextBox.Text = string.Empty;
            ipAddressTextBox.Text = string.Empty;
            emulatorIPTextBox.Text = string.Empty;
            enableEmulationBtn.Checked = false;
            disableEmulationBtn.Checked = false;
            cycletimeTextBox.Text = string.Empty;
            timeoutTextBox.Text = string.Empty;
            portTextBox.Text = string.Empty;
            enableGateBtn.Checked = false;
            disableGateBtn.Checked = false;
            stationNumberTextBox.Text = string.Empty;
        }

        /// <summary>
        /// Загрузка данных проекта в поля
        /// </summary>
        /// <param name="projectName">Имя проекта</param>
        [ExcludeFromCodeCoverage]
        private void LoadProjectDataToFields(string projectName)
        {
            PacInfo pacInfo;
            if(editMode == EditMode.SourceReciever)
            {
                pacInfo = projectsSendingFromMain[projectName];
                stationNumberTextBox.Enabled =
                    !signalsSendingFromMainEmpty[projectName];
            }
            else
            {
                pacInfo = projectsSendingToMain[projectName];
                stationNumberTextBox.Enabled =
                    !signalsSendingToMainEmpty[projectName];
            }

            // Настройки проекта, отключаемые для изменения, если модель не загружена
            projNameTextBox.Enabled = pacInfo.ModelLoaded;
            ipAddressTextBox.Enabled = pacInfo.ModelLoaded;
            portTextBox.Enabled = pacInfo.ModelLoaded;
            stationNumberTextBox.Enabled = pacInfo.ModelLoaded;

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
                if (editMode == EditMode.SourceReciever)
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
    }
}
