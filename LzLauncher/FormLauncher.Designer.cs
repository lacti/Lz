namespace LzLauncher
{
    partial class FormLauncher
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
            this.textHistory = new System.Windows.Forms.TextBox();
            this.labelUpdateHistory = new System.Windows.Forms.Label();
            this.labelStatus = new System.Windows.Forms.Label();
            this.progressUpdate = new System.Windows.Forms.ProgressBar();
            this.buttonStart = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textHistory
            // 
            this.textHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textHistory.BackColor = System.Drawing.SystemColors.Info;
            this.textHistory.Font = new System.Drawing.Font("Malgun Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textHistory.Location = new System.Drawing.Point(12, 24);
            this.textHistory.Multiline = true;
            this.textHistory.Name = "textHistory";
            this.textHistory.ReadOnly = true;
            this.textHistory.Size = new System.Drawing.Size(542, 381);
            this.textHistory.TabIndex = 1;
            // 
            // labelUpdateHistory
            // 
            this.labelUpdateHistory.AutoSize = true;
            this.labelUpdateHistory.Location = new System.Drawing.Point(12, 9);
            this.labelUpdateHistory.Name = "labelUpdateHistory";
            this.labelUpdateHistory.Size = new System.Drawing.Size(105, 12);
            this.labelUpdateHistory.TabIndex = 0;
            this.labelUpdateHistory.Text = "업데이트 히스토리";
            // 
            // labelStatus
            // 
            this.labelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(12, 445);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(17, 12);
            this.labelStatus.TabIndex = 3;
            this.labelStatus.Text = "...";
            // 
            // progressUpdate
            // 
            this.progressUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressUpdate.Location = new System.Drawing.Point(12, 419);
            this.progressUpdate.Name = "progressUpdate";
            this.progressUpdate.Size = new System.Drawing.Size(461, 23);
            this.progressUpdate.TabIndex = 2;
            // 
            // buttonStart
            // 
            this.buttonStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonStart.Enabled = false;
            this.buttonStart.Location = new System.Drawing.Point(479, 419);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 38);
            this.buttonStart.TabIndex = 4;
            this.buttonStart.Text = "&Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // FormLauncher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(566, 469);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.progressUpdate);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.labelUpdateHistory);
            this.Controls.Add(this.textHistory);
            this.Name = "FormLauncher";
            this.Text = "Launcher [Lz]";
            this.Load += new System.EventHandler(this.FormLauncher_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textHistory;
        private System.Windows.Forms.Label labelUpdateHistory;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.ProgressBar progressUpdate;
        private System.Windows.Forms.Button buttonStart;
    }
}

