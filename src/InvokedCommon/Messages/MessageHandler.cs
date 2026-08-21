

using InvokedCommon.Networking;
using System;
using System.Collections.Generic;
using System.Linq;


namespace InvokedCommon.Messages
{
	public static class MessageHandler
	{
		private static readonly List<IMessageProcessor> Processors = new List<IMessageProcessor>();
		private static readonly object SyncLock = new object();

		public static void Register(IMessageProcessor proc)
		{
			lock (MessageHandler.SyncLock)
			{
				if (MessageHandler.Processors.Contains(proc))
					return;
				MessageHandler.Processors.Add(proc);
			}
		}

		public static void Unregister(IMessageProcessor proc)
		{
			lock (MessageHandler.SyncLock)
				MessageHandler.Processors.Remove(proc);
		}

		public static void Process(ISender sender, IMessage msg)
		{
			IEnumerable<IMessageProcessor> list;
			lock (MessageHandler.SyncLock)
				list = (IEnumerable<IMessageProcessor>) MessageHandler.Processors.Where<IMessageProcessor>((Func<IMessageProcessor, bool>) (x => x.CanExecute(msg) && x.CanExecuteFrom(sender))).ToList<IMessageProcessor>();
			foreach (IMessageProcessor messageProcessor in list)
				messageProcessor.Execute(sender, msg);
		}
	}
}
