namespace ConnectorWebService
{
	public interface ILog
	{
		void AddMessage(string format, params object[] args);
		void AddMessage(string message);
	}
}