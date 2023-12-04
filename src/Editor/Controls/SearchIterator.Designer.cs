namespace EditorControls
{
    partial class SearchIterator
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.upButton = new System.Windows.Forms.Button();
            this.downButton = new System.Windows.Forms.Button();
            this.drawPanel = new System.Windows.Forms.Panel();
            this.mainTLP = new System.Windows.Forms.TableLayoutPanel();
            this.mainTLP.SuspendLayout();
            this.SuspendLayout();
            // 
            // upButton
            // 
            this.upButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.upButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.upButton.Location = new System.Drawing.Point(401, 0);
            this.upButton.Margin = new System.Windows.Forms.Padding(0);
            this.upButton.Name = "upButton";
            this.upButton.Size = new System.Drawing.Size(15, 152);
            this.upButton.TabIndex = 0;
            this.upButton.UseMnemonic = false;
            this.upButton.UseVisualStyleBackColor = true;
            this.upButton.Click += new System.EventHandler(this.UpButton_Click);
            this.upButton.Paint += new System.Windows.Forms.PaintEventHandler(this.UpButton_Paint);
            // 
            // downButton
            // 
            this.downButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.downButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.downButton.Location = new System.Drawing.Point(401, 152);
            this.downButton.Margin = new System.Windows.Forms.Padding(0);
            this.downButton.Name = "downButton";
            this.downButton.Size = new System.Drawing.Size(15, 152);
            this.downButton.TabIndex = 1;
            this.downButton.UseMnemonic = false;
            this.downButton.UseVisualStyleBackColor = true;
            this.downButton.Click += new System.EventHandler(this.DownButton_Click);
            this.downButton.Paint += new System.Windows.Forms.PaintEventHandler(this.DownButton_Paint);
            // 
            // drawPanel
            // 
            this.drawPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.drawPanel.Location = new System.Drawing.Point(0, 0);
            this.drawPanel.Margin = new System.Windows.Forms.Padding(0);
            this.drawPanel.Name = "drawPanel";
            this.mainTLP.SetRowSpan(this.drawPanel, 2);
            this.drawPanel.Size = new System.Drawing.Size(401, 304);
            this.drawPanel.TabIndex = 3;
            this.drawPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.DrawPanel_Paint);
            // 
            // mainTLP
            // 
            this.mainTLP.ColumnCount = 2;
            this.mainTLP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainTLP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.mainTLP.Controls.Add(this.downButton, 1, 1);
            this.mainTLP.Controls.Add(this.upButton, 1, 0);
            this.mainTLP.Controls.Add(this.drawPanel, 0, 0);
            this.mainTLP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTLP.Location = new System.Drawing.Point(0, 0);
            this.mainTLP.Name = "mainTLP";
            this.mainTLP.RowCount = 2;
            this.mainTLP.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.mainTLP.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.mainTLP.Size = new System.Drawing.Size(416, 304);
            this.mainTLP.TabIndex = 4;
            // 
            // SearchIterator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainTLP);
            this.Name = "SearchIterator";
            this.Size = new System.Drawing.Size(416, 304);
            this.Load += new System.EventHandler(this.SearchIterator_Load);
            this.mainTLP.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button upButton;
        private System.Windows.Forms.Button downButton;
        private System.Windows.Forms.Panel drawPanel;
        private System.Windows.Forms.TableLayoutPanel mainTLP;
    }
}
