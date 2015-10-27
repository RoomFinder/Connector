using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Security.Tokens;
using System.ServiceModel.Web;
using FindFreeRoom.ExchangeConnector;
using FindFreeRoom.ExchangeConnector.Base;

namespace ConnectorWebService
{
	public class Service : IService
	{
		private static readonly Dictionary<string, ExchangeConnector> ConnectionCache = new Dictionary<string, ExchangeConnector>();

		public IEnumerable<RoomDataContract> GetRooms(string ticket)
		{
			ExchangeConnector connector;
			if (!ConnectionCache.TryGetValue(ticket, out connector))
			{
				throw new WebFaultException<string>("Invalid ticket", HttpStatusCode.Unauthorized);
			}
			var roomsNearby = connector.GetFilteredRooms();
			var availableRooms = connector.GetAvaialility(roomsNearby).Where(a => a.Availability != TimeInterval.Zero);
			return availableRooms.Select(Convertions.ToContract);
		}

		public string Login(string username, string password, string email, string site, string serviceUrl)
		{
			if (string.IsNullOrEmpty(username))
			{
				throw new WebFaultException<string>($"{nameof(username)} is a mandatory parameter", HttpStatusCode.BadRequest);
			}
			if (string.IsNullOrEmpty(password))
			{
				throw new WebFaultException<string>($"{nameof(password)} is a mandatory parameter", HttpStatusCode.BadRequest);
			}
			if (string.IsNullOrEmpty(email))
			{
				throw new WebFaultException<string>($"{nameof(email)} is a mandatory parameter", HttpStatusCode.BadRequest);
			}
			if (string.IsNullOrEmpty(site))
			{
				throw new WebFaultException<string>($"{nameof(site)} is a mandatory parameter", HttpStatusCode.BadRequest);
			}

			try
			{
				var connector = new ExchangeConnector(username, password, serviceUrl, email);
				LocationResolver locations = new LocationResolver();
				locations.Load("locationMap.csv");
				connector.LocationFilter = locations.OfSite(site).ToArray(); // filter locations by site
				connector.Connect();
#if DEBUG
				string ticket = ConnectionCache.Count.ToString();
#else
				string ticket = Guid.NewGuid().ToString("N");
#endif
				ConnectionCache[ticket] = connector;
				return ticket;
			}
			catch
			{
				throw new WebFaultException(HttpStatusCode.Unauthorized);
			}
		}
	}
}
