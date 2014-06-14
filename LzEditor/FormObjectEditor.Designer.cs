namespace LzEditor
{
    partial class FormObjectEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormObjectEditor));
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.panelPalletScroll = new System.Windows.Forms.Panel();
            this.picturePallet = new System.Windows.Forms.PictureBox();
            this.splitBrowser = new System.Windows.Forms.SplitContainer();
            this.panelCanvas = new System.Windows.Forms.Panel();
            this.splitTree = new System.Windows.Forms.SplitContainer();
            this.treeObjects = new System.Windows.Forms.TreeView();
            this.imageListTreeIcons = new System.Windows.Forms.ImageList(this.components);
            this.treeResources = new System.Windows.Forms.TreeView();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.objectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.brushToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.drawToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.originToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.obstacleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.panelPalletScroll.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picturePallet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitBrowser)).BeginInit();
            this.splitBrowser.Panel1.SuspendLayout();
            this.splitBrowser.Panel2.SuspendLayout();
            this.splitBrowser.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitTree)).BeginInit();
            this.splitTree.Panel1.SuspendLayout();
            this.splitTree.Panel2.SuspendLayout();
            this.splitTree.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitMain
            // 
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.Location = new System.Drawing.Point(0, 24);
            this.splitMain.Name = "splitMain";
            // 
            // splitMain.Panel1
            // 
            this.splitMain.Panel1.Controls.Add(this.panelPalletScroll);
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.splitBrowser);
            this.splitMain.Size = new System.Drawing.Size(1138, 609);
            this.splitMain.SplitterDistance = 276;
            this.splitMain.TabIndex = 0;
            // 
            // panelPalletScroll
            // 
            this.panelPalletScroll.AutoScroll = true;
            this.panelPalletScroll.BackColor = System.Drawing.SystemColors.Window;
            this.panelPalletScroll.Controls.Add(this.picturePallet);
            this.panelPalletScroll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelPalletScroll.Location = new System.Drawing.Point(0, 0);
            this.panelPalletScroll.Name = "panelPalletScroll";
            this.panelPalletScroll.Size = new System.Drawing.Size(276, 609);
            this.panelPalletScroll.TabIndex = 0;
            // 
            // picturePallet
            // 
            this.picturePallet.Location = new System.Drawing.Point(0, 0);
            this.picturePallet.Name = "picturePallet";
            this.picturePallet.Size = new System.Drawing.Size(256, 576);
            this.picturePallet.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picturePallet.TabIndex = 0;
            this.picturePallet.TabStop = false;
            this.picturePallet.Paint += new System.Windows.Forms.PaintEventHandler(this.picturePallet_Paint);
            this.picturePallet.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picturePallet_MouseDown);
            this.picturePallet.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picturePallet_MouseMove);
            this.picturePallet.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picturePallet_MouseUp);
            // 
            // splitBrowser
            // 
            this.splitBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitBrowser.Location = new System.Drawing.Point(0, 0);
            this.splitBrowser.Name = "splitBrowser";
            // 
            // splitBrowser.Panel1
            // 
            this.splitBrowser.Panel1.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.splitBrowser.Panel1.Controls.Add(this.panelCanvas);
            this.splitBrowser.Panel1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.splitBrowser_Panel1_MouseDoubleClick);
            // 
            // splitBrowser.Panel2
            // 
            this.splitBrowser.Panel2.Controls.Add(this.splitTree);
            this.splitBrowser.Size = new System.Drawing.Size(858, 609);
            this.splitBrowser.SplitterDistance = 545;
            this.splitBrowser.TabIndex = 0;
            // 
            // panelCanvas
            // 
            this.panelCanvas.BackColor = System.Drawing.SystemColors.Window;
            this.panelCanvas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelCanvas.Location = new System.Drawing.Point(121, 151);
            this.panelCanvas.Name = "panelCanvas";
            this.panelCanvas.Size = new System.Drawing.Size(320, 320);
            this.panelCanvas.TabIndex = 0;
            this.panelCanvas.Paint += new System.Windows.Forms.PaintEventHandler(this.panelCanvas_Paint);
            this.panelCanvas.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelCanvas_MouseDown);
            this.panelCanvas.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelCanvas_MouseMove);
            // 
            // splitTree
            // 
            this.splitTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitTree.Location = new System.Drawing.Point(0, 0);
            this.splitTree.Name = "splitTree";
            this.splitTree.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitTree.Panel1
            // 
            this.splitTree.Panel1.Controls.Add(this.treeObjects);
            // 
            // splitTree.Panel2
            // 
            this.splitTree.Panel2.Controls.Add(this.treeResources);
            this.splitTree.Size = new System.Drawing.Size(309, 609);
            this.splitTree.SplitterDistance = 316;
            this.splitTree.TabIndex = 1;
            // 
            // treeObjects
            // 
            this.treeObjects.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeObjects.ImageIndex = 0;
            this.treeObjects.ImageList = this.imageListTreeIcons;
            this.treeObjects.Location = new System.Drawing.Point(3, 12);
            this.treeObjects.Name = "treeObjects";
            this.treeObjects.SelectedImageIndex = 0;
            this.treeObjects.Size = new System.Drawing.Size(299, 301);
            this.treeObjects.TabIndex = 0;
            this.treeObjects.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeObjects_NodeMouseClick);
            // 
            // imageListTreeIcons
            // 
            this.imageListTreeIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListTreeIcons.ImageStream")));
            this.imageListTreeIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListTreeIcons.Images.SetKeyName(0, "base.png");
            this.imageListTreeIcons.Images.SetKeyName(1, "blog.png");
            this.imageListTreeIcons.Images.SetKeyName(2, "online.png");
            this.imageListTreeIcons.Images.SetKeyName(3, "tag.png");
            // 
            // treeResources
            // 
            this.treeResources.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeResources.ImageIndex = 1;
            this.treeResources.ImageList = this.imageListTreeIcons;
            this.treeResources.Location = new System.Drawing.Point(3, 3);
            this.treeResources.Name = "treeResources";
            this.treeResources.SelectedImageIndex = 0;
            this.treeResources.Size = new System.Drawing.Size(299, 274);
            this.treeResources.TabIndex = 0;
            this.treeResources.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeResources_NodeMouseClick);
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.objectToolStripMenuItem,
            this.brushToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1138, 24);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip1";
            // 
            // objectToolStripMenuItem
            // 
            this.objectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.saveToolStripMenuItem});
            this.objectToolStripMenuItem.Name = "objectToolStripMenuItem";
            this.objectToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.objectToolStripMenuItem.Text = "&Object";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.newToolStripMenuItem.Text = "&New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // brushToolStripMenuItem
            // 
            this.brushToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.noneToolStripMenuItem,
            this.drawToolStripMenuItem,
            this.removeToolStripMenuItem,
            this.originToolStripMenuItem,
            this.obstacleToolStripMenuItem});
            this.brushToolStripMenuItem.Name = "brushToolStripMenuItem";
            this.brushToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
            this.brushToolStripMenuItem.Text = "&Brush";
            // 
            // noneToolStripMenuItem
            // 
            this.noneToolStripMenuItem.Checked = true;
            this.noneToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.noneToolStripMenuItem.Name = "noneToolStripMenuItem";
            this.noneToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.noneToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.noneToolStripMenuItem.Text = "Non&e";
            this.noneToolStripMenuItem.Click += new System.EventHandler(this.brushItemToolStripMenuItem_Click);
            // 
            // drawToolStripMenuItem
            // 
            this.drawToolStripMenuItem.Name = "drawToolStripMenuItem";
            this.drawToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.drawToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.drawToolStripMenuItem.Text = "&Draw";
            this.drawToolStripMenuItem.Click += new System.EventHandler(this.brushItemToolStripMenuItem_Click);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.removeToolStripMenuItem.Text = "&Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.brushItemToolStripMenuItem_Click);
            // 
            // originToolStripMenuItem
            // 
            this.originToolStripMenuItem.Name = "originToolStripMenuItem";
            this.originToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.originToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.originToolStripMenuItem.Text = "&Origin";
            this.originToolStripMenuItem.Click += new System.EventHandler(this.brushItemToolStripMenuItem_Click);
            // 
            // obstacleToolStripMenuItem
            // 
            this.obstacleToolStripMenuItem.Name = "obstacleToolStripMenuItem";
            this.obstacleToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
            this.obstacleToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.obstacleToolStripMenuItem.Text = "O&bstacle";
            this.obstacleToolStripMenuItem.Click += new System.EventHandler(this.brushItemToolStripMenuItem_Click);
            // 
            // FormObjectEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1138, 633);
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.menuStrip);
            this.Name = "FormObjectEditor";
            this.Text = "Object Editor";
            this.Load += new System.EventHandler(this.FormObjectEditor_Load);
            this.Resize += new System.EventHandler(this.FormObjectEditor_Resize);
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.panelPalletScroll.ResumeLayout(false);
            this.panelPalletScroll.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picturePallet)).EndInit();
            this.splitBrowser.Panel1.ResumeLayout(false);
            this.splitBrowser.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitBrowser)).EndInit();
            this.splitBrowser.ResumeLayout(false);
            this.splitTree.Panel1.ResumeLayout(false);
            this.splitTree.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitTree)).EndInit();
            this.splitTree.ResumeLayout(false);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.SplitContainer splitBrowser;
        private System.Windows.Forms.SplitContainer splitTree;
        private System.Windows.Forms.TreeView treeObjects;
        private System.Windows.Forms.TreeView treeResources;
        private System.Windows.Forms.Panel panelPalletScroll;
        private System.Windows.Forms.PictureBox picturePallet;
        private System.Windows.Forms.Panel panelCanvas;
        private System.Windows.Forms.ImageList imageListTreeIcons;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem objectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem brushToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem drawToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem originToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem obstacleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem noneToolStripMenuItem;
    }
}