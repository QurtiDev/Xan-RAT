

using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;


namespace InvokedCommon.Extensions
{
	public static class SocketExtensions
	{
		public static void SetKeepAliveEx(
			this Socket socket,
			uint keepAliveInterval,
			uint keepAliveTime)
		{
			SocketExtensions.TcpKeepAlive structure = new SocketExtensions.TcpKeepAlive()
			{
				onoff = 1,
				keepaliveinterval = keepAliveInterval,
				keepalivetime = keepAliveTime
			};
			int length = Marshal.SizeOf<SocketExtensions.TcpKeepAlive>(structure);
			IntPtr num = Marshal.AllocHGlobal(length);
			Marshal.StructureToPtr<SocketExtensions.TcpKeepAlive>(structure, num, true);
			byte[] numArray = new byte[length];
			Marshal.Copy(num, numArray, 0, length);
			Marshal.FreeHGlobal(num);
			socket.IOControl(IOControlCode.KeepAliveValues, numArray, (byte[]) null);
		}

		internal struct TcpKeepAlive
		{
			internal uint onoff;
			internal uint keepalivetime;
			internal uint keepaliveinterval;
		}
	}
}
