namespace LzEditor
{
    partial class FormResize
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
            this.labelX = new System.Windows.Forms.Label();
            this.textX = new System.Windows.Forms.TextBox();
            this.labelY = new System.Windows.Forms.Label();
            this.textY = new System.Windows.Forms.TextBox();
            this.buttonConfirm = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelX
            // 
            this.labelX.AutoSize = true;
            this.labelX.Location = new System.Drawing.Point(23, 19);
            this.labelX.Name = "labelX";
            this.labelX.Size = new System.Drawing.Size(13, 12);
            this.labelX.TabIndex = 0;
            this.labelX.Text = "X";
            // 
            // textX
            // 
            this.textX.Location = new System.Drawing.Point(42, 16);
            this.textX.Name = "textX";
            this.textX.Size = new System.Drawing.Size(100, 21);
            this.textX.TabIndex = 1;
            this.textX.KeyDown += new System.Windows.Forms.KeyEventHandler(this.text_KeyDown);
            // 
            // labelY
            // 
            this.labelY.AutoSize = true;
            this.labelY.Location = new System.Drawing.Point(23, 53);
            this.labelY.Name = "labelY";
            this.labelY.Size = new System.Drawing.Size(13, 12);
            this.labelY.TabIndex = 2;
            this.labelY.Text = "Y";
            // 
            // textY
            // 
            this.textY.Location = new System.Drawing.Point(42, 50);
            this.textY.Name = "textY";
            this.textY.Size = new System.Drawing.Size(100, 21);
            this.textY.TabIndex = 3;
            this.textY.KeyDown += new System.Windows.Forms.KeyEventHandler(this.text_KeyDown);
            // 
            // buttonConfirm
            // 
            this.buttonConfirm.Location = new System.Drawing.Point(190, 48);
            this.buttonConfirm.Name = "buttonConfirm";
            this.buttonConfirm.Size = new System.Drawing.Size(75, 23);
            this.buttonConfirm.TabIndex = 4;
            this.buttonConfirm.Text = "C&onfirm";
            this.buttonConfirm.UseVisualStyleBackColor = true;
            this.buttonConfirm.Click += new System.EventHandler(this.buttonConfirm_Click);
            // 
            // FormResize
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(283, 92);
            this.Controls.Add(this.buttonConfirm);
            this.Controls.Add(this.textY);
            this.Controls.Add(this.labelY);
            this.Controls.Add(this.textX);
            this.Controls.Add(this.labelX);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormResize";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Resize";
            this.Load += new System.EventHandler(this.FormResize_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelX;
        private System.Windows.Forms.TextBox textX;
        private System.Windows.Forms.Label labelY;
        private System.Windows.Forms.TextBox textY;
        private System.Windows.Forms.Button buttonConfirm;
    }
}