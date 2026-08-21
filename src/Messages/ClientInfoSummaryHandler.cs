

using InvokedCommon.Messages;
using InvokedCommon.Networking;
using InvokedCommon.Video.Codecs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;


namespace InvokedServer.Messages
{
	public class ClientInfoSummaryHandler : MessageProcessorBase<object>
	{
		private static UnsafeStreamCodec _codec;
		private static Size LocalResolution = new Size(190, 100);

		public event ClientInfoSummaryHandler.SummaryInfoHandler NewSystemInfoSummary;

		public event ClientInfoSummaryHandler.SummaryInfoImageHandler NewSystemInfoImage;

		public event ClientInfoSummaryHandler.PluginLoadedHandler NewPluginLoaded;

		private void OnNewInfoResponse(InvokedServer.Networking.Client client, List<Tuple<string, string>> lstInfos)
		{
			this.SynchronizationContext.Post((SendOrPostCallback)(c =>
			{
				ClientInfoSummaryHandler.SummaryInfoHandler systemInfoSummary = this.NewSystemInfoSummary;
				if (systemInfoSummary == null)
					return;
				systemInfoSummary((object)this, (InvokedServer.Networking.Client)c, lstInfos);
			}), (object)client);
		}

		private void OnNewImageResponse(InvokedServer.Networking.Client client, Bitmap DesktopPreview)
		{
			this.SynchronizationContext.Post((SendOrPostCallback)(c =>
			{
				ClientInfoSummaryHandler.SummaryInfoImageHandler newSystemInfoImage = this.NewSystemInfoImage;
				if (newSystemInfoImage == null)
					return;
				newSystemInfoImage((object)this, (InvokedServer.Networking.Client)c, DesktopPreview);
			}), (object)client);
		}

		private void OnNewPluginLoaded(InvokedServer.Networking.Client client, string PluginName)
		{
			this.SynchronizationContext.Post((SendOrPostCallback)(c =>
			{
				ClientInfoSummaryHandler.PluginLoadedHandler newPluginLoaded = this.NewPluginLoaded;
				if (newPluginLoaded == null)
					return;
				newPluginLoaded((object)this, (InvokedServer.Networking.Client)c, PluginName);
			}), (object)client);
		}

		public ClientInfoSummaryHandler()
		  : base(true)
		{
		}

		public override bool CanExecute(IMessage message)
		{
			switch (message)
			{
				case GetSystemSummaryInfoResponse _:
				case GetSystemSummaryDesktopImageResponse _:
					return true;
				default:
					return message is PluginLoadedResponse;
			}
		}

		public override bool CanExecuteFrom(ISender sender) => true;

		public override void Execute(ISender sender, IMessage message)
		{
			switch (message)
			{
				case GetSystemSummaryInfoResponse message1:
					this.Execute((InvokedServer.Networking.Client)sender, message1);
					break;
				case GetSystemSummaryDesktopImageResponse message2:
					this.Execute((InvokedServer.Networking.Client)sender, message2);
					break;
				case PluginLoadedResponse message3:
					this.Execute((InvokedServer.Networking.Client)sender, message3);
					break;
			}
		}

		private void Execute(InvokedServer.Networking.Client client, PluginLoadedResponse message)
		{
			if (message == null)
				return;
			this.OnNewPluginLoaded(client, message.PluginName);
		}

		private void Execute(InvokedServer.Networking.Client client, GetSystemSummaryInfoResponse message)
		{
			if (message == null)
				return;
			this.OnNewInfoResponse(client, message.SystemSummaryInfos);
		}

		private void Execute(InvokedServer.Networking.Client client, GetSystemSummaryDesktopImageResponse message)
		{
			if (message == null)
				return;
			ClientInfoSummaryHandler._codec?.Dispose();
			ClientInfoSummaryHandler._codec = new UnsafeStreamCodec(message.Quality, message.Monitor, message.Resolution);
			ClientInfoSummaryHandler.LocalResolution = new Size(190, 100);
			try
			{
				using (MemoryStream inStream = new MemoryStream(message.Image))
					this.OnNewImageResponse(client, new Bitmap((Image)ClientInfoSummaryHandler._codec.DecodeData((Stream)inStream), ClientInfoSummaryHandler.LocalResolution));
			}
			catch
			{
			}
		}

		public delegate void SummaryInfoHandler(
		  object sender,
		  InvokedServer.Networking.Client client,
		  List<Tuple<string, string>> lstInfos);

		public delegate void SummaryInfoImageHandler(
		  object sender,
		  InvokedServer.Networking.Client client,
		  Bitmap DesktopPreview);

		public delegate void PluginLoadedHandler(object sender, InvokedServer.Networking.Client client, string PluginName);
	}
}