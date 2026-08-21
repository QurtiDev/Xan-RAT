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
    partial class FrmVisitWebsite
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
            ComponentResourceManager resources = new ComponentResourceManager(typeof(FrmVisitWebsite));
            this.chkVisitHidden = new CheckBox();
            this.lblURL = new Label();
            this.txtURL = new TextBox();
            this.btnVisitWebsite = new Button();
            this.SuspendLayout();
            this.chkVisitHidden.AutoSize = true;
            this.chkVisitHidden.Checked = true;
            this.chkVisitHidden.CheckState = CheckState.Checked;
            this.chkVisitHidden.Location = new Point(48, 38);
            this.chkVisitHidden.Name = "chkVisitHidden";
            this.chkVisitHidden.Size = new Size(170, 17);
            this.chkVisitHidden.TabIndex = 2;
            this.chkVisitHidden.Text = "Visit hidden (recommended)";
            this.chkVisitHidden.UseVisualStyleBackColor = true;
            this.lblURL.AutoSize = true;
            this.lblURL.Location = new Point(12, 9);
            this.lblURL.Name = "lblURL";
            this.lblURL.Size = new Size(30, 13);
            this.lblURL.TabIndex = 0;
            this.lblURL.Text = "URL:";
            this.txtURL.Location = new Point(48, 6);
            this.txtURL.Name = "txtURL";
            this.txtURL.Size = new Size(336, 22);
            this.txtURL.TabIndex = 1;
            this.btnVisitWebsite.Location = new Point(246, 34);
            this.btnVisitWebsite.Name = "btnVisitWebsite";
            this.btnVisitWebsite.Size = new Size(138, 23);
            this.btnVisitWebsite.TabIndex = 3;
            this.btnVisitWebsite.Text = "Visit Website";
            this.btnVisitWebsite.UseVisualStyleBackColor = true;
            this.btnVisitWebsite.Click += new EventHandler(this.btnVisitWebsite_Click);
            this.AcceptButton = (IButtonControl)this.btnVisitWebsite;
            this.AutoScaleDimensions = new SizeF(96f, 96f);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.ClientSize = new Size(396, 72);
            this.Controls.Add((Control)this.chkVisitHidden);
            this.Controls.Add((Control)this.lblURL);
            this.Controls.Add((Control)this.txtURL);
            this.Controls.Add((Control)this.btnVisitWebsite);
            this.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.Icon = (Icon)resources.GetObject("$this.Icon");
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmVisitWebsite";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Visit Website []";
            this.Load += new EventHandler(this.FrmVisitWebsite_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private CheckBox chkVisitHidden;
        private Label lblURL;
        private TextBox txtURL;
        private Button btnVisitWebsite;
    }
}
