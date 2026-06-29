using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows.Forms;

namespace EasyEPlanner
{
    [ExcludeFromCodeCoverage]
    public partial class XMLReporterDialog : Form
    {
        public XMLReporterDialog()
        {
            InitializeComponent();

            path = string.Empty;
            projectName = string.Empty;
        }

        /// <summary>
        /// Установить имя проекта.
        /// </summary>
        /// <param name="name"></param>
        public void SetProjectName(string name)
        {
            projectName = name;
            UpdatePathFromMode();
        }

        /// <summary>
        /// Установить каталог DOC проекта как путь по умолчанию.
        /// </summary>
        /// <param name="docDirectoryPath">Путь к папке DOC проекта</param>
        public void SetDefaultPath(string docDirectoryPath)
        {
            docDirectoryPath = docDirectoryPath ?? string.Empty;
            this.docDirectoryPath = docDirectoryPath;
            UpdatePathFromMode();
        }

        private void UpdatePathFromMode()
        {
            if (string.IsNullOrEmpty(docDirectoryPath))
            {
                return;
            }

            if (oldChBaseBut.Checked)
            {
                path = string.IsNullOrEmpty(projectName)
                    ? docDirectoryPath
                    : Path.Combine(docDirectoryPath, $"{projectName}{chBaseFormat}");
            }
            else
            {
                path = docDirectoryPath;
            }

            pathTextBox.Text = path;
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
                    path = browseFolderDialog.SelectedPath;
                }
            }

            pathTextBox.Text = path;
        }

        private void newChBaseBut_CheckedChanged(object sender, EventArgs e)
        {
            if (newChBaseBut.Checked)
            {
                pathTextBoxLabel.Text = "Укажите каталог для сохранения:";
                oldChBaseBut.Checked = false;
                UpdatePathFromMode();
            }
        }

        private void oldChBaseBut_CheckedChanged(object sender, EventArgs e)
        {
            if (oldChBaseBut.Checked)
            {
                pathTextBoxLabel.Text = "Укажите путь к базе каналов:";
                newChBaseBut.Checked = false;
                UpdatePathFromMode();
            }
        }

        private void NonCombineTagBut_CheckedChanged(object sender, EventArgs e)
        {
            if (nonCombineTagBut.Checked)
            {
                combineTagBut.Checked = false;
            }
        }

        private void combineTagBut_CheckedChanged(object sender, EventArgs e)
        {
            if (combineTagBut.Checked)
            {
                nonCombineTagBut.Checked = false;
            }
        }

        private void oldFormatBut_CheckedChanged(object sender, EventArgs e)
        {
            if (oldFormatBut.Checked)
            {
                newFormatBut.Checked = false;
            }
        }

        private void newFormatBut_CheckedChanged(object sender, EventArgs e)
        {
            if (newFormatBut.Checked)
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
            bool combineTag = combineTagBut.Checked;
            bool useNewNames = newFormatBut.Checked;

            if (pathIsValid)
            {
                var reporter = new XmlReporter();
                if (newChBaseBut.Checked)
                {
                    string filePath = Path.Combine(path, $"{projectName}{chBaseFormat}");
                    bool rewrite = true;
                    reporter.SaveAsCDBX(filePath, combineTag, useNewNames, rewrite);
                }
                else
                {
                    reporter.SaveAsCDBX(path, combineTag, useNewNames);
                }

                Close();
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
                if (!File.Exists(path))
                {
                    MessageBox.Show("Не найден файл базы каналов", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (!path.Contains(chBaseFormat))
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
        private string docDirectoryPath = string.Empty;
        private string chBaseFormat = ".cdbx";
    }
}
