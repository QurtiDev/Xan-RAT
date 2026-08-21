

using InvokedCommon.Messages;
using InvokedCommon.Networking;
using System;
using System.Collections.Generic;


namespace InvokedServer.Messages
{
	public class SystemInformationHandler : MessageProcessorBase<List<Tuple<string, string>>>
	{
		private readonly InvokedServer.Networking.Client _client;

		public SystemInformationHandler(InvokedServer.Networking.Client client)
		  : base(true)
		{
			this._client = client;
		}

		public override bool CanExecute(IMessage message) => message is GetSystemInfoResponse;

		public override bool CanExecuteFrom(ISender client) => this._client.Equals((object)client);

		public override void Execute(ISender sender, IMessage message)
		{
			if (!(message is GetSystemInfoResponse message1))
				return;
			this.Execute(sender, message1);
		}

		public void RefreshSystemInformation() => this._client.Send<GetSystemInfo>(new GetSystemInfo());

		private void Execute(ISender client, GetSystemInfoResponse message)
		{
			this.OnReport(message.SystemInfos);
		}
	}
}