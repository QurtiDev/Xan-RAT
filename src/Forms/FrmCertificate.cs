

using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using InvokedServer.Helper;
using InvokedServer.Models;

namespace InvokedServer.Forms
{
	public partial class FrmCertificate : Form
	{
		private X509Certificate2 _certificate;

		public FrmCertificate() => this.InitializeComponent();

		private void SetCertificate(X509Certificate2 certificate)
		{
			this._certificate = certificate;
			this.txtDetails.Text = this._certificate.ToString(false);
			this.btnSave.Enabled = true;
		}

		private void btnCreate_Click(object sender, EventArgs e)
		{
			this.SetCertificate(CertificateHelper.CreateCertificateAuthority("JOYUS", 8192));
		}

		private void btnImport_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog openFileDialog = new OpenFileDialog())
			{
				openFileDialog.CheckFileExists = true;
				openFileDialog.Filter = "*.p12|*.p12";
				openFileDialog.Multiselect = false;
				openFileDialog.InitialDirectory = Application.StartupPath;
				if (openFileDialog.ShowDialog((IWin32Window)this) != DialogResult.OK)
					return;
				try
				{
					this.SetCertificate(new X509Certificate2(openFileDialog.FileName, "", X509KeyStorageFlags.Exportable));
				}
				catch (Exception ex)
				{
					int num = (int)MessageBox.Show((IWin32Window)this, "Error importing the certificate:\n" + ex.Message, "Save error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				}
			}
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			try
			{
				if (this._certificate == null)
					throw new ArgumentNullException();
				if (!this._certificate.HasPrivateKey)
					throw new ArgumentException();
				File.WriteAllBytes(Settings.CertificatePath, this._certificate.Export(X509ContentType.Pfx));
				int num = (int)MessageBox.Show((IWin32Window)this, "Please backup the certificate now. Loss of the certificate results in loosing all clients!", "Certificate backup", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				Process.Start("explorer.exe", "/select, \"" + Settings.CertificatePath + "\"");
				this.DialogResult = DialogResult.OK;
			}
			catch (ArgumentNullException)
			{
				int num = (int)MessageBox.Show((IWin32Window)this, "Please create or import a certificate first.", "Save error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			catch (ArgumentException)
			{
				int num = (int)MessageBox.Show((IWin32Window)this, "The imported certificate has no associated private key. Please import a different certificate.", "Save error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			catch (Exception)
			{
				int num = (int)MessageBox.Show((IWin32Window)this, "There was an error saving the certificate, please make sure you have write access to the InvokedAccess directory.", "Save error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}

		private void btnExit_Click(object sender, EventArgs e) => Environment.Exit(0);
	}
}