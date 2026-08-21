using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using InvokedServer.Controls;
using InvokedServer.Properties;
using InvokedServer.Utilities;

namespace InvokedServer.Forms
{
    partial class FrmConnections
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
            ComponentResourceManager resources = new ComponentResourceManager(typeof(FrmConnections));
            this.contextMenuStrip = new ContextMenuStrip(this.components);
            this.refreshToolStripMenuItem = new ToolStripMenuItem();
            this.closeConnectionToolStripMenuItem = new ToolStripMenuItem();
            this.lstConnections = new AeroListView();
            this.columnHeader1 = new ColumnHeader();
            this.columnHeader2 = new ColumnHeader();
            this.columnHeader3 = new ColumnHeader();
            this.columnHeader4 = new ColumnHeader();
            this.columnHeader5 = new ColumnHeader();
            this.columnHeader6 = new ColumnHeader();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            this.contextMenuStrip.Items.AddRange(new ToolStripItem[2]
            {
                (ToolStripItem) this.refreshToolStripMenuItem,
                (ToolStripItem) this.closeConnectionToolStripMenuItem
            });
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new Size(169, 48);
            this.refreshToolStripMenuItem.Image = (Image)Resources.refresh;
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new Size(168, 22);
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new EventHandler(this.refreshToolStripMenuItem_Click);
            this.closeConnectionToolStripMenuItem.Image = (Image)Resources.uac_shield;
            this.closeConnectionToolStripMenuItem.Name = "closeConnectionToolStripMenuItem";
            this.closeConnectionToolStripMenuItem.Size = new Size(168, 22);
            this.closeConnectionToolStripMenuItem.Text = "Close Connection";
            this.closeConnectionToolStripMenuItem.Click += new EventHandler(this.closeConnectionToolStripMenuItem_Click);
            this.lstConnections.Columns.AddRange(new ColumnHeader[6]
            {
                this.columnHeader1,
                this.columnHeader2,
                this.columnHeader3,
                this.columnHeader4,
                this.columnHeader5,
                this.columnHeader6
            });
            this.lstConnections.ContextMenuStrip = this.contextMenuStrip;
            this.lstConnections.Dock = DockStyle.Fill;
            this.lstConnections.FullRowSelect = true;
            this.lstConnections.HideSelection = false;
            this.lstConnections.Location = new Point(0, 0);
            viewColumnSorter.NeedNumberCompare = false;
            viewColumnSorter.Order = SortOrder.None;
            viewColumnSorter.SortColumn = 0;
            this.lstConnections.LvwColumnSorter = viewColumnSorter;
            this.lstConnections.Name = "lstConnections";
            this.lstConnections.Size = new Size(703, 421);
            this.lstConnections.TabIndex = 0;
            this.lstConnections.UseCompatibleStateImageBehavior = false;
            this.lstConnections.View = View.Details;
            this.lstConnections.ColumnClick += new ColumnClickEventHandler(this.lstConnections_ColumnClick);
            this.columnHeader1.Text = "Process";
            this.columnHeader1.Width = 179;
            this.columnHeader2.Text = "Local Address";
            this.columnHeader2.Width = 95;
            this.columnHeader3.Text = "Local Port";
            this.columnHeader3.Width = 75;
            this.columnHeader4.Text = "Remote Address";
            this.columnHeader4.Width = 95;
            this.columnHeader5.Text = "Remote Port";
            this.columnHeader5.Width = 75;
            this.columnHeader6.Text = "State";
            this.columnHeader6.Width = 85;
            this.AutoScaleDimensions = new SizeF(96f, 96f);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.ClientSize = new Size(703, 421);
            this.Controls.Add((Control)this.lstConnections);
            this.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            this.Icon = (Icon)resources.GetObject("$this.Icon");
            this.Name = "FrmConnections";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Connections []";
            this.FormClosing += new FormClosingEventHandler(this.FrmConnections_FormClosing);
            this.Load += new EventHandler(this.FrmConnections_Load);
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private AeroListView lstConnections;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
        private ColumnHeader columnHeader4;
        private ColumnHeader columnHeader5;
        private ColumnHeader columnHeader6;
        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem refreshToolStripMenuItem;
        private ToolStripMenuItem closeConnectionToolStripMenuItem;
    }
}
