using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConnectorWebService;

namespace ConnectorWebServer
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			using (var logWindow = new LogWindow())
			{
				using (var trayIcon = new TrayIcon(logWindow))
				{
					using (var host = new ServiceHost(logWindow, trayIcon))
					{
						host.Start();
						Application.Run();
					}
				}
			}
		}
	}
}
