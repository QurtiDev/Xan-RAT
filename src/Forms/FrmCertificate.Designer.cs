using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace InvokedServer.Forms
{
    partial class FrmCertificate
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
            ComponentResourceManager resources = new ComponentResourceManager(typeof(FrmCertificate));
            this.lblInfo = new Label();
            this.btnCreate = new Button();
            this.lblDescription = new Label();
            this.txtDetails = new TextBox();
            this.btnImport = new Button();
            this.btnSave = new Button();
            this.label1 = new Label();
            this.btnExit = new Button();
            this.SuspendLayout();
            this.lblInfo.AutoSize = true;
            this.lblInfo.Location = new Point(12, 53);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new Size(130, 13);
            this.lblInfo.TabIndex = 3;
            this.lblInfo.Text = "(this might take a while)";
            this.btnCreate.Location = new Point(12, 27);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new Size(75, 23);
            this.btnCreate.TabIndex = 1;
            this.btnCreate.Text = "Create";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new EventHandler(this.btnCreate_Click);
            this.lblDescription.AutoSize = true;
            this.lblDescription.Font = new Font("Segoe UI", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            this.lblDescription.Location = new Point(9, 9);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new Size(490, 15);
            this.lblDescription.TabIndex = 0;
            this.lblDescription.Text = "To use Quasar create a new certificate or import an existing one from a previous installation.";
            this.txtDetails.Location = new Point(12, 69);
            this.txtDetails.Multiline = true;
            this.txtDetails.Name = "txtDetails";
            this.txtDetails.ReadOnly = true;
            this.txtDetails.ScrollBars = ScrollBars.Vertical;
            this.txtDetails.Size = new Size(517, 230);
            this.txtDetails.TabIndex = 4;
            this.btnImport.Location = new Point(93, 27);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new Size(130, 23);
            this.btnImport.TabIndex = 2;
            this.btnImport.Text = "Browse && Import";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new EventHandler(this.btnImport_Click);
            this.btnSave.Enabled = false;
            this.btnSave.Location = new Point(373, 305);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new Size(75, 23);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new EventHandler(this.btnSave_Click);
            this.label1.AutoSize = true;
            this.label1.Font = new Font("Segoe UI", 9f, FontStyle.Bold, GraphicsUnit.Point, (byte)0);
            this.label1.Location = new Point(12, 310);
            this.label1.Name = "label1";
            this.label1.Size = new Size(355, 15);
            this.label1.TabIndex = 5;
            this.label1.Text = "KEEP THIS FILE SAFE! LOSS RESULTS IN LOOSING ALL CLIENTS!";
            this.btnExit.Location = new Point(454, 306);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new Size(75, 23);
            this.btnExit.TabIndex = 7;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new EventHandler(this.btnExit_Click);
            this.AutoScaleDimensions = new SizeF(96f, 96f);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.ClientSize = new Size(541, 341);
            this.Controls.Add((Control)this.btnExit);
            this.Controls.Add((Control)this.label1);
            this.Controls.Add((Control)this.btnSave);
            this.Controls.Add((Control)this.lblDescription);
            this.Controls.Add((Control)this.txtDetails);
            this.Controls.Add((Control)this.lblInfo);
            this.Controls.Add((Control)this.btnImport);
            this.Controls.Add((Control)this.btnCreate);
            this.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.Icon = (Icon)resources.GetObject("$this.Icon");
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmCertificate";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "InvokedAccess - Certificate Wizard";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private Label lblDescription;
        private Label lblInfo;
        private Button btnCreate;
        private TextBox txtDetails;
        private Button btnImport;
        private Button btnSave;
        private Label label1;
        private Button btnExit;
    }
}
