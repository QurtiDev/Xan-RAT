using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using InvokedServer.Controls;
using InvokedServer.Enums;

namespace InvokedServer.Forms
{
    partial class FrmRegValueEditWord
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
            ComponentResourceManager resources = new ComponentResourceManager(typeof(FrmRegValueEditWord));
            this.valueNameTxtBox = new TextBox();
            this.label1 = new Label();
            this.label2 = new Label();
            this.cancelButton = new Button();
            this.okButton = new Button();
            this.baseBox = new GroupBox();
            this.radioDecimal = new RadioButton();
            this.radioHexa = new RadioButton();
            this.valueDataTxtBox = new WordTextBox();
            this.baseBox.SuspendLayout();
            this.SuspendLayout();
            this.valueNameTxtBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.valueNameTxtBox.Location = new Point(12, 27);
            this.valueNameTxtBox.Name = "valueNameTxtBox";
            this.valueNameTxtBox.ReadOnly = true;
            this.valueNameTxtBox.Size = new Size(334, 20);
            this.valueNameTxtBox.TabIndex = 5;
            this.label1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new Point(9, 11);
            this.label1.Name = "label1";
            this.label1.Size = new Size(66, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Value name:";
            this.label1.TextAlign = ContentAlignment.MiddleLeft;
            this.label2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new Point(9, 53);
            this.label2.Name = "label2";
            this.label2.Size = new Size(61, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Value data:";
            this.label2.TextAlign = ContentAlignment.MiddleLeft;
            this.cancelButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.cancelButton.DialogResult = DialogResult.Cancel;
            this.cancelButton.Location = new Point(271, 128);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.okButton.DialogResult = DialogResult.OK;
            this.okButton.Location = new Point(190, 128);
            this.okButton.Name = "okButton";
            this.okButton.Size = new Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new EventHandler(this.okButton_Click);
            this.baseBox.Controls.Add((Control)this.radioDecimal);
            this.baseBox.Controls.Add((Control)this.radioHexa);
            this.baseBox.Location = new Point(190, 53);
            this.baseBox.Name = "baseBox";
            this.baseBox.Size = new Size(156, 63);
            this.baseBox.TabIndex = 6;
            this.baseBox.TabStop = false;
            this.baseBox.Text = "Base";
            this.radioDecimal.AutoSize = true;
            this.radioDecimal.Location = new Point(14, 40);
            this.radioDecimal.Name = "radioDecimal";
            this.radioDecimal.Size = new Size(63, 17);
            this.radioDecimal.TabIndex = 4;
            this.radioDecimal.Text = "Decimal";
            this.radioDecimal.UseVisualStyleBackColor = true;
            this.radioHexa.AutoSize = true;
            this.radioHexa.Checked = true;
            this.radioHexa.Location = new Point(14, 17);
            this.radioHexa.Name = "radioHexa";
            this.radioHexa.Size = new Size(86, 17);
            this.radioHexa.TabIndex = 3;
            this.radioHexa.TabStop = true;
            this.radioHexa.Text = "Hexadecimal";
            this.radioHexa.UseVisualStyleBackColor = true;
            this.radioHexa.CheckedChanged += new EventHandler(this.radioHex_CheckboxChanged);
            this.valueDataTxtBox.IsHexNumber = true;
            this.valueDataTxtBox.Location = new Point(12, 70);
            this.valueDataTxtBox.MaxLength = 8;
            this.valueDataTxtBox.Name = "valueDataTxtBox";
            this.valueDataTxtBox.Size = new Size(161, 20);
            this.valueDataTxtBox.TabIndex = 0;
            this.valueDataTxtBox.Text = "0";
            this.valueDataTxtBox.Type = WordType.DWORD;
            this.AcceptButton = (IButtonControl)this.okButton;
            this.AutoScaleDimensions = new SizeF(96f, 96f);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.CancelButton = (IButtonControl)this.cancelButton;
            this.ClientSize = new Size(358, 163);
            this.Controls.Add((Control)this.valueDataTxtBox);
            this.Controls.Add((Control)this.cancelButton);
            this.Controls.Add((Control)this.baseBox);
            this.Controls.Add((Control)this.okButton);
            this.Controls.Add((Control)this.label2);
            this.Controls.Add((Control)this.valueNameTxtBox);
            this.Controls.Add((Control)this.label1);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.Icon = (Icon)resources.GetObject("$this.Icon");
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmRegValueEditWord";
            this.ShowIcon = false;
            this.Text = "Edit";
            this.baseBox.ResumeLayout(false);
            this.baseBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private TextBox valueNameTxtBox;
        private Label label1;
        private Label label2;
        private Button cancelButton;
        private Button okButton;
        private GroupBox baseBox;
        private RadioButton radioDecimal;
        private RadioButton radioHexa;
        private WordTextBox valueDataTxtBox;
    }
}
