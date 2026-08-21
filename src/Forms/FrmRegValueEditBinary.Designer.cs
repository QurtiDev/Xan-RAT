using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace InvokedServer.Forms
{
    partial class FrmRegValueEditBinary
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
            this.hexEditor = new InvokedServer.Controls.HexEditor.HexEditor();
            this.SuspendLayout();
            this.valueNameTxtBox.Anchor = AnchorStyles.Left;
            this.valueNameTxtBox.Location = new Point(12, 31);
            this.valueNameTxtBox.Name = "valueNameTxtBox";
            this.valueNameTxtBox.ReadOnly = true;
            this.valueNameTxtBox.Size = new Size(341, 20);
            this.valueNameTxtBox.TabIndex = 3;
            this.label1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new Point(9, 15);
            this.label1.Name = "label1";
            this.label1.Size = new Size(66, 13);
            this.label1.Text = "Value name:";
            this.label1.TextAlign = ContentAlignment.MiddleLeft;
            this.label2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new Point(9, 54);
            this.label2.Name = "label2";
            this.label2.Size = new Size(61, 13);
            this.label2.Text = "Value data:";
            this.label2.TextAlign = ContentAlignment.MiddleLeft;
            this.cancelButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.cancelButton.DialogResult = DialogResult.Cancel;
            this.cancelButton.Location = new Point(278, 273);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.okButton.DialogResult = DialogResult.OK;
            this.okButton.Location = new Point(197, 273);
            this.okButton.Name = "okButton";
            this.okButton.Size = new Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new EventHandler(this.okButton_Click);
            this.hexEditor.BackColor = Color.White;
            this.hexEditor.BorderColor = Color.Empty;
            this.hexEditor.Cursor = Cursors.IBeam;
            this.hexEditor.EntityMargin = 8;
            this.hexEditor.Font = new Font("Consolas", 9.75f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            this.hexEditor.Location = new Point(12, 71);
            this.hexEditor.Margin = new Padding(0, 2, 3, 3);
            this.hexEditor.Name = "hexEditor";
            this.hexEditor.Size = new Size(341, 196);
            this.hexEditor.TabIndex = 0;
            this.hexEditor.VScrollBarVisisble = true;
            this.AcceptButton = (IButtonControl)this.okButton;
            this.AutoScaleDimensions = new SizeF(96f, 96f);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.CancelButton = (IButtonControl)this.cancelButton;
            this.ClientSize = new Size(365, 304);
            this.Controls.Add((Control)this.cancelButton);
            this.Controls.Add((Control)this.hexEditor);
            this.Controls.Add((Control)this.okButton);
            this.Controls.Add((Control)this.label2);
            this.Controls.Add((Control)this.valueNameTxtBox);
            this.Controls.Add((Control)this.label1);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmRegValueEditBinary";
            this.ShowIcon = false;
            this.Text = "Edit Binary";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private TextBox valueNameTxtBox;
        private Label label1;
        private Label label2;
        private Button cancelButton;
        private Button okButton;
        private InvokedServer.Controls.HexEditor.HexEditor hexEditor;
    }
}
