

using InvokedServer.Forms;
using System;
using System.Net;
using System.Windows.Forms;

namespace InvokedServer
{
  internal static class Program
  {
    [STAThread]
    private static void Main()
    {
      ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new FrmMain());
    }
  }
}
