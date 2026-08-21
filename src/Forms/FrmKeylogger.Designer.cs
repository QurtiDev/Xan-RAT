using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using Guna.UI2.WinForms.Enums;
using InvokedServer.Properties;

namespace InvokedServer.Forms
{
    partial class FrmKeylogger
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
            ComponentResourceManager resources = new ComponentResourceManager(typeof(FrmKeylogger));
            this.lstLogs = new ListView();
            this.hLogs = new ColumnHeader();
            this.statusStrip = new StatusStrip();
            this.stripLblStatus = new ToolStripStatusLabel();
            this.wLogViewer = new WebBrowser();
            this.btnGetLogs = new Guna2GradientButton();
            this.OpenAllBtn = new Guna2GradientButton();
            this.LstViewVScrollBar = new Guna2VScrollBar();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            this.lstLogs.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            this.lstLogs.BackColor = Color.FromArgb(58, 61, 76);
            this.lstLogs.Columns.AddRange(new ColumnHeader[1]
            {
        this.hLogs
            });
            this.lstLogs.Font = new Font("Segoe UI", 8.25f);
            this.lstLogs.ForeColor = Color.White;
            this.lstLogs.FullRowSelect = true;
            this.lstLogs.GridLines = true;
            this.lstLogs.HideSelection = false;
            this.lstLogs.Location = new Point(0, 60);
            this.lstLogs.Name = "lstLogs";
            this.lstLogs.Size = new Size(153, 402);
            this.lstLogs.TabIndex = 0;
            this.lstLogs.UseCompatibleStateImageBehavior = false;
            this.lstLogs.View = View.Details;
            this.lstLogs.ItemActivate += new EventHandler(this.lstLogs_ItemActivate);
            this.hLogs.Text = "Logs";
            this.hLogs.Width = 149;
            this.statusStrip.BackColor = Color.Transparent;
            this.statusStrip.Items.AddRange(new ToolStripItem[1]
            {
        (ToolStripItem) this.stripLblStatus
            });
            this.statusStrip.Location = new Point(0, 460);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new Size(862, 22);
            this.statusStrip.TabIndex = 6;
            this.statusStrip.Text = "statusStrip1";
            this.stripLblStatus.ForeColor = Color.White;
            this.stripLblStatus.Name = "stripLblStatus";
            this.stripLblStatus.Size = new Size(77, 17);
            this.stripLblStatus.Text = "Status: Ready";
            this.wLogViewer.Dock = DockStyle.Right;
            this.wLogViewer.Location = new Point(154, 0);
            this.wLogViewer.MinimumSize = new Size(20, 20);
            this.wLogViewer.Name = "wLogViewer";
            this.wLogViewer.ScriptErrorsSuppressed = true;
            this.wLogViewer.Size = new Size(708, 460);
            this.wLogViewer.TabIndex = 8;
            this.wLogViewer.Visible = false;
            this.wLogViewer.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(this.WebBrowser_DocumentCompleted);
            this.btnGetLogs.Animated = true;
            this.btnGetLogs.BackColor = Color.FromArgb(62, 72, 88);
            this.btnGetLogs.BorderColor = Color.FromArgb(72, 82, 98);
            this.btnGetLogs.BorderRadius = 2;
            this.btnGetLogs.BorderThickness = 1;
            this.btnGetLogs.DisabledState.BorderColor = Color.DarkGray;
            this.btnGetLogs.DisabledState.CustomBorderColor = Color.DarkGray;
            this.btnGetLogs.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            this.btnGetLogs.DisabledState.FillColor2 = Color.FromArgb(169, 169, 169);
            this.btnGetLogs.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            this.btnGetLogs.FillColor = Color.FromArgb(35, 35, 35);
            this.btnGetLogs.FillColor2 = Color.FromArgb(62, 72, 88);
            this.btnGetLogs.Font = new Font("Segoe UI", 8.25f);
            this.btnGetLogs.ForeColor = Color.White;
            this.btnGetLogs.GradientMode = LinearGradientMode.ForwardDiagonal;
            this.btnGetLogs.Image = (Image)Resources.refresh;
            this.btnGetLogs.ImageSize = new Size(16, 16);
            this.btnGetLogs.Location = new Point(4, 4);
            this.btnGetLogs.Name = "btnGetLogs";
            this.btnGetLogs.Size = new Size(144, 23);
            this.btnGetLogs.TabIndex = 31;
            this.btnGetLogs.Text = "Refresh Logs";
            this.btnGetLogs.Click += new EventHandler(this.btnGetLogs_Click);
            this.OpenAllBtn.Animated = true;
            this.OpenAllBtn.BackColor = Color.FromArgb(62, 72, 88);
            this.OpenAllBtn.BorderColor = Color.FromArgb(72, 82, 98);
            this.OpenAllBtn.BorderRadius = 2;
            this.OpenAllBtn.BorderThickness = 1;
            this.OpenAllBtn.DisabledState.BorderColor = Color.DarkGray;
            this.OpenAllBtn.DisabledState.CustomBorderColor = Color.DarkGray;
            this.OpenAllBtn.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            this.OpenAllBtn.DisabledState.FillColor2 = Color.FromArgb(169, 169, 169);
            this.OpenAllBtn.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            this.OpenAllBtn.FillColor = Color.FromArgb(35, 35, 35);
            this.OpenAllBtn.FillColor2 = Color.FromArgb(62, 72, 88);
            this.OpenAllBtn.Font = new Font("Segoe UI", 8.25f);
            this.OpenAllBtn.ForeColor = Color.White;
            this.OpenAllBtn.GradientMode = LinearGradientMode.ForwardDiagonal;
            this.OpenAllBtn.Image = (Image)Resources.page_copy;
            this.OpenAllBtn.ImageSize = new Size(16, 16);
            this.OpenAllBtn.Location = new Point(4, 31);
            this.OpenAllBtn.Name = "OpenAllBtn";
            this.OpenAllBtn.Size = new Size(144, 23);
            this.OpenAllBtn.TabIndex = 32;
            this.OpenAllBtn.Text = "Open All Logs";
            this.OpenAllBtn.Click += new EventHandler(this.OpenAllBtn_Click);
            this.LstViewVScrollBar.AutoScroll = true;
            this.LstViewVScrollBar.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.LstViewVScrollBar.BindingContainer = (Control)this.lstLogs;
            this.LstViewVScrollBar.FillColor = Color.FromArgb(58, 61, 76);
            this.LstViewVScrollBar.HighlightOnWheel = true;
            this.LstViewVScrollBar.InUpdate = false;
            this.LstViewVScrollBar.LargeChange = 10;
            this.LstViewVScrollBar.Location = new Point(134, 61);
            this.LstViewVScrollBar.Name = "LstViewVScrollBar";
            this.LstViewVScrollBar.ScrollbarSize = 18;
            this.LstViewVScrollBar.Size = new Size(18, 400);
            this.LstViewVScrollBar.TabIndex = 33;
            this.LstViewVScrollBar.ThumbColor = Color.FromArgb(39, 52, 66);
            this.LstViewVScrollBar.ThumbStyle = ThumbStyle.Inset;
            this.AutoScaleDimensions = new SizeF(96f, 96f);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.BackColor = Color.FromArgb(58, 61, 76);
            this.ClientSize = new Size(862, 482);
            this.Controls.Add((Control)this.LstViewVScrollBar);
            this.Controls.Add((Control)this.OpenAllBtn);
            this.Controls.Add((Control)this.btnGetLogs);
            this.Controls.Add((Control)this.wLogViewer);
            this.Controls.Add((Control)this.statusStrip);
            this.Controls.Add((Control)this.lstLogs);
            this.Font = new Font("Segoe UI", 8.25f);
            this.Icon = (Icon)resources.GetObject("$this.Icon");
            this.MinimumSize = new Size(630, 465);
            this.Name = "FrmKeylogger";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Keylogger []";
            this.FormClosing += new FormClosingEventHandler(this.FrmKeylogger_FormClosing);
            this.Load += new EventHandler(this.FrmKeylogger_Load);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private ColumnHeader hLogs;
        private StatusStrip statusStrip;
        private WebBrowser wLogViewer;
        private ListView lstLogs;
        private ToolStripStatusLabel stripLblStatus;
        private Guna2GradientButton btnGetLogs;
        private Guna2GradientButton OpenAllBtn;
        private Guna2VScrollBar LstViewVScrollBar;
    }
}
