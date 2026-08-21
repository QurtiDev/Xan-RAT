

using InvokedCommon.Enums;
using InvokedCommon.Messages;
using InvokedCommon.Networking;
using System.Threading;


namespace InvokedServer.Messages
{
	public class ClientStatusHandler : MessageProcessorBase<object>
	{
		public event ClientStatusHandler.StatusUpdatedEventHandler StatusUpdated;

		public event ClientStatusHandler.UserStatusUpdatedEventHandler UserStatusUpdated;

		private void OnStatusUpdated(InvokedServer.Networking.Client client, string statusMessage)
		{
			this.SynchronizationContext.Post((SendOrPostCallback)(c =>
			{
				ClientStatusHandler.StatusUpdatedEventHandler statusUpdated = this.StatusUpdated;
				if (statusUpdated == null)
					return;
				statusUpdated((object)this, (InvokedServer.Networking.Client)c, statusMessage);
			}), (object)client);
		}

		private void OnUserStatusUpdated(InvokedServer.Networking.Client client, UserStatus userStatusMessage)
		{
			this.SynchronizationContext.Post((SendOrPostCallback)(c =>
			{
				ClientStatusHandler.UserStatusUpdatedEventHandler userStatusUpdated = this.UserStatusUpdated;
				if (userStatusUpdated == null)
					return;
				userStatusUpdated((object)this, (InvokedServer.Networking.Client)c, userStatusMessage);
			}), (object)client);
		}

		public ClientStatusHandler()
		  : base(true)
		{
		}

		public override bool CanExecute(IMessage message)
		{
			return message is SetStatus || message is SetUserStatus;
		}

		public override bool CanExecuteFrom(ISender sender) => true;

		public override void Execute(ISender sender, IMessage message)
		{
			switch (message)
			{
				case SetStatus message1:
					this.Execute((InvokedServer.Networking.Client)sender, message1);
					break;
				case SetUserStatus message2:
					this.Execute((InvokedServer.Networking.Client)sender, message2);
					break;
			}
		}

		private void Execute(InvokedServer.Networking.Client client, SetStatus message)
		{
			this.OnStatusUpdated(client, message.Message);
		}

		private void Execute(InvokedServer.Networking.Client client, SetUserStatus message)
		{
			this.OnUserStatusUpdated(client, message.Message);
		}

		public delegate void StatusUpdatedEventHandler(
		  object sender,
		  InvokedServer.Networking.Client client,
		  string statusMessage);

		public delegate void UserStatusUpdatedEventHandler(
		  object sender,
		  InvokedServer.Networking.Client client,
		  UserStatus userStatusMessage);
	}
}