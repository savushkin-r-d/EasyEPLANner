using EasyEPlanner.mpk.ModelBuilder;
using EasyEPlanner.mpk.Saver;
using EasyEPlanner.mpk.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TechObject;

namespace EasyEPlanner.mpk.View
{
    [ExcludeFromCodeCoverage]
    public partial class MpkSaverForm : Form
    {
        public IMpkSaverContext Context { get; private set; }

        public MpkSaverForm(IMpkSaverContext context)
        {
            Context = context;
            InitializeComponent();
            InitModel();
        }

        private void InitModel()
        {
            MpkDirectoryTextBox.Text = Context.MpkDirectory;
            MainContainerNameTextBox.Text = Context.MainContainerName;
            NewContainerBttn.Checked = Context.Rewrite;
            UpdateContainerBttn.Checked = !Context.Rewrite;
        }

        private void PathReviewBttn_Click(object sender, EventArgs e)
        {
            using var folderDialog = new FolderBrowserDialog();
            DirectoryInfo defaultDirectory = null;
            
            if (!Directory.Exists(Context.MpkDirectory))
                defaultDirectory = Directory.CreateDirectory(Context.MpkDirectory);

            folderDialog.SelectedPath = Context.MpkDirectory;
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                Context.MpkDirectory = folderDialog.SelectedPath;
                MpkDirectoryTextBox.Text = folderDialog.SelectedPath;

                if (defaultDirectory is not null &&
                    folderDialog.SelectedPath != defaultDirectory.FullName &&
                    !defaultDirectory.EnumerateFiles().Any())
                {
                    defaultDirectory.Delete();
                }

                return;
            }
            
            if (defaultDirectory?.EnumerateFiles().Any() is false)
                defaultDirectory.Delete();
        }

        private void CancelBttn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void ExportBttn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
