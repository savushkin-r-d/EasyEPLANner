namespace InterprojectExchange
{
    partial class UnknownDevTypeForm
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
            this.cancelBtn = new System.Windows.Forms.Button();
            this.okBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.AIBtn = new System.Windows.Forms.RadioButton();
            this.AOBtn = new System.Windows.Forms.RadioButton();
            this.DIBtn = new System.Windows.Forms.RadioButton();
            this.DOBtn = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // cancelBtn
            // 
            this.cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelBtn.Location = new System.Drawing.Point(120, 112);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(61, 23);
            this.cancelBtn.TabIndex = 1;
            this.cancelBtn.Text = "Отмена";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // okBtn
            // 
            this.okBtn.Location = new System.Drawing.Point(53, 112);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(61, 23);
            this.okBtn.TabIndex = 2;
            this.okBtn.Text = "ОК";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(180, 39);
            this.label1.TabIndex = 2;
            this.label1.Text = "Невозможно определить тип\r\nустройства из текущего проекта, \r\nпожалуйста укажите в" +
    "ручную";
            // 
            // AIBtn
            // 
            this.AIBtn.AutoSize = true;
            this.AIBtn.Checked = true;
            this.AIBtn.Location = new System.Drawing.Point(27, 54);
            this.AIBtn.Name = "AIBtn";
            this.AIBtn.Size = new System.Drawing.Size(35, 17);
            this.AIBtn.TabIndex = 4;
            this.AIBtn.TabStop = true;
            this.AIBtn.Text = "AI";
            this.AIBtn.UseVisualStyleBackColor = true;
            this.AIBtn.CheckedChanged += new System.EventHandler(this.AIBtn_CheckedChanged);
            // 
            // AOBtn
            // 
            this.AOBtn.AutoSize = true;
            this.AOBtn.Location = new System.Drawing.Point(120, 54);
            this.AOBtn.Name = "AOBtn";
            this.AOBtn.Size = new System.Drawing.Size(40, 17);
            this.AOBtn.TabIndex = 5;
            this.AOBtn.Text = "AO";
            this.AOBtn.UseVisualStyleBackColor = true;
            this.AOBtn.CheckedChanged += new System.EventHandler(this.AOBtn_CheckedChanged);
            // 
            // DIBtn
            // 
            this.DIBtn.AutoSize = true;
            this.DIBtn.Location = new System.Drawing.Point(26, 81);
            this.DIBtn.Name = "DIBtn";
            this.DIBtn.Size = new System.Drawing.Size(36, 17);
            this.DIBtn.TabIndex = 6;
            this.DIBtn.Text = "DI";
            this.DIBtn.UseVisualStyleBackColor = true;
            this.DIBtn.CheckedChanged += new System.EventHandler(this.DIBtn_CheckedChanged);
            // 
            // DOBtn
            // 
            this.DOBtn.AutoSize = true;
            this.DOBtn.Location = new System.Drawing.Point(119, 81);
            this.DOBtn.Name = "DOBtn";
            this.DOBtn.Size = new System.Drawing.Size(41, 17);
            this.DOBtn.TabIndex = 7;
            this.DOBtn.Text = "DO";
            this.DOBtn.UseVisualStyleBackColor = true;
            this.DOBtn.CheckedChanged += new System.EventHandler(this.DOBtn_CheckedChanged);
            // 
            // UnknownDevTypeForm
            // 
            this.AcceptButton = this.okBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelBtn;
            this.ClientSize = new System.Drawing.Size(184, 141);
            this.ControlBox = false;
            this.Controls.Add(this.DOBtn);
            this.Controls.Add(this.DIBtn);
            this.Controls.Add(this.AOBtn);
            this.Controls.Add(this.AIBtn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.okBtn);
            this.Controls.Add(this.cancelBtn);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(200, 180);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(200, 180);
            this.Name = "UnknownDevTypeForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Выберите тип устройства";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.UnknownDevTypeForm_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton AIBtn;
        private System.Windows.Forms.RadioButton AOBtn;
        private System.Windows.Forms.RadioButton DIBtn;
        private System.Windows.Forms.RadioButton DOBtn;
    }
}