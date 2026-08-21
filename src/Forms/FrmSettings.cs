

using InvokedServer.Models;
using InvokedServer.Networking;
using InvokedServer.Utilities;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Net.Sockets;
using System.Windows.Forms;


namespace InvokedServer.Forms
{
	public partial class FrmSettings : Form
	{
		private readonly QuasarServer _listenServer;

		public FrmSettings(QuasarServer listenServer)
		{
			this._listenServer = listenServer;
			this.InitializeComponent();
			this.ToggleListenerSettings(!listenServer.Listening);
			this.ShowPassword(false);
		}

		private void FrmSettings_Load(object sender, EventArgs e)
		{
			this.ncPort.Value = (Decimal)Settings.ListenPort;
			this.chkIPv6Support.Checked = Settings.IPv6Support;
			this.chkAutoListen.Checked = Settings.AutoListen;
			this.chkPopup.Checked = Settings.ShowPopup;
			this.chkUseUpnp.Checked = Settings.UseUPnP;
			this.chkShowTooltip.Checked = Settings.ShowToolTip;
			this.chkNoIPIntegration.Checked = Settings.EnableNoIPUpdater;
			this.txtNoIPHost.Text = Settings.NoIPHost;
			this.txtNoIPUser.Text = Settings.NoIPUsername;
			this.txtNoIPPass.Text = Settings.NoIPPassword;
		}

		private ushort GetPortSafe()
		{
			ushort result;
			return ushort.TryParse(this.ncPort.Value.ToString((IFormatProvider)CultureInfo.InvariantCulture), out result) ? result : (ushort)0;
		}

		private void btnListen_Click(object sender, EventArgs e)
		{
			ushort portSafe = this.GetPortSafe();
			if (portSafe == (ushort)0)
			{
				int num1 = (int)MessageBox.Show("Please enter a valid port > 0.", "Please enter a valid port", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			else
			{
				if (this.btnListen.Text == "Start listening")
				{
					if (!this._listenServer.Listening)
					{
						try
						{
							if (this.chkNoIPIntegration.Checked)
								NoIpUpdater.Start();
							this._listenServer.Listen(portSafe, this.chkIPv6Support.Checked, this.chkUseUpnp.Checked);
							this.ToggleListenerSettings(false);
							return;
						}
						catch (SocketException ex)
						{
							if (ex.ErrorCode == 10048)
							{
								int num2 = (int)MessageBox.Show((IWin32Window)this, "The port is already in use.", "Socket Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
							}
							else
							{
								int num3 = (int)MessageBox.Show((IWin32Window)this, string.Format("An unexpected socket error occurred: {0}\n\nError Code: {1}\n\n", (object)ex.Message, (object)ex.ErrorCode), "Unexpected Socket Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
							}
							this._listenServer.Disconnect();
							return;
						}
						catch
						{
							this._listenServer.Disconnect();
							return;
						}
					}
				}
				if (!(this.btnListen.Text == "Stop listening") || !this._listenServer.Listening)
					return;
				this._listenServer.Disconnect();
				this.ToggleListenerSettings(true);
			}
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			ushort portSafe = this.GetPortSafe();
			if (portSafe == (ushort)0)
			{
				int num = (int)MessageBox.Show("Please enter a valid port > 0.", "Please enter a valid port", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			else
			{
				Settings.ListenPort = portSafe;
				Settings.IPv6Support = this.chkIPv6Support.Checked;
				Settings.AutoListen = this.chkAutoListen.Checked;
				Settings.ShowPopup = this.chkPopup.Checked;
				Settings.UseUPnP = this.chkUseUpnp.Checked;
				Settings.ShowToolTip = this.chkShowTooltip.Checked;
				Settings.EnableNoIPUpdater = this.chkNoIPIntegration.Checked;
				Settings.NoIPHost = this.txtNoIPHost.Text;
				Settings.NoIPUsername = this.txtNoIPUser.Text;
				Settings.NoIPPassword = this.txtNoIPPass.Text;
				this.Close();
			}
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Discard your changes?", "Cancel", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				return;
			this.Close();
		}

		private void chkNoIPIntegration_CheckedChanged(object sender, EventArgs e)
		{
			this.NoIPControlHandler(this.chkNoIPIntegration.Checked);
		}

		private void ToggleListenerSettings(bool enabled)
		{
			this.btnListen.Text = enabled ? "Start listening" : "Stop listening";
			this.ncPort.Enabled = enabled;
			this.chkIPv6Support.Enabled = enabled;
			this.chkUseUpnp.Enabled = enabled;
		}

		private void NoIPControlHandler(bool enable)
		{
			this.lblHost.Enabled = enable;
			this.lblUser.Enabled = enable;
			this.lblPass.Enabled = enable;
			this.txtNoIPHost.Enabled = enable;
			this.txtNoIPUser.Enabled = enable;
			this.txtNoIPPass.Enabled = enable;
			this.chkShowPassword.Enabled = enable;
		}

		private void ShowPassword(bool show = true)
		{
			this.txtNoIPPass.PasswordChar = show ? char.MinValue : '●';
		}

		private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
		{
			this.ShowPassword(this.chkShowPassword.Checked);
		}
	}
}