

using InvokedServer.Models;
using System;
using System.Net;
using System.Text;
using System.Threading;


namespace InvokedServer.Utilities
{
	public static class NoIpUpdater
	{
		private static bool _running;

		public static void Start()
		{
			if (NoIpUpdater._running)
				return;
			new Thread(new ThreadStart(NoIpUpdater.BackgroundUpdater))
			{
				IsBackground = true
			}.Start();
		}

		private static void BackgroundUpdater()
		{
			NoIpUpdater._running = true;
			while (Settings.EnableNoIPUpdater)
			{
				try
				{
					HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(string.Format("https://dynupdate.no-ip.com/nic/update?hostname={0}", (object)Settings.NoIPHost));
					httpWebRequest.Proxy = (IWebProxy)null;
					httpWebRequest.UserAgent = string.Format("Quasar No-Ip Updater/2.0 {0}", (object)Settings.NoIPUsername);
					httpWebRequest.Timeout = 10000;
					httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, string.Format("Basic {0}", (object)Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", (object)Settings.NoIPUsername, (object)Settings.NoIPPassword)))));
					httpWebRequest.Method = "GET";
					httpWebRequest.GetResponse();

                }
				catch
				{
				}
				Thread.Sleep(TimeSpan.FromMinutes(10.0));
			}
			NoIpUpdater._running = false;
		}
	}
}