namespace EasyEPlanner
{
    partial class InterprojectExchangeForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.bindedSignalsGrid = new System.Windows.Forms.DataGridView();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.closeButton = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.advancedProjSignalsList = new System.Windows.Forms.ListView();
            this.AdvSignal = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.AdvDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.currentProjSignalsList = new System.Windows.Forms.ListView();
            this.CurrSignal = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.CurrDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.CurrenctProject = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AdvancedProject = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.bindedSignalsGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // bindedSignalsGrid
            // 
            this.bindedSignalsGrid.AllowUserToResizeColumns = false;
            this.bindedSignalsGrid.AllowUserToResizeRows = false;
            this.bindedSignalsGrid.BackgroundColor = System.Drawing.Color.White;
            this.bindedSignalsGrid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.bindedSignalsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.bindedSignalsGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CurrenctProject,
            this.AdvancedProject});
            this.bindedSignalsGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
            this.bindedSignalsGrid.Location = new System.Drawing.Point(369, 12);
            this.bindedSignalsGrid.Name = "bindedSignalsGrid";
            this.bindedSignalsGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.bindedSignalsGrid.RowHeadersVisible = false;
            this.bindedSignalsGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.bindedSignalsGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.bindedSignalsGrid.Size = new System.Drawing.Size(354, 417);
            this.bindedSignalsGrid.TabIndex = 0;
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.Window;
            this.textBox1.Location = new System.Drawing.Point(111, 12);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(251, 20);
            this.textBox1.TabIndex = 1;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(835, 11);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(194, 21);
            this.comboBox1.TabIndex = 5;
            // 
            // closeButton
            // 
            this.closeButton.Location = new System.Drawing.Point(1004, 435);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 6;
            this.closeButton.Text = "Отмена";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(923, 435);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 7;
            this.button2.Text = "Сохранить";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(1035, 10);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(19, 23);
            this.button3.TabIndex = 8;
            this.button3.Text = "+";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(1060, 10);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(19, 23);
            this.button4.TabIndex = 9;
            this.button4.Text = "-";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // advancedProjSignalsList
            // 
            this.advancedProjSignalsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.AdvSignal,
            this.AdvDescription});
            this.advancedProjSignalsList.FullRowSelect = true;
            this.advancedProjSignalsList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.advancedProjSignalsList.HideSelection = false;
            this.advancedProjSignalsList.LabelWrap = false;
            this.advancedProjSignalsList.Location = new System.Drawing.Point(729, 60);
            this.advancedProjSignalsList.MultiSelect = false;
            this.advancedProjSignalsList.Name = "advancedProjSignalsList";
            this.advancedProjSignalsList.ShowGroups = false;
            this.advancedProjSignalsList.ShowItemToolTips = true;
            this.advancedProjSignalsList.Size = new System.Drawing.Size(350, 369);
            this.advancedProjSignalsList.TabIndex = 11;
            this.advancedProjSignalsList.UseCompatibleStateImageBehavior = false;
            this.advancedProjSignalsList.View = System.Windows.Forms.View.Details;
            // 
            // AdvSignal
            // 
            this.AdvSignal.Text = "Сигнал";
            this.AdvSignal.Width = 170;
            // 
            // AdvDescription
            // 
            this.AdvDescription.Text = "Описание";
            this.AdvDescription.Width = 175;
            // 
            // currentProjSignalsList
            // 
            this.currentProjSignalsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.CurrDescription,
            this.CurrSignal});
            this.currentProjSignalsList.FullRowSelect = true;
            this.currentProjSignalsList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.currentProjSignalsList.HideSelection = false;
            this.currentProjSignalsList.LabelWrap = false;
            this.currentProjSignalsList.Location = new System.Drawing.Point(12, 60);
            this.currentProjSignalsList.MultiSelect = false;
            this.currentProjSignalsList.Name = "currentProjSignalsList";
            this.currentProjSignalsList.ShowGroups = false;
            this.currentProjSignalsList.ShowItemToolTips = true;
            this.currentProjSignalsList.Size = new System.Drawing.Size(350, 369);
            this.currentProjSignalsList.TabIndex = 12;
            this.currentProjSignalsList.UseCompatibleStateImageBehavior = false;
            this.currentProjSignalsList.View = System.Windows.Forms.View.Details;
            // 
            // CurrSignal
            // 
            this.CurrSignal.Text = "Сигнал";
            this.CurrSignal.Width = 170;
            // 
            // CurrDescription
            // 
            this.CurrDescription.Text = "Описание";
            this.CurrDescription.Width = 175;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(726, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Связуемый проект:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Текущий проект:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 435);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(85, 23);
            this.button1.TabIndex = 15;
            this.button1.Text = "Фильтр";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 40);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "Поиск:";
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.SystemColors.Window;
            this.textBox2.Location = new System.Drawing.Point(111, 37);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(251, 20);
            this.textBox2.TabIndex = 17;
            // 
            // textBox3
            // 
            this.textBox3.BackColor = System.Drawing.SystemColors.Window;
            this.textBox3.Location = new System.Drawing.Point(828, 37);
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.Size = new System.Drawing.Size(251, 20);
            this.textBox3.TabIndex = 19;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(726, 40);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 13);
            this.label4.TabIndex = 18;
            this.label4.Text = "Поиск:";
            // 
            // CurrenctProject
            // 
            this.CurrenctProject.Frozen = true;
            this.CurrenctProject.HeaderText = "Текущий проект";
            this.CurrenctProject.MinimumWidth = 175;
            this.CurrenctProject.Name = "CurrenctProject";
            this.CurrenctProject.ReadOnly = true;
            this.CurrenctProject.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.CurrenctProject.Width = 175;
            // 
            // AdvancedProject
            // 
            this.AdvancedProject.Frozen = true;
            this.AdvancedProject.HeaderText = "Связуемый проект";
            this.AdvancedProject.MinimumWidth = 175;
            this.AdvancedProject.Name = "AdvancedProject";
            this.AdvancedProject.ReadOnly = true;
            this.AdvancedProject.Width = 175;
            // 
            // InterprojectExchangeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1091, 465);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.currentProjSignalsList);
            this.Controls.Add(this.advancedProjSignalsList);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.bindedSignalsGrid);
            this.Name = "InterprojectExchangeForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Настройка межконтроллерного обмена сигналами";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.InterprojectExchangeForm_FormClosed);
            this.Load += new System.EventHandler(this.InterprojectExchangeForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.bindedSignalsGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView bindedSignalsGrid;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.ListView advancedProjSignalsList;
        private System.Windows.Forms.ColumnHeader AdvSignal;
        private System.Windows.Forms.ColumnHeader AdvDescription;
        private System.Windows.Forms.ListView currentProjSignalsList;
        private System.Windows.Forms.ColumnHeader CurrSignal;
        private System.Windows.Forms.ColumnHeader CurrDescription;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridViewTextBoxColumn CurrenctProject;
        private System.Windows.Forms.DataGridViewTextBoxColumn AdvancedProject;
    }
}