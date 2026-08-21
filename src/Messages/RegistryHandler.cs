

using InvokedCommon.Messages;
using InvokedCommon.Models;
using InvokedCommon.Networking;
using Microsoft.Win32;
using System.Threading;


namespace InvokedServer.Messages
{
	public class RegistryHandler : MessageProcessorBase<string>
	{
		private readonly InvokedServer.Networking.Client _client;

		public event RegistryHandler.KeysReceivedEventHandler KeysReceived;

		public event RegistryHandler.KeyCreatedEventHandler KeyCreated;

		public event RegistryHandler.KeyDeletedEventHandler KeyDeleted;

		public event RegistryHandler.KeyRenamedEventHandler KeyRenamed;

		public event RegistryHandler.ValueCreatedEventHandler ValueCreated;

		public event RegistryHandler.ValueDeletedEventHandler ValueDeleted;

		public event RegistryHandler.ValueRenamedEventHandler ValueRenamed;

		public event RegistryHandler.ValueChangedEventHandler ValueChanged;

		private void OnKeysReceived(string rootKey, RegSeekerMatch[] matches)
		{
			this.SynchronizationContext.Post((SendOrPostCallback)(t =>
			{
				RegistryHandler.KeysReceivedEventHandler keysReceived = this.KeysReceived;
				if (keysReceived == null)
					return;
				keysReceived((object)this, rootKey, (RegSeekerMatch[])t);
			}), (object)matches);
		}

		private void OnKeyCreated(string parentPath, RegSeekerMatch match)
		{
			this.SynchronizationContext.Post((SendOrPostCallback)(t =>
			{
				RegistryHandler.KeyCreatedEventHandler keyCreated = this.KeyCreated;
				if (keyCreated == null)
					return;
				keyCreated((object)this, parentPath, (RegSeekerMatch)t);
			}), (object)match);
		}

		private void OnKeyDeleted(string parentPath, string subKey)
		{
			this.SynchronizationContext.Post((SendOrPostCallback)(t =>
			{
				RegistryHandler.KeyDeletedEventHandler keyDeleted = this.KeyDeleted;
				if (keyDeleted == null)
					return;
				keyDeleted((object)this, parentPath, (string)t);
			}), (object)subKey);
		}

		private void OnKeyRenamed(string parentPath, string oldSubKey, string newSubKey)
		{
			this.SynchronizationContext.Post((SendOrPostCallback)(t =>
			{
				RegistryHandler.KeyRenamedEventHandler keyRenamed = this.KeyRenamed;
				if (keyRenamed == null)
					return;
				keyRenamed((object)this, parentPath, oldSubKey, (string)t);
			}), (object)newSubKey);
		}

		private void OnValueCreated(string keyPath, RegValueData value)
		{
			this.SynchronizationContext.Post((SendOrPostCallback)(t =>
			{
				RegistryHandler.ValueCreatedEventHandler valueCreated = this.ValueCreated;
				if (valueCreated == null)
					return;
				valueCreated((object)this, keyPath, (RegValueData)t);
			}), (object)value);
		}

		private void OnValueDeleted(string keyPath, string valueName)
		{
			this.SynchronizationContext.Post((SendOrPostCallback)(t =>
			{
				RegistryHandler.ValueDeletedEventHandler valueDeleted = this.ValueDeleted;
				if (valueDeleted == null)
					return;
				valueDeleted((object)this, keyPath, (string)t);
			}), (object)valueName);
		}

		private void OnValueRenamed(string keyPath, string oldValueName, string newValueName)
		{
			this.SynchronizationContext.Post((SendOrPostCallback)(t =>
			{
				RegistryHandler.ValueRenamedEventHandler valueRenamed = this.ValueRenamed;
				if (valueRenamed == null)
					return;
				valueRenamed((object)this, keyPath, oldValueName, (string)t);
			}), (object)newValueName);
		}

		private void OnValueChanged(string keyPath, RegValueData value)
		{
			this.SynchronizationContext.Post((SendOrPostCallback)(t =>
			{
				RegistryHandler.ValueChangedEventHandler valueChanged = this.ValueChanged;
				if (valueChanged == null)
					return;
				valueChanged((object)this, keyPath, (RegValueData)t);
			}), (object)value);
		}

		public RegistryHandler(InvokedServer.Networking.Client client)
		  : base(true)
		{
			this._client = client;
		}

		public override bool CanExecute(IMessage message)
		{
			switch (message)
			{
				case GetRegistryKeysResponse _:
				case GetCreateRegistryKeyResponse _:
				case GetDeleteRegistryKeyResponse _:
				case GetRenameRegistryKeyResponse _:
				case GetCreateRegistryValueResponse _:
				case GetDeleteRegistryValueResponse _:
				case GetRenameRegistryValueResponse _:
					return true;
				default:
					return message is GetChangeRegistryValueResponse;
			}
		}

		public override bool CanExecuteFrom(ISender sender) => this._client.Equals((object)sender);

