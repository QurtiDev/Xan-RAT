using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace InvokedServer.Forms
{
    partial class FrmStartupAdd
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
            ComponentResourceManager resources = new ComponentResourceManager(typeof(FrmStartupAdd));
            this.groupAutostartItem = new GroupBox();
            this.lblType = new Label();
            this.cmbType = new ComboBox();
            this.txtPath = new TextBox();
            this.txtName = new TextBox();
            this.lblPath = new Label();
            this.lblName = new Label();
            this.btnAdd = new Button();
            this.btnCancel = new Button();
            this.toolTip1 = new ToolTip(this.components);
            this.groupAutostartItem.SuspendLayout();
            this.SuspendLayout();
            this.groupAutostartItem.Controls.Add((Control)this.lblType);
            this.groupAutostartItem.Controls.Add((Control)this.cmbType);
            this.groupAutostartItem.Controls.Add((Control)this.txtPath);
            this.groupAutostartItem.Controls.Add((Control)this.txtName);
            this.groupAutostartItem.Controls.Add((Control)this.lblPath);
            this.groupAutostartItem.Controls.Add((Control)this.lblName);
            this.groupAutostartItem.Location = new Point(12, 12);
            this.groupAutostartItem.Name = "groupAutostartItem";
            this.groupAutostartItem.Size = new Size(653, 105);
            this.groupAutostartItem.TabIndex = 0;
            this.groupAutostartItem.TabStop = false;
            this.groupAutostartItem.Text = "Autostart Item";
            this.lblType.AutoSize = true;
            this.lblType.Location = new Point(35, 74);
            this.lblType.Name = "lblType";
            this.lblType.Size = new Size(33, 13);
            this.lblType.TabIndex = 4;
            this.lblType.Text = "Type:";
            this.cmbType.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Location = new Point(74, 71);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new Size(573, 21);
            this.cmbType.TabIndex = 5;
            this.toolTip1.SetToolTip((Control)this.cmbType, "Remote Type of Autostart Item.");
            this.txtPath.Location = new Point(74, 43);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new Size(573, 22);
            this.txtPath.TabIndex = 3;
            this.toolTip1.SetToolTip((Control)this.txtPath, "Remote Path to Autostart Item.");
            this.txtPath.KeyPress += new KeyPressEventHandler(this.txtPath_KeyPress);
            this.txtName.Location = new Point(74, 15);
            this.txtName.MaxLength = 64;
            this.txtName.Name = "txtName";
            this.txtName.Size = new Size(573, 22);
            this.txtName.TabIndex = 1;
            this.txtName.KeyPress += new KeyPressEventHandler(this.txtName_KeyPress);
            this.lblPath.AutoSize = true;
            this.lblPath.Location = new Point(35, 46);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new Size(33, 13);
            this.lblPath.TabIndex = 2;
            this.lblPath.Text = "Path:";
            this.lblName.AutoSize = true;
            this.lblName.Location = new Point(29, 18);
            this.lblName.Name = "lblName";
            this.lblName.Size = new Size(39, 13);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Name:";
            this.btnAdd.DialogResult = DialogResult.Cancel;
            this.btnAdd.Location = new Point(576, 123);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new Size(89, 23);
            this.btnAdd.TabIndex = 1;
            this.btnAdd.Text = "&Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new EventHandler(this.btnAdd_Click);
            this.btnCancel.DialogResult = DialogResult.Cancel;
            this.btnCancel.Location = new Point(449, 123);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size(89, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
            this.AcceptButton = (IButtonControl)this.btnAdd;
            this.AutoScaleDimensions = new SizeF(96f, 96f);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.CancelButton = (IButtonControl)this.btnCancel;
            this.ClientSize = new Size(677, 158);
            this.Controls.Add((Control)this.btnCancel);
            this.Controls.Add((Control)this.btnAdd);
            this.Controls.Add((Control)this.groupAutostartItem);
            this.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.Icon = (Icon)resources.GetObject("$this.Icon");
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmStartupAdd";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Add to Autostart";
            this.groupAutostartItem.ResumeLayout(false);
            this.groupAutostartItem.PerformLayout();
            this.ResumeLayout(false);
        }

        private GroupBox groupAutostartItem;
        private TextBox txtPath;
        private TextBox txtName;
        private Label lblPath;
        private Label lblName;
        private ComboBox cmbType;
        private Label lblType;
        private Button btnAdd;
        private Button btnCancel;
        private ToolTip toolTip1;
    }
}
