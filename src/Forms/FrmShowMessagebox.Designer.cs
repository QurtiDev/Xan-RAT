using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace InvokedServer.Forms
{
    partial class FrmShowMessagebox
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
            ComponentResourceManager resources = new ComponentResourceManager(typeof(FrmShowMessagebox));
            this.groupMsgSettings = new GroupBox();
            this.cmbMsgIcon = new ComboBox();
            this.lblMsgIcon = new Label();
            this.cmbMsgButtons = new ComboBox();
            this.lblMsgButtons = new Label();
            this.txtText = new TextBox();
            this.txtCaption = new TextBox();
            this.lblText = new Label();
            this.lblCaption = new Label();
            this.btnPreview = new Button();
            this.btnSend = new Button();
            this.groupMsgSettings.SuspendLayout();
            this.SuspendLayout();
            this.groupMsgSettings.Controls.Add((Control)this.cmbMsgIcon);
            this.groupMsgSettings.Controls.Add((Control)this.lblMsgIcon);
            this.groupMsgSettings.Controls.Add((Control)this.cmbMsgButtons);
            this.groupMsgSettings.Controls.Add((Control)this.lblMsgButtons);
            this.groupMsgSettings.Controls.Add((Control)this.txtText);
            this.groupMsgSettings.Controls.Add((Control)this.txtCaption);
            this.groupMsgSettings.Controls.Add((Control)this.lblText);
            this.groupMsgSettings.Controls.Add((Control)this.lblCaption);
            this.groupMsgSettings.Location = new Point(12, 12);
            this.groupMsgSettings.Name = "groupMsgSettings";
            this.groupMsgSettings.Size = new Size(325, 146);
            this.groupMsgSettings.TabIndex = 0;
            this.groupMsgSettings.TabStop = false;
            this.groupMsgSettings.Text = "Messagebox Settings";
            this.cmbMsgIcon.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbMsgIcon.FormattingEnabled = true;
            this.cmbMsgIcon.Location = new Point(147, 107);
            this.cmbMsgIcon.Name = "cmbMsgIcon";
            this.cmbMsgIcon.Size = new Size(162, 21);
            this.cmbMsgIcon.TabIndex = 8;
            this.lblMsgIcon.AutoSize = true;
            this.lblMsgIcon.Location = new Point(42, 110);
            this.lblMsgIcon.Name = "lblMsgIcon";
            this.lblMsgIcon.Size = new Size(99, 13);
            this.lblMsgIcon.TabIndex = 7;
            this.lblMsgIcon.Text = "Messagebox Icon:";
            this.cmbMsgButtons.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbMsgButtons.FormattingEnabled = true;
            this.cmbMsgButtons.Location = new Point(147, 80);
            this.cmbMsgButtons.Name = "cmbMsgButtons";
            this.cmbMsgButtons.Size = new Size(162, 21);
            this.cmbMsgButtons.TabIndex = 6;
            this.lblMsgButtons.AutoSize = true;
            this.lblMsgButtons.Location = new Point(23, 83);
            this.lblMsgButtons.Name = "lblMsgButtons";
            this.lblMsgButtons.Size = new Size(117, 13);
            this.lblMsgButtons.TabIndex = 5;
            this.lblMsgButtons.Text = "Messagebox Buttons:";
            this.txtText.Location = new Point(60, 49);
            this.txtText.MaxLength = 256;
            this.txtText.Name = "txtText";
            this.txtText.Size = new Size(249, 22);
            this.txtText.TabIndex = 4;
            this.txtText.Text = "You are running Quasar.";
            this.txtCaption.Location = new Point(60, 21);
            this.txtCaption.MaxLength = 256;
            this.txtCaption.Name = "txtCaption";
            this.txtCaption.Size = new Size(249, 22);
            this.txtCaption.TabIndex = 2;
            this.txtCaption.Text = "Information";
            this.lblText.AutoSize = true;
            this.lblText.Location = new Point(24, 52);
            this.lblText.Name = "lblText";
            this.lblText.Size = new Size(30, 13);
            this.lblText.TabIndex = 3;
            this.lblText.Text = "Text:";
            this.lblCaption.AutoSize = true;
            this.lblCaption.Location = new Point(6, 24);
            this.lblCaption.Name = "lblCaption";
            this.lblCaption.Size = new Size(51, 13);
            this.lblCaption.TabIndex = 1;
            this.lblCaption.Text = "Caption:";
            this.btnPreview.Location = new Point(181, 164);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new Size(75, 23);
            this.btnPreview.TabIndex = 1;
            this.btnPreview.Text = "Preview";
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new EventHandler(this.btnPreview_Click);
            this.btnSend.Location = new Point(262, 164);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new Size(75, 23);
            this.btnSend.TabIndex = 2;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new EventHandler(this.btnSend_Click);
            this.AutoScaleDimensions = new SizeF(96f, 96f);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.ClientSize = new Size(349, 199);
            this.Controls.Add((Control)this.btnSend);
            this.Controls.Add((Control)this.btnPreview);
            this.Controls.Add((Control)this.groupMsgSettings);
            this.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.Icon = (Icon)resources.GetObject("$this.Icon");
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmShowMessagebox";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Show Messagebox []";
            this.Load += new EventHandler(this.FrmShowMessagebox_Load);
            this.groupMsgSettings.ResumeLayout(false);
            this.groupMsgSettings.PerformLayout();
            this.ResumeLayout(false);
        }

        private GroupBox groupMsgSettings;
        private ComboBox cmbMsgIcon;
        private Label lblMsgIcon;
        private ComboBox cmbMsgButtons;
        private Label lblMsgButtons;
        private TextBox txtText;
        private TextBox txtCaption;
        private Label lblText;
        private Label lblCaption;
        private Button btnPreview;
        private Button btnSend;
    }
}