		public override void Execute(ISender sender, IMessage message)
		{
			switch (message)
			{
				case GetRegistryKeysResponse message1:
					this.Execute(sender, message1);
					break;
				case GetCreateRegistryKeyResponse message2:
					this.Execute(sender, message2);
					break;
				case GetDeleteRegistryKeyResponse message3:
					this.Execute(sender, message3);
					break;
				case GetRenameRegistryKeyResponse message4:
					this.Execute(sender, message4);
					break;
				case GetCreateRegistryValueResponse message5:
					this.Execute(sender, message5);
					break;
				case GetDeleteRegistryValueResponse message6:
					this.Execute(sender, message6);
					break;
				case GetRenameRegistryValueResponse message7:
					this.Execute(sender, message7);
					break;
				case GetChangeRegistryValueResponse message8:
					this.Execute(sender, message8);
					break;
			}
		}

		public void LoadRegistryKey(string rootKeyName)
		{
			this._client.Send<DoLoadRegistryKey>(new DoLoadRegistryKey()
			{
				RootKeyName = rootKeyName
			});
		}

		public void CreateRegistryKey(string parentPath)
		{
			this._client.Send<DoCreateRegistryKey>(new DoCreateRegistryKey()
			{
				ParentPath = parentPath
			});
		}

		public void DeleteRegistryKey(string parentPath, string keyName)
		{
			this._client.Send<DoDeleteRegistryKey>(new DoDeleteRegistryKey()
			{
				ParentPath = parentPath,
				KeyName = keyName
			});
		}

		public void RenameRegistryKey(string parentPath, string oldKeyName, string newKeyName)
		{
			this._client.Send<DoRenameRegistryKey>(new DoRenameRegistryKey()
			{
				ParentPath = parentPath,
				OldKeyName = oldKeyName,
				NewKeyName = newKeyName
			});
		}

		public void CreateRegistryValue(string keyPath, RegistryValueKind kind)
		{
			this._client.Send<DoCreateRegistryValue>(new DoCreateRegistryValue()
			{
				KeyPath = keyPath,
				Kind = kind
			});
		}

		public void DeleteRegistryValue(string keyPath, string valueName)
		{
			this._client.Send<DoDeleteRegistryValue>(new DoDeleteRegistryValue()
			{
				KeyPath = keyPath,
				ValueName = valueName
			});
		}

		public void RenameRegistryValue(string keyPath, string oldValueName, string newValueName)
		{
			this._client.Send<DoRenameRegistryValue>(new DoRenameRegistryValue()
			{
				KeyPath = keyPath,
				OldValueName = oldValueName,
				NewValueName = newValueName
			});
		}

		public void ChangeRegistryValue(string keyPath, RegValueData value)
		{
			this._client.Send<DoChangeRegistryValue>(new DoChangeRegistryValue()
			{
				KeyPath = keyPath,
				Value = value
			});
		}

		private void Execute(ISender client, GetRegistryKeysResponse message)
		{
			if (!message.IsError)
				this.OnKeysReceived(message.RootKey, message.Matches);
			else
				this.OnReport(message.ErrorMsg);
		}

		private void Execute(ISender client, GetCreateRegistryKeyResponse message)
		{
			if (!message.IsError)
				this.OnKeyCreated(message.ParentPath, message.Match);
			else
				this.OnReport(message.ErrorMsg);
		}

		private void Execute(ISender client, GetDeleteRegistryKeyResponse message)
		{
			if (!message.IsError)
				this.OnKeyDeleted(message.ParentPath, message.KeyName);
			else
				this.OnReport(message.ErrorMsg);
		}

		private void Execute(ISender client, GetRenameRegistryKeyResponse message)
		{
			if (!message.IsError)
				this.OnKeyRenamed(message.ParentPath, message.OldKeyName, message.NewKeyName);
			else
				this.OnReport(message.ErrorMsg);
		}

		private void Execute(ISender client, GetCreateRegistryValueResponse message)
		{
			if (!message.IsError)
				this.OnValueCreated(message.KeyPath, message.Value);
			else
				this.OnReport(message.ErrorMsg);
		}

		private void Execute(ISender client, GetDeleteRegistryValueResponse message)
		{
			if (!message.IsError)
				this.OnValueDeleted(message.KeyPath, message.ValueName);
			else
				this.OnReport(message.ErrorMsg);
		}

		private void Execute(ISender client, GetRenameRegistryValueResponse message)
		{
			if (!message.IsError)
				this.OnValueRenamed(message.KeyPath, message.OldValueName, message.NewValueName);
			else
				this.OnReport(message.ErrorMsg);
		}

		private void Execute(ISender client, GetChangeRegistryValueResponse message)
		{
			if (!message.IsError)
				this.OnValueChanged(message.KeyPath, message.Value);
			else
				this.OnReport(message.ErrorMsg);
		}

		public delegate void KeysReceivedEventHandler(
		  object sender,
		  string rootKey,
		  RegSeekerMatch[] matches);

		public delegate void KeyCreatedEventHandler(
		  object sender,
		  string parentPath,
		  RegSeekerMatch match);

		public delegate void KeyDeletedEventHandler(object sender, string parentPath, string subKey);

		public delegate void KeyRenamedEventHandler(
		  object sender,
		  string parentPath,
		  string oldSubKey,
		  string newSubKey);

		public delegate void ValueCreatedEventHandler(
		  object sender,
		  string keyPath,
		  RegValueData value);

		public delegate void ValueDeletedEventHandler(object sender, string keyPath, string valueName);

		public delegate void ValueRenamedEventHandler(
		  object sender,
		  string keyPath,
		  string oldValueName,
		  string newValueName);

		public delegate void ValueChangedEventHandler(
		  object sender,
		  string keyPath,
		  RegValueData value);
	}
}