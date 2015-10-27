using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Threading;
using System.Threading.Tasks;
using ConnectorWebService.Web;

namespace ConnectorWebService
{
	public class ServiceHost : IDisposable
	{
		private static readonly Uri ServiceUri = new Uri("http://0.0.0.0:18799/api");
		private static readonly Uri WebAppUri = new Uri("http://0.0.0.0:18799");

		internal static ILog Log;
		internal static IStatusMonitor StatusMonitor;
		private readonly WebServiceHost _host;
		private readonly WebServiceHost _webAppHost;
		private readonly CancellationTokenSource _gcCancellation = new CancellationTokenSource();

		public ServiceHost(ILog log, IStatusMonitor statusMonitor)
		{
			Log = log;
			StatusMonitor = statusMonitor;
			_host = new WebServiceHost(typeof(Service), ServiceUri);
			// TODO: Enable HTTPS - http://stackoverflow.com/questions/14933696
			_host.AddDefaultEndpoints()[0].Binding = new WebHttpBinding();
			_host.Description.Endpoints[0].Behaviors.Add(new WebHttpBehavior { HelpEnabled = true });

			_webAppHost = new WebServiceHost(typeof(WebServer), WebAppUri);
			_webAppHost.AddDefaultEndpoints()[0].Binding = new WebHttpBinding();
			_webAppHost.Description.Endpoints[0].Behaviors.Add(new WebHttpBehavior());
		}

		public void Start()
		{
			try
			{
				try
				{
					Task.Run(() => _host.Open()).Wait();
					Task.Run(() => _webAppHost.Open()).Wait();
				}
				catch (AggregateException ex)
				{
					throw ex.InnerExceptions.FirstOrDefault() ?? ex;
				}
				Task.Run(() => SessionManager.GarbageCollectorAsync(_gcCancellation.Token));
				StatusMonitor?.SetOnline();
				Log?.AddMessage("Service successfully started at {0}", ServiceUri);
			}
			catch (Exception ex)
			{
				Log?.AddMessage("Failed to start: {0}", ex);
				StatusMonitor?.SetError(ex.Message);
			}
		}

		public void Stop()
		{
			try
			{
				_gcCancellation.Cancel();
				_host.Close();
				_webAppHost.Close();
				StatusMonitor?.SetOffline();
			}
			catch (Exception ex)
			{
				Log?.AddMessage("Failed to start: {0}", ex);
				StatusMonitor?.SetError(ex.Message);
			}
		}

		public void Dispose()
		{
			Log = null;
			StatusMonitor = null;
			Stop();
		}
	}
}
