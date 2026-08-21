using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using InvokedServer.Properties;

namespace InvokedServer.Forms
{
    partial class FrmHVNCBrowserOptions
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
            this.cbBrowsers = new Guna2ComboBox();
            this.StartProgramBtn = new Guna2GradientButton();
            this.CloneProfileCheckBox = new Guna2CheckBox();
            this.label1 = new Label();
            this.SuspendLayout();
            this.cbBrowsers.BackColor = Color.Transparent;
            this.cbBrowsers.BorderColor = Color.FromArgb(72, 82, 98);
            this.cbBrowsers.BorderRadius = 2;
            this.cbBrowsers.DrawMode = DrawMode.OwnerDrawFixed;
            this.cbBrowsers.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cbBrowsers.FillColor = Color.FromArgb(48, 51, 66);
            this.cbBrowsers.FocusedColor = Color.Empty;
            this.cbBrowsers.Font = new Font("Segoe UI", 8.25f);
            this.cbBrowsers.ForeColor = Color.White;
            this.cbBrowsers.ItemHeight = 18;
            this.cbBrowsers.Items.AddRange(new object[6]
            {
        (object) "Chrome",
        (object) "Firefox",
        (object) "Edge",
        (object) "Brave",
        (object) "Opera",
        (object) "OperaGX"
            });
            this.cbBrowsers.Location = new Point(66, 71);
            this.cbBrowsers.Margin = new Padding(0);
            this.cbBrowsers.Name = "cbBrowsers";
            this.cbBrowsers.Size = new Size(142, 24);
            this.cbBrowsers.TabIndex = 39;
            this.cbBrowsers.SelectedIndexChanged += new EventHandler(this.cbBrowsers_SelectedIndexChanged);
            this.StartProgramBtn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.StartProgramBtn.Animated = true;
            this.StartProgramBtn.BackColor = Color.FromArgb(62, 72, 88);
            this.StartProgramBtn.BorderColor = Color.FromArgb(72, 82, 98);
            this.StartProgramBtn.BorderRadius = 2;
            this.StartProgramBtn.BorderThickness = 1;
            this.StartProgramBtn.DisabledState.BorderColor = Color.DarkGray;
            this.StartProgramBtn.DisabledState.CustomBorderColor = Color.DarkGray;
            this.StartProgramBtn.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            this.StartProgramBtn.DisabledState.FillColor2 = Color.FromArgb(169, 169, 169);
            this.StartProgramBtn.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            this.StartProgramBtn.Enabled = false;
            this.StartProgramBtn.FillColor = Color.FromArgb(35, 35, 35);
            this.StartProgramBtn.FillColor2 = Color.FromArgb(62, 72, 88);
            this.StartProgramBtn.Font = new Font("Segoe UI", 8.25f);
            this.StartProgramBtn.ForeColor = Color.White;
            this.StartProgramBtn.GradientMode = LinearGradientMode.ForwardDiagonal;
            this.StartProgramBtn.Image = (Image)Resources.HiddenProgram_161;
            this.StartProgramBtn.ImageSize = new Size(16, 16);
            this.StartProgramBtn.Location = new Point(241, 72);
            this.StartProgramBtn.Name = "StartProgramBtn";
            this.StartProgramBtn.Size = new Size(63, 23);
            this.StartProgramBtn.TabIndex = 40;
            this.StartProgramBtn.Text = "Start";
            this.StartProgramBtn.Click += new EventHandler(this.StartProgramBtn_Click);
            this.CloneProfileCheckBox.Animated = true;
            this.CloneProfileCheckBox.AutoSize = true;
            this.CloneProfileCheckBox.BorderStyle = DashStyle.Dot;
            this.CloneProfileCheckBox.Checked = true;
            this.CloneProfileCheckBox.CheckedState.BorderColor = Color.FromArgb(94, 148, (int)byte.MaxValue);
            this.CloneProfileCheckBox.CheckedState.BorderRadius = 0;
            this.CloneProfileCheckBox.CheckedState.BorderThickness = 0;
            this.CloneProfileCheckBox.CheckedState.FillColor = Color.FromArgb(250, 0, 0);
            this.CloneProfileCheckBox.CheckMarkColor = Color.Black;
            this.CloneProfileCheckBox.CheckState = CheckState.Checked;
            this.CloneProfileCheckBox.ForeColor = Color.White;
            this.CloneProfileCheckBox.Location = new Point(81, 35);
            this.CloneProfileCheckBox.Name = "CloneProfileCheckBox";
            this.CloneProfileCheckBox.Size = new Size(85, 17);
            this.CloneProfileCheckBox.TabIndex = 41;
            this.CloneProfileCheckBox.Text = "Clone Profile";
            this.CloneProfileCheckBox.UncheckedState.BorderColor = Color.FromArgb(125, 137, 149);
            this.CloneProfileCheckBox.UncheckedState.BorderRadius = 0;
            this.CloneProfileCheckBox.UncheckedState.BorderThickness = 0;
            this.CloneProfileCheckBox.UncheckedState.FillColor = Color.FromArgb(125, 137, 149);
            this.label1.AutoSize = true;
            this.label1.ForeColor = Color.Red;
            this.label1.Location = new Point(26, 13);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0, 13);
            this.label1.TabIndex = 42;
            this.AutoScaleDimensions = new SizeF(6f, 13f);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(35, 35, 35);
            this.ClientSize = new Size(415, 232);
            this.Controls.Add((Control)this.label1);
            this.Controls.Add((Control)this.CloneProfileCheckBox);
            this.Controls.Add((Control)this.StartProgramBtn);
            this.Controls.Add((Control)this.cbBrowsers);
            this.Name = "FrmHVNCBrowserOptions";
            this.Text = "FrmHVNCBrowserOptions";
            this.Load += new EventHandler(this.FrmHVNCBrowserOptions_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private Guna2ComboBox cbBrowsers;
        private Guna2GradientButton StartProgramBtn;
        private Guna2CheckBox CloneProfileCheckBox;
        private Label label1;
    }
}
