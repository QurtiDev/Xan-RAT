using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InvokedServer.Forms
{
    partial class FrmSettings
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
            ComponentResourceManager resources = new ComponentResourceManager(typeof(FrmSettings));
            this.btnSave = new Button();
            this.lblPort = new Label();
            this.ncPort = new NumericUpDown();
            this.chkAutoListen = new CheckBox();
            this.chkPopup = new CheckBox();
            this.btnListen = new Button();
            this.btnCancel = new Button();
            this.chkUseUpnp = new CheckBox();
            this.chkShowTooltip = new CheckBox();
            this.chkNoIPIntegration = new CheckBox();
            this.lblHost = new Label();
            this.lblPass = new Label();
            this.lblUser = new Label();
            this.txtNoIPPass = new TextBox();
            this.txtNoIPUser = new TextBox();
            this.txtNoIPHost = new TextBox();
            this.chkShowPassword = new CheckBox();
            this.chkIPv6Support = new CheckBox();
            this.ncPort.BeginInit();
            this.SuspendLayout();
            this.btnSave.Location = new Point(227, 298);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new Size(75, 23);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "&Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new EventHandler(this.btnSave_Click);
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new Point(12, 11);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new Size(93, 13);
            this.lblPort.TabIndex = 0;
            this.lblPort.Text = "Port to listen on:";
            this.ncPort.Location = new Point(111, 7);
            this.ncPort.Maximum = new Decimal(new int[4]
            {
        65535,
        0,
        0,
        0
            });
            this.ncPort.Minimum = new Decimal(new int[4]
            {
        1,
        0,
        0,
        0
            });
            this.ncPort.Name = "ncPort";
            this.ncPort.Size = new Size(75, 22);
            this.ncPort.TabIndex = 1;
            this.ncPort.Value = new Decimal(new int[4]
            {
        1,
        0,
        0,
        0
            });
            this.chkAutoListen.AutoSize = true;
            this.chkAutoListen.Location = new Point(12, 68);
            this.chkAutoListen.Name = "chkAutoListen";
            this.chkAutoListen.Size = new Size(222, 17);
            this.chkAutoListen.TabIndex = 6;
            this.chkAutoListen.Text = "Listen for new connections on startup";
            this.chkAutoListen.UseVisualStyleBackColor = true;
            this.chkPopup.AutoSize = true;
            this.chkPopup.Location = new Point(12, 91);
            this.chkPopup.Name = "chkPopup";
            this.chkPopup.Size = new Size(259, 17);
            this.chkPopup.TabIndex = 7;
            this.chkPopup.Text = "Show popup notification on new connection";
            this.chkPopup.UseVisualStyleBackColor = true;
            this.btnListen.Location = new Point(192, 6);
            this.btnListen.Name = "btnListen";
            this.btnListen.Size = new Size(110, 23);
            this.btnListen.TabIndex = 2;
            this.btnListen.Text = "Start listening";
            this.btnListen.UseVisualStyleBackColor = true;
            this.btnListen.Click += new EventHandler(this.btnListen_Click);
            this.btnCancel.Location = new Point(146, 298);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size(75, 23);
            this.btnCancel.TabIndex = 18;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
            this.chkUseUpnp.AutoSize = true;
            this.chkUseUpnp.Location = new Point(12, 114);
            this.chkUseUpnp.Name = "chkUseUpnp";
            this.chkUseUpnp.Size = new Size(250, 17);
            this.chkUseUpnp.TabIndex = 8;
            this.chkUseUpnp.Text = "Try to automatically forward the port (UPnP)";
            this.chkUseUpnp.UseVisualStyleBackColor = true;
            this.chkShowTooltip.AutoSize = true;
            this.chkShowTooltip.Location = new Point(12, 137);
            this.chkShowTooltip.Name = "chkShowTooltip";
            this.chkShowTooltip.Size = new Size(268, 17);
            this.chkShowTooltip.TabIndex = 9;
            this.chkShowTooltip.Text = "Show tooltip on client with system information";
            this.chkShowTooltip.UseVisualStyleBackColor = true;
            this.chkNoIPIntegration.AutoSize = true;
            this.chkNoIPIntegration.Location = new Point(12, 177);
            this.chkNoIPIntegration.Name = "chkNoIPIntegration";
            this.chkNoIPIntegration.Size = new Size(187, 17);
            this.chkNoIPIntegration.TabIndex = 10;
            this.chkNoIPIntegration.Text = "Enable No-Ip.com DNS Updater";
            this.chkNoIPIntegration.UseVisualStyleBackColor = true;
            this.chkNoIPIntegration.CheckedChanged += new EventHandler(this.chkNoIPIntegration_CheckedChanged);
            this.lblHost.AutoSize = true;
            this.lblHost.Enabled = false;
            this.lblHost.Location = new Point(30, 203);
            this.lblHost.Name = "lblHost";
            this.lblHost.Size = new Size(34, 13);
            this.lblHost.TabIndex = 11;
            this.lblHost.Text = "Host:";
            this.lblPass.AutoSize = true;
            this.lblPass.Enabled = false;
            this.lblPass.Location = new Point(167, 231);
            this.lblPass.Name = "lblPass";
            this.lblPass.Size = new Size(32, 13);
            this.lblPass.TabIndex = 15;
            this.lblPass.Text = "Pass:";
            this.lblUser.AutoSize = true;
            this.lblUser.Enabled = false;
            this.lblUser.Location = new Point(30, 231);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new Size(32, 13);
            this.lblUser.TabIndex = 13;
            this.lblUser.Text = "Mail:";
            this.txtNoIPPass.Enabled = false;
            this.txtNoIPPass.Location = new Point(199, 228);
            this.txtNoIPPass.Name = "txtNoIPPass";
            this.txtNoIPPass.Size = new Size(100, 22);
            this.txtNoIPPass.TabIndex = 16;
            this.txtNoIPUser.Enabled = false;
            this.txtNoIPUser.Location = new Point(70, 228);
            this.txtNoIPUser.Name = "txtNoIPUser";
            this.txtNoIPUser.Size = new Size(91, 22);
            this.txtNoIPUser.TabIndex = 14;
            this.txtNoIPHost.Enabled = false;
            this.txtNoIPHost.Location = new Point(70, 200);
            this.txtNoIPHost.Name = "txtNoIPHost";
            this.txtNoIPHost.Size = new Size(229, 22);
            this.txtNoIPHost.TabIndex = 12;
            this.chkShowPassword.AutoSize = true;
            this.chkShowPassword.Enabled = false;
            this.chkShowPassword.Location = new Point(192, 256);
            this.chkShowPassword.Name = "chkShowPassword";
            this.chkShowPassword.Size = new Size(107, 17);
            this.chkShowPassword.TabIndex = 17;
            this.chkShowPassword.Text = "Show Password";
            this.chkShowPassword.UseVisualStyleBackColor = true;
            this.chkShowPassword.CheckedChanged += new EventHandler(this.chkShowPassword_CheckedChanged);
            this.chkIPv6Support.AutoSize = true;
            this.chkIPv6Support.Location = new Point(12, 45);
            this.chkIPv6Support.Name = "chkIPv6Support";
            this.chkIPv6Support.Size = new Size(128, 17);
            this.chkIPv6Support.TabIndex = 5;
            this.chkIPv6Support.Text = "Enable IPv6 support";
            this.chkIPv6Support.UseVisualStyleBackColor = true;
            this.AutoScaleDimensions = new SizeF(96f, 96f);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.ClientSize = new Size(314, 333);
            this.Controls.Add((Control)this.chkIPv6Support);
            this.Controls.Add((Control)this.chkShowPassword);
            this.Controls.Add((Control)this.txtNoIPHost);
            this.Controls.Add((Control)this.txtNoIPUser);
            this.Controls.Add((Control)this.txtNoIPPass);
            this.Controls.Add((Control)this.lblUser);
            this.Controls.Add((Control)this.lblPass);
            this.Controls.Add((Control)this.lblHost);
            this.Controls.Add((Control)this.chkNoIPIntegration);
            this.Controls.Add((Control)this.chkShowTooltip);
            this.Controls.Add((Control)this.chkUseUpnp);
            this.Controls.Add((Control)this.btnCancel);
            this.Controls.Add((Control)this.btnListen);
            this.Controls.Add((Control)this.chkPopup);
            this.Controls.Add((Control)this.chkAutoListen);
            this.Controls.Add((Control)this.ncPort);
            this.Controls.Add((Control)this.lblPort);
            this.Controls.Add((Control)this.btnSave);
            this.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.Icon = (Icon)resources.GetObject("$this.Icon");
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmSettings";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.Load += new EventHandler(this.FrmSettings_Load);
            this.ncPort.EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private Button btnSave;
        private Label lblPort;
        private NumericUpDown ncPort;
        private CheckBox chkAutoListen;
        private CheckBox chkPopup;
        private Button btnListen;
        private Button btnCancel;
        private CheckBox chkUseUpnp;
        private CheckBox chkShowTooltip;
        private CheckBox chkNoIPIntegration;
        private Label lblHost;
        private Label lblPass;
        private Label lblUser;
        private TextBox txtNoIPPass;
        private TextBox txtNoIPUser;
        private TextBox txtNoIPHost;
        private CheckBox chkShowPassword;
        private CheckBox chkIPv6Support;
    }
}
