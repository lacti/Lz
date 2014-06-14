namespace LzEditor
{
    partial class FormMapEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMapEditor));
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageTiles = new System.Windows.Forms.TabPage();
            this.comboBoxTile = new System.Windows.Forms.ComboBox();
            this.picturePallet = new System.Windows.Forms.PictureBox();
            this.tabPageObjects = new System.Windows.Forms.TabPage();
            this.treeObjects = new System.Windows.Forms.TreeView();
            this.imageListTreeIcons = new System.Windows.Forms.ImageList(this.components);
            this.tabPageNpcs = new System.Windows.Forms.TabPage();
            this.panelCanvas = new System.Windows.Forms.Panel();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.mapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.brushToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.drawToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.obstacleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.zoomInToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPageTiles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picturePallet)).BeginInit();
            this.tabPageObjects.SuspendLayout();
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
            this.splitMain.Panel1.Controls.Add(this.tabControl);
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.AutoScroll = true;
            this.splitMain.Panel2.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.splitMain.Panel2.Controls.Add(this.panelCanvas);
            this.splitMain.Panel2.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.splitBrowser_Panel1_MouseDoubleClick);
            this.splitMain.Size = new System.Drawing.Size(1604, 810);
            this.splitMain.SplitterDistance = 262;
            this.splitMain.TabIndex = 0;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageTiles);
            this.tabControl.Controls.Add(this.tabPageObjects);
            this.tabControl.Controls.Add(this.tabPageNpcs);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(262, 810);
            this.tabControl.TabIndex = 2;
            // 
            // tabPageTiles
            // 
            this.tabPageTiles.AutoScroll = true;
            this.tabPageTiles.Controls.Add(this.comboBoxTile);
            this.tabPageTiles.Controls.Add(this.picturePallet);
            this.tabPageTiles.Location = new System.Drawing.Point(4, 22);
            this.tabPageTiles.Name = "tabPageTiles";
            this.tabPageTiles.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageTiles.Size = new System.Drawing.Size(254, 784);
            this.tabPageTiles.TabIndex = 0;
            this.tabPageTiles.Text = "Tiles";
            this.tabPageTiles.UseVisualStyleBackColor = true;
            // 
            // comboBoxTile
            // 
            this.comboBoxTile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxTile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTile.FormattingEnabled = true;
            this.comboBoxTile.Location = new System.Drawing.Point(0, 0);
            this.comboBoxTile.Name = "comboBoxTile";
            this.comboBoxTile.Size = new System.Drawing.Size(256, 20);
            this.comboBoxTile.TabIndex = 2;
            this.comboBoxTile.SelectedIndexChanged += new System.EventHandler(this.comboBoxTile_SelectedIndexChanged);
            // 
            // picturePallet
            // 
            this.picturePallet.Location = new System.Drawing.Point(-1, 26);
            this.picturePallet.Name = "picturePallet";
            this.picturePallet.Size = new System.Drawing.Size(256, 576);
            this.picturePallet.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picturePallet.TabIndex = 1;
            this.picturePallet.TabStop = false;
            this.picturePallet.Paint += new System.Windows.Forms.PaintEventHandler(this.picturePallet_Paint);
            this.picturePallet.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picturePallet_MouseDown);
            this.picturePallet.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picturePallet_MouseMove);
            this.picturePallet.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picturePallet_MouseUp);
            // 
            // tabPageObjects
            // 
            this.tabPageObjects.Controls.Add(this.treeObjects);
            this.tabPageObjects.Location = new System.Drawing.Point(4, 22);
            this.tabPageObjects.Name = "tabPageObjects";
            this.tabPageObjects.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageObjects.Size = new System.Drawing.Size(254, 784);
            this.tabPageObjects.TabIndex = 1;
            this.tabPageObjects.Text = "Objects";
            this.tabPageObjects.UseVisualStyleBackColor = true;
            // 
            // treeObjects
            // 
            this.treeObjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeObjects.ImageIndex = 0;
            this.treeObjects.ImageList = this.imageListTreeIcons;
            this.treeObjects.Location = new System.Drawing.Point(3, 3);
            this.treeObjects.Name = "treeObjects";
            this.treeObjects.SelectedImageIndex = 0;
            this.treeObjects.Size = new System.Drawing.Size(248, 778);
            this.treeObjects.TabIndex = 1;
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
            // tabPageNpcs
            // 
            this.tabPageNpcs.Location = new System.Drawing.Point(4, 22);
            this.tabPageNpcs.Name = "tabPageNpcs";
            this.tabPageNpcs.Size = new System.Drawing.Size(254, 784);
            this.tabPageNpcs.TabIndex = 2;
            this.tabPageNpcs.Text = "Npcs";
            this.tabPageNpcs.UseVisualStyleBackColor = true;
            // 
            // panelCanvas
            // 
            this.panelCanvas.BackColor = System.Drawing.SystemColors.Window;
            this.panelCanvas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelCanvas.Location = new System.Drawing.Point(0, 0);
            this.panelCanvas.Name = "panelCanvas";
            this.panelCanvas.Size = new System.Drawing.Size(320, 320);
            this.panelCanvas.TabIndex = 4;
            this.panelCanvas.Paint += new System.Windows.Forms.PaintEventHandler(this.panelCanvas_Paint);
            this.panelCanvas.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelCanvas_MouseDown);
            this.panelCanvas.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelCanvas_MouseMove);
            this.panelCanvas.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelCanvas_MouseUp);
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mapToolStripMenuItem,
            this.brushToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1604, 24);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip1";
            // 
            // mapToolStripMenuItem
            // 
            this.mapToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.toolStripMenuItem1,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem});
            this.mapToolStripMenuItem.Name = "mapToolStripMenuItem";
            this.mapToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.mapToolStripMenuItem.Text = "&Map";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.newToolStripMenuItem.Text = "&New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(149, 6);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // brushToolStripMenuItem
            // 
            this.brushToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectToolStripMenuItem,
            this.drawToolStripMenuItem,
            this.obstacleToolStripMenuItem});
            this.brushToolStripMenuItem.Name = "brushToolStripMenuItem";
            this.brushToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
            this.brushToolStripMenuItem.Text = "&Brush";
            // 
            // selectToolStripMenuItem
            // 
            this.selectToolStripMenuItem.Checked = true;
            this.selectToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.selectToolStripMenuItem.Name = "selectToolStripMenuItem";
            this.selectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.selectToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.selectToolStripMenuItem.Text = "&Select";
            this.selectToolStripMenuItem.Click += new System.EventHandler(this.brushItemToolStripMenuItem_Click);
            // 
            // drawToolStripMenuItem
            // 
            this.drawToolStripMenuItem.Name = "drawToolStripMenuItem";
            this.drawToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.drawToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.drawToolStripMenuItem.Text = "&Draw";
            this.drawToolStripMenuItem.Click += new System.EventHandler(this.brushItemToolStripMenuItem_Click);
            // 
            // obstacleToolStripMenuItem
            // 
            this.obstacleToolStripMenuItem.Name = "obstacleToolStripMenuItem";
            this.obstacleToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
            this.obstacleToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.obstacleToolStripMenuItem.Text = "O&bstacle";
            this.obstacleToolStripMenuItem.Click += new System.EventHandler(this.brushItemToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gridToolStripMenuItem,
            this.toolStripMenuItem2,
            this.zoomInToolStripMenuItem,
            this.zoomOutToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(45, 20);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // gridToolStripMenuItem
            // 
            this.gridToolStripMenuItem.Checked = true;
            this.gridToolStripMenuItem.CheckOnClick = true;
            this.gridToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.gridToolStripMenuItem.Name = "gridToolStripMenuItem";
            this.gridToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.gridToolStripMenuItem.Text = "&Grid";
            this.gridToolStripMenuItem.Click += new System.EventHandler(this.gridToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(169, 6);
            // 
            // zoomInToolStripMenuItem
            // 
            this.zoomInToolStripMenuItem.Name = "zoomInToolStripMenuItem";
            this.zoomInToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.zoomInToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.zoomInToolStripMenuItem.Text = "Zoom &In";
            this.zoomInToolStripMenuItem.Click += new System.EventHandler(this.zoomInToolStripMenuItem_Click);
            // 
            // zoomOutToolStripMenuItem
            // 
            this.zoomOutToolStripMenuItem.Name = "zoomOutToolStripMenuItem";
            this.zoomOutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U)));
            this.zoomOutToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.zoomOutToolStripMenuItem.Text = "Zoom &Out";
            this.zoomOutToolStripMenuItem.Click += new System.EventHandler(this.zoomOutToolStripMenuItem_Click);
            // 
            // FormMapEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1604, 834);
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.menuStrip);
            this.Name = "FormMapEditor";
            this.Text = "Map Editor";
            this.Load += new System.EventHandler(this.FormMapEditor_Load);
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.tabPageTiles.ResumeLayout(false);
            this.tabPageTiles.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picturePallet)).EndInit();
            this.tabPageObjects.ResumeLayout(false);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.ImageList imageListTreeIcons;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem mapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem brushToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem obstacleToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gridToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem zoomInToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoomOutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem drawToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageTiles;
        private System.Windows.Forms.TabPage tabPageObjects;
        private System.Windows.Forms.TreeView treeObjects;
        private System.Windows.Forms.TabPage tabPageNpcs;
        private System.Windows.Forms.ComboBox comboBoxTile;
        private System.Windows.Forms.PictureBox picturePallet;
        private System.Windows.Forms.Panel panelCanvas;
    }
}