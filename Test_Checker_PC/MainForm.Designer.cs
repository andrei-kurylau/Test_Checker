namespace Test_Checker_PC
{
    partial class MainForm
    {
        /// <summary>
        /// Требуется переменная конструктора.
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

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonAddDevice = new System.Windows.Forms.Button();
            this.Devices = new System.Windows.Forms.ListBox();
            this.buttonScan = new System.Windows.Forms.Button();
            this.pictureBoxScannedImg = new System.Windows.Forms.PictureBox();
            this.buttonFromFile = new System.Windows.Forms.Button();
            this.TotalLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxScannedImg)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonAddDevice
            // 
            this.buttonAddDevice.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonAddDevice.Location = new System.Drawing.Point(13, 31);
            this.buttonAddDevice.Name = "buttonAddDevice";
            this.buttonAddDevice.Size = new System.Drawing.Size(179, 64);
            this.buttonAddDevice.TabIndex = 1;
            this.buttonAddDevice.Text = "Find Devices";
            this.buttonAddDevice.UseVisualStyleBackColor = true;
            this.buttonAddDevice.Click += new System.EventHandler(this.buttonAddDevice_Click);
            // 
            // Devices
            // 
            this.Devices.FormattingEnabled = true;
            this.Devices.Location = new System.Drawing.Point(14, 101);
            this.Devices.Name = "Devices";
            this.Devices.Size = new System.Drawing.Size(178, 173);
            this.Devices.TabIndex = 2;
            // 
            // buttonScan
            // 
            this.buttonScan.Location = new System.Drawing.Point(12, 280);
            this.buttonScan.Name = "buttonScan";
            this.buttonScan.Size = new System.Drawing.Size(179, 64);
            this.buttonScan.TabIndex = 3;
            this.buttonScan.Text = "Input From Scanner";
            this.buttonScan.UseVisualStyleBackColor = true;
            this.buttonScan.Click += new System.EventHandler(this.buttonScan_Click);
            // 
            // pictureBoxScannedImg
            // 
            this.pictureBoxScannedImg.Location = new System.Drawing.Point(206, 31);
            this.pictureBoxScannedImg.Name = "pictureBoxScannedImg";
            this.pictureBoxScannedImg.Size = new System.Drawing.Size(726, 349);
            this.pictureBoxScannedImg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxScannedImg.TabIndex = 4;
            this.pictureBoxScannedImg.TabStop = false;
            // 
            // buttonFromFile
            // 
            this.buttonFromFile.Location = new System.Drawing.Point(12, 350);
            this.buttonFromFile.Name = "buttonFromFile";
            this.buttonFromFile.Size = new System.Drawing.Size(179, 59);
            this.buttonFromFile.TabIndex = 5;
            this.buttonFromFile.Text = "Input From File";
            this.buttonFromFile.UseVisualStyleBackColor = true;
            this.buttonFromFile.Click += new System.EventHandler(this.buttonFromFile_Click);
            // 
            // TotalLabel
            // 
            this.TotalLabel.AutoSize = true;
            this.TotalLabel.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TotalLabel.Location = new System.Drawing.Point(436, 410);
            this.TotalLabel.Name = "TotalLabel";
            this.TotalLabel.Size = new System.Drawing.Size(180, 32);
            this.TotalLabel.TabIndex = 8;
            this.TotalLabel.Text = "Total correct :";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(944, 477);
            this.Controls.Add(this.TotalLabel);
            this.Controls.Add(this.buttonFromFile);
            this.Controls.Add(this.pictureBoxScannedImg);
            this.Controls.Add(this.buttonScan);
            this.Controls.Add(this.Devices);
            this.Controls.Add(this.buttonAddDevice);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Test Checker PC";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxScannedImg)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonAddDevice;
        private System.Windows.Forms.ListBox Devices;
        private System.Windows.Forms.Button buttonScan;
        private System.Windows.Forms.PictureBox pictureBoxScannedImg;
        private System.Windows.Forms.Button buttonFromFile;
        private System.Windows.Forms.Label TotalLabel;

    }
}

