using System;
using System.Windows.Forms;
using ConnectorWebService;

namespace ConnectorWebServer
{
	internal class TrayIcon : IStatusMonitor, IDisposable
	{
		private readonly NotifyIcon _icon;

		public TrayIcon(LogWindow window)
		{
			_icon = new NotifyIcon
			{
				Text = "ConnectorWebServer",
				Icon = Properties.Resources.icon_error,
				ContextMenu = new ContextMenu(new[]
				{
					new MenuItem("Log", (o,e) => window.Display()),
					new MenuItem("Exit", (o, e) => Application.Exit())
				}),
				Visible = true
			};
		}

		public void SetOnline()
		{
			_icon.Icon = Properties.Resources.icon;
		}

		public void SetOffline()
		{
			_icon.Icon = Properties.Resources.icon_error;
		}

		public void SetError(string message)
		{
			_icon.ShowBalloonTip(4000, "Error", message, ToolTipIcon.Error);
		}

		public void Dispose()
		{
			_icon.Dispose();
		}
	}
}