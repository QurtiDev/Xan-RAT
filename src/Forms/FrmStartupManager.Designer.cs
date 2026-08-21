using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using InvokedServer.Controls;
using InvokedServer.Properties;
using InvokedServer.Utilities;

namespace InvokedServer.Forms
{
    partial class FrmStartupManager
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
            ComponentResourceManager resources = new ComponentResourceManager(typeof(FrmStartupManager));
            this.contextMenuStrip = new ContextMenuStrip(this.components);
            this.addEntryToolStripMenuItem = new ToolStripMenuItem();
            this.removeEntryToolStripMenuItem = new ToolStripMenuItem();
            this.lstStartupItems = new AeroListView();
            this.hName = new ColumnHeader();
            this.hPath = new ColumnHeader();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            this.contextMenuStrip.Items.AddRange(new ToolStripItem[2]
            {
        (ToolStripItem) this.addEntryToolStripMenuItem,
        (ToolStripItem) this.removeEntryToolStripMenuItem
            });
            this.contextMenuStrip.Name = "ctxtMenu";
            this.contextMenuStrip.Size = new Size(148, 48);
            this.addEntryToolStripMenuItem.Image = (Image)Resources.application_add;
            this.addEntryToolStripMenuItem.Name = "addEntryToolStripMenuItem";
            this.addEntryToolStripMenuItem.Size = new Size(147, 22);
            this.addEntryToolStripMenuItem.Text = "Add Entry";
            this.addEntryToolStripMenuItem.Click += new EventHandler(this.addEntryToolStripMenuItem_Click);
            this.removeEntryToolStripMenuItem.Image = (Image)Resources.application_delete;
            this.removeEntryToolStripMenuItem.Name = "removeEntryToolStripMenuItem";
            this.removeEntryToolStripMenuItem.Size = new Size(147, 22);
            this.removeEntryToolStripMenuItem.Text = "Remove Entry";
            this.removeEntryToolStripMenuItem.Click += new EventHandler(this.removeEntryToolStripMenuItem_Click);
            this.lstStartupItems.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.lstStartupItems.Columns.AddRange(new ColumnHeader[2]
            {
        this.hName,
        this.hPath
            });
            this.lstStartupItems.ContextMenuStrip = this.contextMenuStrip;
            this.lstStartupItems.FullRowSelect = true;
            this.lstStartupItems.GridLines = true;
            this.lstStartupItems.HideSelection = false;
            this.lstStartupItems.Location = new Point(12, 12);
            viewColumnSorter.NeedNumberCompare = false;
            viewColumnSorter.Order = SortOrder.None;
            viewColumnSorter.SortColumn = 0;
            this.lstStartupItems.LvwColumnSorter = viewColumnSorter;
            this.lstStartupItems.Name = "lstStartupItems";
            this.lstStartupItems.Size = new Size(653, 349);
            this.lstStartupItems.TabIndex = 0;
            this.lstStartupItems.UseCompatibleStateImageBehavior = false;
            this.lstStartupItems.View = View.Details;
            this.hName.Text = "Name";
            this.hName.Width = 187;
            this.hPath.Text = "Path";
            this.hPath.Width = 460;
            this.AutoScaleDimensions = new SizeF(96f, 96f);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.ClientSize = new Size(677, 373);
            this.Controls.Add((Control)this.lstStartupItems);
            this.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            this.Icon = (Icon)resources.GetObject("$this.Icon");
            this.MinimumSize = new Size(650, 400);
            this.Name = "FrmStartupManager";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Startup Manager []";
            this.FormClosing += new FormClosingEventHandler(this.FrmStartupManager_FormClosing);
            this.Load += new EventHandler(this.FrmStartupManager_Load);
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private ColumnHeader hName;
        private ColumnHeader hPath;
        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem addEntryToolStripMenuItem;
        private ToolStripMenuItem removeEntryToolStripMenuItem;
        private AeroListView lstStartupItems;
    }
}
