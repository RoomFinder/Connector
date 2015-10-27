namespace ConnectorWebService
{
	public interface IStatusMonitor
	{
		void SetOnline();
		void SetOffline();
		void SetError(string message);
	}
}