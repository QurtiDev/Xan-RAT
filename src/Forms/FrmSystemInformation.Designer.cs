using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using InvokedServer.Controls;
using InvokedServer.Properties;
using InvokedServer.Utilities;

namespace InvokedServer.Forms
{
    partial class FrmSystemInformation
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
            ListViewColumnSorter viewColumnSorter = new ListViewColumnSorter();
            ComponentResourceManager resources = new ComponentResourceManager(typeof(FrmSystemInformation));
            this.lstSystem = new AeroListView();
            this.hComponent = new ColumnHeader();
            this.hValue = new ColumnHeader();
            this.contextMenuStrip = new ContextMenuStrip(this.components);
            this.copyToClipboardToolStripMenuItem = new ToolStripMenuItem();
            this.copyAllToolStripMenuItem = new ToolStripMenuItem();
            this.copySelectedToolStripMenuItem = new ToolStripMenuItem();
            this.toolStripMenuItem2 = new ToolStripSeparator();
            this.refreshToolStripMenuItem = new ToolStripMenuItem();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            this.lstSystem.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.lstSystem.Columns.AddRange(new ColumnHeader[2]
            {
        this.hComponent,
        this.hValue
            });
            this.lstSystem.ContextMenuStrip = this.contextMenuStrip;
            this.lstSystem.FullRowSelect = true;
            this.lstSystem.GridLines = true;
            this.lstSystem.HideSelection = false;
            this.lstSystem.Location = new Point(12, 12);
            viewColumnSorter.NeedNumberCompare = false;
            viewColumnSorter.Order = SortOrder.None;
            viewColumnSorter.SortColumn = 0;
            this.lstSystem.LvwColumnSorter = viewColumnSorter;
            this.lstSystem.Name = "lstSystem";
            this.lstSystem.Size = new Size(536, 311);
            this.lstSystem.TabIndex = 0;
            this.lstSystem.UseCompatibleStateImageBehavior = false;
            this.lstSystem.View = View.Details;
            this.hComponent.Text = "Component";
            this.hComponent.Width = 172;
            this.hValue.Text = "Value";
            this.hValue.Width = 321;
            this.contextMenuStrip.Items.AddRange(new ToolStripItem[3]
            {
        (ToolStripItem) this.copyToClipboardToolStripMenuItem,
        (ToolStripItem) this.toolStripMenuItem2,
        (ToolStripItem) this.refreshToolStripMenuItem
            });
            this.contextMenuStrip.Name = "ctxtMenu";
            this.contextMenuStrip.Size = new Size(172, 54);
            this.copyToClipboardToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[2]
            {
        (ToolStripItem) this.copyAllToolStripMenuItem,
        (ToolStripItem) this.copySelectedToolStripMenuItem
            });
            this.copyToClipboardToolStripMenuItem.Image = (Image)Resources.page_copy;
            this.copyToClipboardToolStripMenuItem.Name = "copyToClipboardToolStripMenuItem";
            this.copyToClipboardToolStripMenuItem.Size = new Size(171, 22);
            this.copyToClipboardToolStripMenuItem.Text = "Copy to Clipboard";
            this.copyAllToolStripMenuItem.Name = "copyAllToolStripMenuItem";
            this.copyAllToolStripMenuItem.Size = new Size(118, 22);
            this.copyAllToolStripMenuItem.Text = "All";
            this.copyAllToolStripMenuItem.Click += new EventHandler(this.copyAllToolStripMenuItem_Click);
            this.copySelectedToolStripMenuItem.Name = "copySelectedToolStripMenuItem";
            this.copySelectedToolStripMenuItem.Size = new Size(118, 22);
            this.copySelectedToolStripMenuItem.Text = "Selected";
            this.copySelectedToolStripMenuItem.Click += new EventHandler(this.copySelectedToolStripMenuItem_Click);
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new Size(168, 6);
            this.refreshToolStripMenuItem.Image = (Image)Resources.refresh;
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new Size(171, 22);
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new EventHandler(this.refreshToolStripMenuItem_Click);
            this.AutoScaleDimensions = new SizeF(96f, 96f);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.ClientSize = new Size(560, 335);
            this.Controls.Add((Control)this.lstSystem);
            this.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            this.Icon = (Icon)resources.GetObject("$this.Icon");
            this.MinimumSize = new Size(576, 373);
            this.Name = "FrmSystemInformation";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "System Information []";
            this.FormClosing += new FormClosingEventHandler(this.FrmSystemInformation_FormClosing);
            this.Load += new EventHandler(this.FrmSystemInformation_Load);
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private ColumnHeader hComponent;
        private ColumnHeader hValue;
        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem copyToClipboardToolStripMenuItem;
        private AeroListView lstSystem;
        private ToolStripMenuItem copyAllToolStripMenuItem;
        private ToolStripMenuItem copySelectedToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem2;
        private ToolStripMenuItem refreshToolStripMenuItem;
    }
}
