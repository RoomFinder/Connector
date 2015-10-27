using System;
using FindFreeRoom.ExchangeConnector;

namespace ConnectorWebService
{
	internal class Session
	{
		private readonly TimeSpan _timeout;
		public DateTime ExpirationDate { get; private set; }
		public ExchangeConnector Connector { get; }

		public Session(ExchangeConnector connector, TimeSpan timeout)
		{
			Connector = connector;
			_timeout = timeout;
			ExpirationDate = DateTime.Now + timeout;
		}

		public void Touch()
		{
			ExpirationDate = DateTime.Now + _timeout;
		}
	}
}