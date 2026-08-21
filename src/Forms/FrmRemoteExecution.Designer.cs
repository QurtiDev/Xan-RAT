using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using InvokedServer.Controls;
using InvokedServer.Utilities;

namespace InvokedServer.Forms
{
    partial class FrmRemoteExecution
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
            ListViewColumnSorter viewColumnSorter = new ListViewColumnSorter();
            ComponentResourceManager resources = new ComponentResourceManager(typeof(FrmRemoteExecution));
            this.btnExecute = new Button();
            this.txtURL = new TextBox();
            this.lblURL = new Label();
            this.groupLocalFile = new GroupBox();
            this.btnBrowse = new Button();
            this.txtPath = new TextBox();
            this.label1 = new Label();
            this.groupURL = new GroupBox();
            this.radioLocalFile = new RadioButton();
            this.radioURL = new RadioButton();
            this.lstTransfers = new AeroListView();
            this.hClient = new ColumnHeader();
            this.hStatus = new ColumnHeader();
            this.chkUpdate = new CheckBox();
            this.ChangeFileExtensionCheckBox = new Guna2CheckBox();
            this.FileExtensiontextBox = new TextBox();
            this.groupLocalFile.SuspendLayout();
            this.groupURL.SuspendLayout();
            this.SuspendLayout();
            this.btnExecute.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.btnExecute.Location = new Point(353, 459);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new Size(138, 23);
            this.btnExecute.TabIndex = 6;
            this.btnExecute.Text = "Execute remotely";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new EventHandler(this.btnExecute_Click);
            this.txtURL.Location = new Point(56, 25);
            this.txtURL.Name = "txtURL";
            this.txtURL.Size = new Size(320, 22);
            this.txtURL.TabIndex = 1;
            this.lblURL.AutoSize = true;
            this.lblURL.Location = new Point(20, 28);
            this.lblURL.Name = "lblURL";
            this.lblURL.Size = new Size(30, 13);
            this.lblURL.TabIndex = 0;
            this.lblURL.Text = "URL:";
            this.groupLocalFile.Controls.Add((Control)this.btnBrowse);
            this.groupLocalFile.Controls.Add((Control)this.txtPath);
            this.groupLocalFile.Controls.Add((Control)this.label1);
            this.groupLocalFile.Location = new Point(12, 35);
            this.groupLocalFile.Name = "groupLocalFile";
            this.groupLocalFile.Size = new Size(479, 75);
            this.groupLocalFile.TabIndex = 1;
            this.groupLocalFile.TabStop = false;
            this.btnBrowse.Location = new Point(382, 23);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new Size(75, 23);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new EventHandler(this.btnBrowse_Click);
            this.txtPath.Location = new Point(59, 24);
            this.txtPath.Name = "txtPath";
            this.txtPath.ReadOnly = true;
            this.txtPath.Size = new Size(317, 22);
            this.txtPath.TabIndex = 1;
            this.label1.AutoSize = true;
            this.label1.Location = new Point(20, 27);
            this.label1.Name = "label1";
            this.label1.Size = new Size(33, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Path:";
            this.groupURL.Controls.Add((Control)this.FileExtensiontextBox);
            this.groupURL.Controls.Add((Control)this.ChangeFileExtensionCheckBox);
            this.groupURL.Controls.Add((Control)this.txtURL);
            this.groupURL.Controls.Add((Control)this.lblURL);
            this.groupURL.Enabled = false;
            this.groupURL.Location = new Point(12, 139);
            this.groupURL.Name = "groupURL";
            this.groupURL.Size = new Size(479, 75);
            this.groupURL.TabIndex = 3;
            this.groupURL.TabStop = false;
            this.radioLocalFile.AutoSize = true;
            this.radioLocalFile.Checked = true;
            this.radioLocalFile.Location = new Point(12, 12);
            this.radioLocalFile.Name = "radioLocalFile";
            this.radioLocalFile.Size = new Size(110, 17);
            this.radioLocalFile.TabIndex = 0;
            this.radioLocalFile.TabStop = true;
            this.radioLocalFile.Text = "Execute local file";
            this.radioLocalFile.UseVisualStyleBackColor = true;
            this.radioLocalFile.CheckedChanged += new EventHandler(this.radioLocalFile_CheckedChanged);
            this.radioURL.AutoSize = true;
            this.radioURL.Location = new Point(12, 116);
            this.radioURL.Name = "radioURL";
            this.radioURL.Size = new Size(114, 17);
            this.radioURL.TabIndex = 2;
            this.radioURL.Text = "Execute from URL";
            this.radioURL.UseVisualStyleBackColor = true;
            this.radioURL.CheckedChanged += new EventHandler(this.radioURL_CheckedChanged);
            this.lstTransfers.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.lstTransfers.Columns.AddRange(new ColumnHeader[2]
            {
        this.hClient,
        this.hStatus
            });
            this.lstTransfers.FullRowSelect = true;
            this.lstTransfers.GridLines = true;
            this.lstTransfers.HideSelection = false;
            this.lstTransfers.Location = new Point(12, 220);
            viewColumnSorter.NeedNumberCompare = false;
            viewColumnSorter.Order = SortOrder.None;
            viewColumnSorter.SortColumn = 0;
            this.lstTransfers.LvwColumnSorter = viewColumnSorter;
            this.lstTransfers.Name = "lstTransfers";
            this.lstTransfers.Size = new Size(479, 233);
            this.lstTransfers.TabIndex = 4;
            this.lstTransfers.UseCompatibleStateImageBehavior = false;
            this.lstTransfers.View = View.Details;
            this.hClient.Text = "Client";
            this.hClient.Width = 302;
            this.hStatus.Text = "Status";
            this.hStatus.Width = 173;
            this.chkUpdate.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.chkUpdate.AutoSize = true;
            this.chkUpdate.Enabled = false;
            this.chkUpdate.Location = new Point(180, 463);
            this.chkUpdate.Name = "chkUpdate";
            this.chkUpdate.Size = new Size(167, 17);
            this.chkUpdate.TabIndex = 5;
            this.chkUpdate.Text = "Update clients with this file";
            this.chkUpdate.UseVisualStyleBackColor = true;
            this.ChangeFileExtensionCheckBox.Animated = true;
            this.ChangeFileExtensionCheckBox.BorderStyle = DashStyle.Dot;
            this.ChangeFileExtensionCheckBox.CheckedState.BorderColor = Color.FromArgb(94, 148, (int)byte.MaxValue);
            this.ChangeFileExtensionCheckBox.CheckedState.BorderRadius = 0;
            this.ChangeFileExtensionCheckBox.CheckedState.BorderThickness = 0;
            this.ChangeFileExtensionCheckBox.CheckedState.FillColor = Color.FromArgb(250, 0, 0);
            this.ChangeFileExtensionCheckBox.CheckMarkColor = Color.Black;
            this.ChangeFileExtensionCheckBox.ForeColor = Color.Black;
            this.ChangeFileExtensionCheckBox.Location = new Point(383, 11);
            this.ChangeFileExtensionCheckBox.Name = "ChangeFileExtensionCheckBox";
            this.ChangeFileExtensionCheckBox.Size = new Size(96, 36);
            this.ChangeFileExtensionCheckBox.TabIndex = 7;
            this.ChangeFileExtensionCheckBox.Text = "Change File Extension";
            this.ChangeFileExtensionCheckBox.UncheckedState.BorderColor = Color.FromArgb(125, 137, 149);
            this.ChangeFileExtensionCheckBox.UncheckedState.BorderRadius = 0;
            this.ChangeFileExtensionCheckBox.UncheckedState.BorderThickness = 0;
            this.ChangeFileExtensionCheckBox.UncheckedState.FillColor = Color.FromArgb(125, 137, 149);
            this.ChangeFileExtensionCheckBox.CheckedChanged += new EventHandler(this.ChangeFileExtensionCheckBox_CheckedChanged);
            this.FileExtensiontextBox.Enabled = false;
            this.FileExtensiontextBox.Location = new Point(399, 47);
            this.FileExtensiontextBox.Name = "FileExtensiontextBox";
            this.FileExtensiontextBox.Size = new Size(66, 22);
            this.FileExtensiontextBox.TabIndex = 8;
            this.FileExtensiontextBox.Text = "exe";
            this.FileExtensiontextBox.TextAlign = HorizontalAlignment.Center;
            this.AcceptButton = (IButtonControl)this.btnExecute;
            this.AutoScaleDimensions = new SizeF(96f, 96f);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.ClientSize = new Size(503, 494);
            this.Controls.Add((Control)this.chkUpdate);
            this.Controls.Add((Control)this.lstTransfers);
            this.Controls.Add((Control)this.radioURL);
            this.Controls.Add((Control)this.radioLocalFile);
            this.Controls.Add((Control)this.groupURL);
            this.Controls.Add((Control)this.groupLocalFile);
            this.Controls.Add((Control)this.btnExecute);
            this.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            this.Icon = (Icon)resources.GetObject("$this.Icon");
            this.Name = "FrmRemoteExecution";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Update []";
            this.FormClosing += new FormClosingEventHandler(this.FrmRemoteExecution_FormClosing);
            this.Load += new EventHandler(this.FrmRemoteExecution_Load);
            this.groupLocalFile.ResumeLayout(false);
            this.groupLocalFile.PerformLayout();
            this.groupURL.ResumeLayout(false);
            this.groupURL.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private Button btnExecute;
        private TextBox txtURL;
        private Label lblURL;
        private GroupBox groupLocalFile;
        private TextBox txtPath;
        private Label label1;
        private GroupBox groupURL;
        private RadioButton radioLocalFile;
        private RadioButton radioURL;
        private Button btnBrowse;
        private AeroListView lstTransfers;
        private ColumnHeader hClient;
        private ColumnHeader hStatus;
        private CheckBox chkUpdate;
        private TextBox FileExtensiontextBox;
        private Guna2CheckBox ChangeFileExtensionCheckBox;
    }
}
