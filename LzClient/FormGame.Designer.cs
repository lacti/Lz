using LzClient.Util;

namespace LzClient
{
    partial class FormGame
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
            this.components = new System.ComponentModel.Container();
            this.timerGame = new System.Windows.Forms.Timer(this.components);
            this.panelCanvas = new LzClient.Util.CanvasPanel();
            this.textChat = new System.Windows.Forms.TextBox();
            this.panelCanvas.SuspendLayout();
            this.SuspendLayout();
            // 
            // timerGame
            // 
            this.timerGame.Enabled = true;
            this.timerGame.Interval = 33;
            this.timerGame.Tick += new System.EventHandler(this.timerGame_Tick);
            // 
            // panelCanvas
            // 
            this.panelCanvas.BackgroundImage = global::LzClient.Properties.Resources.Land001;
            this.panelCanvas.Controls.Add(this.textChat);
            this.panelCanvas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCanvas.Location = new System.Drawing.Point(0, 0);
            this.panelCanvas.Name = "panelCanvas";
            this.panelCanvas.Size = new System.Drawing.Size(764, 510);
            this.panelCanvas.TabIndex = 0;
            this.panelCanvas.Paint += new System.Windows.Forms.PaintEventHandler(this.panelCanvas_Paint);
            // 
            // textChat
            // 
            this.textChat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textChat.Enabled = false;
            this.textChat.Location = new System.Drawing.Point(0, 490);
            this.textChat.Name = "textChat";
            this.textChat.Size = new System.Drawing.Size(764, 21);
            this.textChat.TabIndex = 1;
            this.textChat.Visible = false;
            this.textChat.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textChat_KeyDown);
            // 
            // FormGame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(764, 510);
            this.Controls.Add(this.panelCanvas);
            this.KeyPreview = true;
            this.Name = "FormGame";
            this.Text = "Lz";
            this.Load += new System.EventHandler(this.FormGame_Load);
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.FormGame_PreviewKeyDown);
            this.panelCanvas.ResumeLayout(false);
            this.panelCanvas.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timerGame;
        private CanvasPanel panelCanvas;
        private System.Windows.Forms.TextBox textChat;
    }
}

