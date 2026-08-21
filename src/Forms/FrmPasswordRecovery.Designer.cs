using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using InvokedServer.Controls;
using InvokedServer.Properties;
using InvokedServer.Utilities;

namespace InvokedServer.Forms
{
    partial class FrmPasswordRecovery
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
            ComponentResourceManager resources = new ComponentResourceManager(typeof(FrmPasswordRecovery));
            this.contextMenuStrip = new ContextMenuStrip(this.components);
            this.saveToFileToolStripMenuItem = new ToolStripMenuItem();
            this.saveAllToolStripMenuItem = new ToolStripMenuItem();
            this.saveSelectedToolStripMenuItem = new ToolStripMenuItem();
            this.copyToClipboardToolStripMenuItem = new ToolStripMenuItem();
            this.copyAllToolStripMenuItem = new ToolStripMenuItem();
            this.copySelectedToolStripMenuItem = new ToolStripMenuItem();
            this.toolStripSeparator1 = new ToolStripSeparator();
            this.clearToolStripMenuItem = new ToolStripMenuItem();
            this.clearAllToolStripMenuItem = new ToolStripMenuItem();
            this.clearSelectedToolStripMenuItem = new ToolStripMenuItem();
            this.refreshToolStripMenuItem = new ToolStripMenuItem();
            this.groupBox1 = new GroupBox();
            this.PluginLabel = new Label();
            this.lstPasswords = new AeroListView();
            this.hIdentification = new ColumnHeader();
            this.hURL = new ColumnHeader();
            this.hUser = new ColumnHeader();
            this.hPass = new ColumnHeader();
            this.groupBox2 = new GroupBox();
            this.lblInfo = new Label();
            this.txtFormat = new TextBox();
            this.contextMenuStrip.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            this.contextMenuStrip.Items.AddRange(new ToolStripItem[5]
            {
        (ToolStripItem) this.saveToFileToolStripMenuItem,
        (ToolStripItem) this.copyToClipboardToolStripMenuItem,
        (ToolStripItem) this.toolStripSeparator1,
        (ToolStripItem) this.clearToolStripMenuItem,
        (ToolStripItem) this.refreshToolStripMenuItem
            });
            this.contextMenuStrip.Name = "menuMain";
            this.contextMenuStrip.Size = new Size(172, 98);
            this.saveToFileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[2]
            {
        (ToolStripItem) this.saveAllToolStripMenuItem,
        (ToolStripItem) this.saveSelectedToolStripMenuItem
            });
            this.saveToFileToolStripMenuItem.Image = (Image)Resources.save;
            this.saveToFileToolStripMenuItem.Name = "saveToFileToolStripMenuItem";
            this.saveToFileToolStripMenuItem.Size = new Size(171, 22);
            this.saveToFileToolStripMenuItem.Text = "Save to File";
            this.saveAllToolStripMenuItem.Name = "saveAllToolStripMenuItem";
            this.saveAllToolStripMenuItem.Size = new Size(118, 22);
            this.saveAllToolStripMenuItem.Text = "All";
            this.saveAllToolStripMenuItem.Click += new EventHandler(this.saveAllToolStripMenuItem_Click);
            this.saveSelectedToolStripMenuItem.Name = "saveSelectedToolStripMenuItem";
            this.saveSelectedToolStripMenuItem.Size = new Size(118, 22);
            this.saveSelectedToolStripMenuItem.Text = "Selected";
            this.saveSelectedToolStripMenuItem.Click += new EventHandler(this.saveSelectedToolStripMenuItem_Click);
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
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new Size(168, 6);
            this.clearToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[2]
            {
        (ToolStripItem) this.clearAllToolStripMenuItem,
        (ToolStripItem) this.clearSelectedToolStripMenuItem
            });
            this.clearToolStripMenuItem.Image = (Image)Resources.delete;
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new Size(171, 22);
            this.clearToolStripMenuItem.Text = "Clear";
            this.clearAllToolStripMenuItem.Name = "clearAllToolStripMenuItem";
            this.clearAllToolStripMenuItem.Size = new Size(118, 22);
            this.clearAllToolStripMenuItem.Text = "All";
            this.clearAllToolStripMenuItem.Click += new EventHandler(this.clearAllToolStripMenuItem_Click);
            this.clearSelectedToolStripMenuItem.Name = "clearSelectedToolStripMenuItem";
            this.clearSelectedToolStripMenuItem.Size = new Size(118, 22);
            this.clearSelectedToolStripMenuItem.Text = "Selected";
            this.clearSelectedToolStripMenuItem.Click += new EventHandler(this.clearSelectedToolStripMenuItem_Click);
            this.refreshToolStripMenuItem.Image = (Image)Resources.refresh;
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new Size(171, 22);
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new EventHandler(this.refreshToolStripMenuItem_Click);
            this.groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.groupBox1.Controls.Add((Control)this.PluginLabel);
            this.groupBox1.Controls.Add((Control)this.lstPasswords);
            this.groupBox1.ForeColor = Color.White;
            this.groupBox1.Location = new Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new Size(549, 325);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Recovered Accounts";
            this.PluginLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.PluginLabel.AutoSize = true;
            this.PluginLabel.BackColor = Color.Black;
            this.PluginLabel.ForeColor = Color.OrangeRed;
            this.PluginLabel.Location = new Point(182, 118);
            this.PluginLabel.Name = "PluginLabel";
            this.PluginLabel.Size = new Size(167, 13);
            this.PluginLabel.TabIndex = 42;
            this.PluginLabel.Text = "Checking if Module is loaded...";
            this.lstPasswords.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.lstPasswords.BackColor = Color.FromArgb(58, 61, 76);
            this.lstPasswords.Columns.AddRange(new ColumnHeader[4]
            {
        this.hIdentification,
        this.hURL,
        this.hUser,
        this.hPass
            });
            this.lstPasswords.ContextMenuStrip = this.contextMenuStrip;
            this.lstPasswords.ForeColor = Color.White;
            this.lstPasswords.FullRowSelect = true;
            this.lstPasswords.HideSelection = false;
            this.lstPasswords.Location = new Point(6, 19);
            viewColumnSorter.NeedNumberCompare = false;
            viewColumnSorter.Order = SortOrder.None;
            viewColumnSorter.SortColumn = 0;
            this.lstPasswords.LvwColumnSorter = viewColumnSorter;
            this.lstPasswords.Name = "lstPasswords";
            this.lstPasswords.Size = new Size(537, 300);
            this.lstPasswords.TabIndex = 0;
            this.lstPasswords.UseCompatibleStateImageBehavior = false;
            this.lstPasswords.View = View.Details;
            this.hIdentification.Text = "Identification";
            this.hIdentification.Width = 107;
            this.hURL.Text = "URL / Location";
            this.hURL.Width = 151;
            this.hUser.Text = "Username";
            this.hUser.Width = 142;
            this.hPass.Text = "Password";
            this.hPass.Width = 130;
            this.groupBox2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.groupBox2.Controls.Add((Control)this.lblInfo);
            this.groupBox2.Controls.Add((Control)this.txtFormat);
            this.groupBox2.ForeColor = Color.White;
            this.groupBox2.Location = new Point(12, 343);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new Size(549, 90);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Custom Saving/Copying Format";
            this.lblInfo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.lblInfo.ForeColor = Color.White;
            this.lblInfo.Location = new Point(35, 50);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new Size(467, 26);
            this.lblInfo.TabIndex = 1;
            this.lblInfo.Text = "You can change the way the accounts are saved by adjusting the format in the box above.\r\nAvailable variables: APP, URL, USER, PASS\r\n";
            this.lblInfo.TextAlign = ContentAlignment.MiddleCenter;
            this.txtFormat.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.txtFormat.Location = new Point(6, 19);
            this.txtFormat.Name = "txtFormat";
            this.txtFormat.Size = new Size(537, 22);
            this.txtFormat.TabIndex = 0;
            this.txtFormat.Text = "APP - URL - USER:PASS";
            this.txtFormat.TextAlign = HorizontalAlignment.Center;
            this.AutoScaleDimensions = new SizeF(96f, 96f);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.BackColor = Color.FromArgb(58, 61, 76);
            this.ClientSize = new Size(573, 445);
            this.Controls.Add((Control)this.groupBox2);
            this.Controls.Add((Control)this.groupBox1);
            this.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            this.Icon = (Icon)resources.GetObject("$this.Icon");
            this.MinimumSize = new Size(589, 400);
            this.Name = "FrmPasswordRecovery";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Password Recovery []";
            this.FormClosing += new FormClosingEventHandler(this.FrmPasswordRecovery_FormClosing);
            this.Load += new EventHandler(this.FrmPasswordRecovery_Load);
            this.Resize += new EventHandler(this.FrmPasswordRecovery_Resize);
            this.contextMenuStrip.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
        }

        private AeroListView lstPasswords;
        private GroupBox groupBox1;
        private ColumnHeader hIdentification;
        private ColumnHeader hURL;
        private ColumnHeader hUser;
        private ColumnHeader hPass;
        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem saveToFileToolStripMenuItem;
        private ToolStripMenuItem copyToClipboardToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem clearToolStripMenuItem;
        private ToolStripMenuItem refreshToolStripMenuItem;
        private ToolStripMenuItem saveAllToolStripMenuItem;
        private ToolStripMenuItem saveSelectedToolStripMenuItem;
        private ToolStripMenuItem copyAllToolStripMenuItem;
        private ToolStripMenuItem copySelectedToolStripMenuItem;
        private GroupBox groupBox2;
        private Label lblInfo;
        private TextBox txtFormat;
        private ToolStripMenuItem clearAllToolStripMenuItem;
        private ToolStripMenuItem clearSelectedToolStripMenuItem;
        private Label PluginLabel;
    }
}
