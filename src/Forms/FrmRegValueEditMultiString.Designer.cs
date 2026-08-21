using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace InvokedServer.Forms
{
    partial class FrmRegValueEditMultiString
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
            this.valueNameTxtBox = new TextBox();
            this.label1 = new Label();
            this.label2 = new Label();
            this.cancelButton = new Button();
            this.okButton = new Button();
            this.valueDataTxtBox = new TextBox();
            this.SuspendLayout();
            this.valueNameTxtBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.valueNameTxtBox.Location = new Point(15, 25);
            this.valueNameTxtBox.Name = "valueNameTxtBox";
            this.valueNameTxtBox.ReadOnly = true;
            this.valueNameTxtBox.Size = new Size(346, 20);
            this.valueNameTxtBox.TabIndex = 3;
            this.label1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new Size(66, 13);
            this.label1.Text = "Value name:";
            this.label1.TextAlign = ContentAlignment.MiddleLeft;
            this.label2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new Point(12, 53);
            this.label2.Name = "label2";
            this.label2.Size = new Size(61, 13);
            this.label2.Text = "Value data:";
            this.label2.TextAlign = ContentAlignment.MiddleLeft;
            this.cancelButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.cancelButton.DialogResult = DialogResult.Cancel;
            this.cancelButton.Location = new Point(286, 330);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.okButton.DialogResult = DialogResult.OK;
            this.okButton.Location = new Point(205, 330);
            this.okButton.Name = "okButton";
            this.okButton.Size = new Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new EventHandler(this.okButton_Click);
            this.valueDataTxtBox.AcceptsReturn = true;
            this.valueDataTxtBox.Location = new Point(15, 72);
            this.valueDataTxtBox.Multiline = true;
            this.valueDataTxtBox.Name = "valueDataTxtBox";
            this.valueDataTxtBox.ScrollBars = ScrollBars.Both;
            this.valueDataTxtBox.Size = new Size(346, 252);
            this.valueDataTxtBox.TabIndex = 0;
            this.valueDataTxtBox.WordWrap = false;
            this.AcceptButton = (IButtonControl)this.okButton;
            this.AutoScaleDimensions = new SizeF(96f, 96f);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.CancelButton = (IButtonControl)this.cancelButton;
            this.ClientSize = new Size(373, 365);
            this.Controls.Add((Control)this.cancelButton);
            this.Controls.Add((Control)this.valueDataTxtBox);
            this.Controls.Add((Control)this.okButton);
            this.Controls.Add((Control)this.label2);
            this.Controls.Add((Control)this.valueNameTxtBox);
            this.Controls.Add((Control)this.label1);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmRegValueEditMultiString";
            this.ShowIcon = false;
            this.Text = "Edit Multi-String";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private TextBox valueNameTxtBox;
        private Label label1;
        private Label label2;
        private Button cancelButton;
        private Button okButton;
        private TextBox valueDataTxtBox;
    }
}
