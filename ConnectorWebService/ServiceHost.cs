using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Threading;
using System.Threading.Tasks;

namespace ConnectorWebService
{
	public class ServiceHost : IDisposable
	{
		private static readonly Uri ServiceUri = new Uri("http://0.0.0.0:18799/api");

		internal static ILog Log;
		internal static IStatusMonitor StatusMonitor;
		private readonly WebServiceHost _host;
		private readonly CancellationTokenSource _gcCancellation = new CancellationTokenSource();

		public ServiceHost(ILog log, IStatusMonitor statusMonitor)
		{
			Log = log;
			StatusMonitor = statusMonitor;
			_host = new WebServiceHost(typeof(Service), ServiceUri);
			// TODO: Enable HTTPS - http://stackoverflow.com/questions/14933696
			_host.AddDefaultEndpoints()[0].Binding = new WebHttpBinding();
			_host.Description.Endpoints[0].Behaviors.Add(new WebHttpBehavior { HelpEnabled = true });
		}

		public void Start()
		{
			try
			{
				Task.Run(() => _host.Open()).Wait();
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
