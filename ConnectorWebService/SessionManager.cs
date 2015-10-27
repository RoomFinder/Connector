using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FindFreeRoom.ExchangeConnector;

namespace ConnectorWebService
{
	public static class SessionManager
	{
		private static readonly TimeSpan SessionTimeout = TimeSpan.FromMinutes(30);
		private static readonly ConcurrentDictionary<string, Session> AllSessions = new ConcurrentDictionary<string, Session>();

		public static async Task GarbageCollectorAsync(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				try
				{
					await Task.Delay(5000, cancellationToken);
				}
				catch (TaskCanceledException)
				{
					return;
				}
				var expiredTickets = AllSessions.Where(session => session.Value.ExpirationDate > DateTime.Now).Select(session => session.Key).ToArray();
				foreach (var expiredTicket in expiredTickets)
				{
					Session tmp;
					if (AllSessions.TryRemove(expiredTicket, out tmp))
					{
						Debug.WriteLine($"Session {expiredTicket} expires");
					}
				}
			}
		}

		public static bool TryAdd(string ticket, ExchangeConnector connector)
		{
			Debug.WriteLine($"New session created ({ticket})");
			return AllSessions.TryAdd(ticket, new Session(connector, SessionTimeout));
		}

		public static bool TryGetValue(string ticket, out ExchangeConnector connector)
		{
			Session tmp;
			if (AllSessions.TryGetValue(ticket, out tmp))
			{
				tmp.Touch();
				connector = tmp.Connector;
				return true;
			}
			connector = null;
			return false;
		}
	}
}
