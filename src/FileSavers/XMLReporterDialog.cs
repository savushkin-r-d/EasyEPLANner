using System;
using System.IO;
using System.Windows.Forms;

namespace EasyEPlanner
{
    public partial class XMLReporterDialog : Form
    {
        public XMLReporterDialog()
        {
            InitializeComponent();

            path = pathTextBox.Text;
            projectName = "";
        }

        /// <summary>
        /// Установить имя проекта.
        /// </summary>
        /// <param name="name"></param>
        public void SetProjectName(string name)
        {
            this.projectName = name;
        }

        /// <summary>
        /// Кнопка "отменить".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelBut_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Событие после закрытия формы.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportChannelBaseDialog_FormClosed(object sender, 
            FormClosedEventArgs e)
        {
            this.Dispose();
        }

        /// <summary>
        /// Кнопка "Обзор".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void reviewPathBut_Click(object sender, EventArgs e)
        {
            if (oldChBaseBut.Checked)
            {
                var openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = $"Файлы базы каналов|*{chBaseFormat}";
                DialogResult openFileResult = openFileDialog.ShowDialog();

                if (openFileResult == DialogResult.OK)
                {
                    path = openFileDialog.FileName;
                }
            }

            if (newChBaseBut.Checked)
            {
                var browseFolderDialog = new FolderBrowserDialog();
                DialogResult browseDialog = browseFolderDialog.ShowDialog();

                if (browseDialog == DialogResult.OK)
                {
                    path = browseFolderDialog.SelectedPath + "\\" +
                        projectName + chBaseFormat;
                }
            }

            pathTextBox.Text = path;
        }

        private void newChBaseBut_CheckedChanged(object sender, EventArgs e)
        {
            if (newChBaseBut.Checked == true)
            {
                pathTextBoxLabel.Text = "Укажите каталог для сохранения:";
                oldChBaseBut.Checked = false;
            }
        }

        private void oldChBaseBut_CheckedChanged(object sender, EventArgs e)
        {
            if (oldChBaseBut.Checked == true)
            {
                pathTextBoxLabel.Text = "Укажите путь к базе каналов:";
                newChBaseBut.Checked = false;
            }
        }

        private void NonCombineTagBut_CheckedChanged(object sender, EventArgs e)
        {
            if (nonCombineTagBut.Checked == true)
            {
                combineTagBut.Checked = false;
            }
        }

        private void combineTagBut_CheckedChanged(object sender, EventArgs e)
        {
            if (combineTagBut.Checked == true)
            {
                nonCombineTagBut.Checked = false;
            }
        }

        private void oldFormatBut_CheckedChanged(object sender, EventArgs e)
        {
            if (oldFormatBut.Checked == true)
            {
                newFormatBut.Checked = false;
            }
        }

        private void newFormatBut_CheckedChanged(object sender, EventArgs e)
        {
            if (newFormatBut.Checked == true)
            {
                oldFormatBut.Checked = false;
            }
        }

        /// <summary>
        /// Экспортировать базу каналов.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportBut_Click(object sender, EventArgs e)
        {
            bool pathIsValid = CheckPath();
            bool combineTag = combineTagBut.Checked ? true : false;
            bool useNewNames = newFormatBut.Checked ? true : false;

            if (pathIsValid)
            {
                if (newChBaseBut.Checked == true)
                {
                    string filePath = path + "\\" + projectName + chBaseFormat;
                    bool rewrite = true;
                    ProjectManager.GetInstance().SaveAsCDBX(filePath, 
                        combineTag, useNewNames, rewrite);
                }
                else
                {
                    ProjectManager.GetInstance().SaveAsCDBX(path, combineTag,
                    useNewNames);
                }

                this.Close();
            }
        }

        /// <summary>
        /// Проверить выбранный путь.
        /// </summary>
        /// <returns></returns>
        private bool CheckPath()
        {
            if (oldChBaseBut.Checked)
            {
                bool isExist = File.Exists(path);
                if (isExist == false)
                {
                    MessageBox.Show("Не найден файл базы каналов", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                bool pathIsValid = path.Contains(chBaseFormat);
                if (pathIsValid == false)
                {
                    MessageBox.Show("Указан неправильный файл базы каналов.",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            return true;
        }

        private string path;
        private string projectName;
        private string chBaseFormat = ".cdbx";
    }
}
