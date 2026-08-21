using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using InvokedServer.Controls;
using InvokedServer.Properties;
using InvokedServer.Utilities;

namespace InvokedServer.Forms
{
    partial class FrmTaskManager
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
            ComponentResourceManager resources = new ComponentResourceManager(typeof(FrmTaskManager));
            this.contextMenuStrip = new ContextMenuStrip(this.components);
            this.killProcessToolStripMenuItem = new ToolStripMenuItem();
            this.startProcessToolStripMenuItem = new ToolStripMenuItem();
            this.lineToolStripMenuItem = new ToolStripSeparator();
            this.refreshToolStripMenuItem = new ToolStripMenuItem();
            this.tableLayoutPanel = new TableLayoutPanel();
            this.lstTasks = new AeroListView();
            this.hProcessname = new ColumnHeader();
            this.hPID = new ColumnHeader();
            this.hTitle = new ColumnHeader();
            this.statusStrip = new StatusStrip();
            this.processesToolStripStatusLabel = new ToolStripStatusLabel();
            this.contextMenuStrip.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            this.contextMenuStrip.Items.AddRange(new ToolStripItem[4]
            {
        (ToolStripItem) this.killProcessToolStripMenuItem,
        (ToolStripItem) this.startProcessToolStripMenuItem,
        (ToolStripItem) this.lineToolStripMenuItem,
        (ToolStripItem) this.refreshToolStripMenuItem
            });
            this.contextMenuStrip.Name = "ctxtMenu";
            this.contextMenuStrip.Size = new Size(142, 76);
            this.killProcessToolStripMenuItem.Image = (Image)Resources.cancel;
            this.killProcessToolStripMenuItem.Name = "killProcessToolStripMenuItem";
            this.killProcessToolStripMenuItem.Size = new Size(141, 22);
            this.killProcessToolStripMenuItem.Text = "Kill Process";
            this.killProcessToolStripMenuItem.Click += new EventHandler(this.killProcessToolStripMenuItem_Click);
            this.startProcessToolStripMenuItem.Image = (Image)Resources.application_go;
            this.startProcessToolStripMenuItem.Name = "startProcessToolStripMenuItem";
            this.startProcessToolStripMenuItem.Size = new Size(141, 22);
            this.startProcessToolStripMenuItem.Text = "Start Process";
            this.startProcessToolStripMenuItem.Click += new EventHandler(this.startProcessToolStripMenuItem_Click);
            this.lineToolStripMenuItem.Name = "lineToolStripMenuItem";
            this.lineToolStripMenuItem.Size = new Size(138, 6);
            this.refreshToolStripMenuItem.Image = (Image)Resources.refresh;
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new Size(141, 22);
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new EventHandler(this.refreshToolStripMenuItem_Click);
            this.tableLayoutPanel.ColumnCount = 1;
            this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            this.tableLayoutPanel.Controls.Add((Control)this.lstTasks, 0, 0);
            this.tableLayoutPanel.Controls.Add((Control)this.statusStrip, 0, 1);
            this.tableLayoutPanel.Dock = DockStyle.Fill;
            this.tableLayoutPanel.Location = new Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 22f));
            this.tableLayoutPanel.Size = new Size(821, 493);
            this.tableLayoutPanel.TabIndex = 2;
            this.lstTasks.Columns.AddRange(new ColumnHeader[3]
            {
        this.hProcessname,
        this.hPID,
        this.hTitle
            });
            this.lstTasks.ContextMenuStrip = this.contextMenuStrip;
            this.lstTasks.Dock = DockStyle.Fill;
            this.lstTasks.FullRowSelect = true;
            this.lstTasks.GridLines = true;
            this.lstTasks.HideSelection = false;
            this.lstTasks.Location = new Point(3, 3);
            viewColumnSorter.NeedNumberCompare = false;
            viewColumnSorter.Order = SortOrder.None;
            viewColumnSorter.SortColumn = 0;
            this.lstTasks.LvwColumnSorter = viewColumnSorter;
            this.lstTasks.Name = "lstTasks";
            this.lstTasks.Size = new Size(815, 465);
            this.lstTasks.TabIndex = 1;
            this.lstTasks.UseCompatibleStateImageBehavior = false;
            this.lstTasks.View = View.Details;
            this.lstTasks.ColumnClick += new ColumnClickEventHandler(this.lstTasks_ColumnClick);
            this.hProcessname.Text = "Processname";
            this.hProcessname.Width = 202;
            this.hPID.Text = "PID";
            this.hTitle.Text = "Title";
            this.hTitle.Width = 531;
            this.statusStrip.Dock = DockStyle.Fill;
            this.statusStrip.Items.AddRange(new ToolStripItem[1]
            {
        (ToolStripItem) this.processesToolStripStatusLabel
            });
            this.statusStrip.Location = new Point(0, 471);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new Size(821, 22);
            this.statusStrip.TabIndex = 2;
            this.statusStrip.Text = "statusStrip1";
            this.processesToolStripStatusLabel.Name = "processesToolStripStatusLabel";
            this.processesToolStripStatusLabel.Size = new Size(70, 17);
            this.processesToolStripStatusLabel.Text = "Processes: 0";
            this.AutoScaleDimensions = new SizeF(96f, 96f);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.ClientSize = new Size(821, 493);
            this.Controls.Add((Control)this.tableLayoutPanel);
            this.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            this.Icon = (Icon)resources.GetObject("$this.Icon");
            this.MinimumSize = new Size(351, 449);
            this.Name = "FrmTaskManager";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Task Manager []";
            this.FormClosing += new FormClosingEventHandler(this.FrmTaskManager_FormClosing);
            this.Load += new EventHandler(this.FrmTaskManager_Load);
            this.contextMenuStrip.ResumeLayout(false);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
        }

        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem killProcessToolStripMenuItem;
        private ToolStripMenuItem refreshToolStripMenuItem;
        private ToolStripMenuItem startProcessToolStripMenuItem;
        private ColumnHeader hProcessname;
        private ColumnHeader hPID;
        private ColumnHeader hTitle;
        private ToolStripSeparator lineToolStripMenuItem;
        private AeroListView lstTasks;
        private TableLayoutPanel tableLayoutPanel;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel processesToolStripStatusLabel;
    }
}
