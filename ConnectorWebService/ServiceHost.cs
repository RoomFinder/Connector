using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace ConnectorWebService
{
	public class ServiceHost : IDisposable
	{
		private ILog _log;
		private IStatusMonitor _statusMonitor;
		private readonly WebServiceHost _host;

		public ServiceHost(ILog log, IStatusMonitor statusMonitor)
		{
			_log = log;
			_statusMonitor = statusMonitor;
			_host = new WebServiceHost(typeof(Service), new Uri("http://0.0.0.0:18799/api"));
			_host.AddDefaultEndpoints()[0].Binding = new WebHttpBinding();
			_host.Description.Endpoints[0].Behaviors.Add(new WebHttpBehavior { HelpEnabled = true });
		}

		public void Start()
		{
			try
			{
				_host.Open();
				_statusMonitor?.SetOnline();
			}
			catch (Exception ex)
			{
				_log?.AddMessage("Failed to start: {0}", ex);
				_statusMonitor?.SetError(ex.Message);
			}
		}

		public void Stop()
		{
			try
			{
				_host.Close();
				_statusMonitor?.SetOffline();
			}
			catch (Exception ex)
			{
				_log?.AddMessage("Failed to start: {0}", ex);
				_statusMonitor?.SetError(ex.Message);
			}
		}

		public void Dispose()
		{
			_log = null;
			_statusMonitor = null;
			Stop();
		}
	}
}
