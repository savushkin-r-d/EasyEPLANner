namespace EasyEPlanner
{
    partial class IdleForm
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
            this.acceptButton = new System.Windows.Forms.Button();
            this.DisplayingInfoLabel = new System.Windows.Forms.Label();
            this.timerLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // acceptButton
            // 
            this.acceptButton.Location = new System.Drawing.Point(12, 106);
            this.acceptButton.Name = "acceptButton";
            this.acceptButton.Size = new System.Drawing.Size(246, 23);
            this.acceptButton.TabIndex = 0;
            this.acceptButton.Text = "Подтвердить";
            this.acceptButton.UseVisualStyleBackColor = true;
            this.acceptButton.Click += new System.EventHandler(this.acceptButton_Click);
            // 
            // DisplayingInfoLabel
            // 
            this.DisplayingInfoLabel.Location = new System.Drawing.Point(12, 9);
            this.DisplayingInfoLabel.Name = "DisplayingInfoLabel";
            this.DisplayingInfoLabel.Size = new System.Drawing.Size(246, 64);
            this.DisplayingInfoLabel.TabIndex = 1;
            this.DisplayingInfoLabel.Text = "Пожалуйста, подтвердите свою активность. Для этого нажмите кнопку \'Подтвердить\'. " +
    "В противном случае программа Eplan будет закрыта через 1 минуту.";
            // 
            // timerLabel
            // 
            this.timerLabel.AutoSize = true;
            this.timerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.timerLabel.Location = new System.Drawing.Point(80, 74);
            this.timerLabel.Name = "timerLabel";
            this.timerLabel.Size = new System.Drawing.Size(110, 17);
            this.timerLabel.TabIndex = 2;
            this.timerLabel.Text = "Осталось: 60 с.";
            // 
            // IdleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(270, 141);
            this.ControlBox = false;
            this.Controls.Add(this.timerLabel);
            this.Controls.Add(this.DisplayingInfoLabel);
            this.Controls.Add(this.acceptButton);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(286, 180);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(286, 180);
            this.Name = "IdleForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Подтвердите активность";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button acceptButton;
        private System.Windows.Forms.Label DisplayingInfoLabel;
        private System.Windows.Forms.Label timerLabel;
    }
}