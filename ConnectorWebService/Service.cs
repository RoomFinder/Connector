using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading;
using FindFreeRoom.ExchangeConnector;
using FindFreeRoom.ExchangeConnector.Base;

namespace ConnectorWebService
{
	[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
	public class Service : IService
	{

		public IEnumerable<RoomDataContract> GetRooms(string ticket)
		{
			Debug.WriteLine($"Request on thread {Thread.CurrentThread.ManagedThreadId}");
			ExchangeConnector connector;
			if (!SessionManager.TryGetValue(ticket, out connector))
			{
				throw new WebFaultException<string>("Invalid ticket", HttpStatusCode.Unauthorized);
			}
			var roomsNearby = connector.GetFilteredRooms();
			var availableRooms = connector.GetAvaialility(roomsNearby).Where(a => a.Availability != TimeInterval.Zero);
			return availableRooms.Select(Convertions.ToContract);
		}

		public string Login(string username, string password, string email, string site, string serviceUrl)
		{
			Debug.WriteLine($"Request on thread {Thread.CurrentThread.ManagedThreadId}");
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
				string ticket = Guid.NewGuid().ToString("N");
				if (!SessionManager.TryAdd(ticket, connector))
				{
					throw new WebFaultException<string>("GUID conflict", HttpStatusCode.InternalServerError);
				}
				return ticket;
			}
			catch
			{
				throw new WebFaultException(HttpStatusCode.Unauthorized);
			}
		}
	}
}
