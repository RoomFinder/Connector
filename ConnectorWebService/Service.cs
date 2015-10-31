using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading;
using System.Threading.Tasks;
using FindFreeRoom.ExchangeConnector;
using FindFreeRoom.ExchangeConnector.Base;

namespace ConnectorWebService
{
	[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
	public class Service : IService
	{
		private const int MaxRoomsInResponse = 15;
		private static readonly LocationResolver LocationResolver = new LocationResolver();

		static Service()
		{
			try
			{
				LocationResolver.Load("locationMap.csv");
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				ServiceHost.StatusMonitor?.SetError($"Unable to load location map: {ex.Message}");
				ServiceHost.Log?.AddMessage($"Unable to load location map: {ex.Message}");
			}
		}

		private static ExchangeConnector GetConnector(string ticket)
		{
			Debug.WriteLine($"Request on thread {Thread.CurrentThread.ManagedThreadId}");
			ExchangeConnector connector;
			if (!SessionManager.TryGetValue(ticket, out connector))
			{
				throw new WebFaultException<string>("Invalid ticket", HttpStatusCode.Unauthorized);
			}

			return connector;
		}

		public IEnumerable<RoomDataContract> GetRooms(string lat, string lon, string ticket)
		{
			try
			{
				var connector = GetConnector(ticket);
				var roomsNearby = connector.GetFilteredRooms();
				if (string.IsNullOrEmpty(lat) || string.IsNullOrEmpty(lon))
				{
					var availableRooms = connector.GetAvaialility(roomsNearby).Where(a => a.Availability != TimeInterval.Zero).Take(MaxRoomsInResponse);
					return availableRooms.Select(Convertions.ToContract);
				}
				else
				{
					double latitude;
					double longitude;
					if (!double.TryParse(lat, out latitude) || !double.TryParse(lon, out longitude))
					{
						throw new WebFaultException<string>($"Invalid {nameof(lat)} or {nameof(lon)} values", HttpStatusCode.BadRequest);
					}
					var loc = new Location
					{
						// TODO: real elevation
						Geometry = new Geometry(latitude, longitude, 120.0)
					};
					var availableRooms = connector.GetAvaialility(LocationResolver.ResolveLocations(roomsNearby));
					availableRooms = LocationResolver.SmartSort(availableRooms, loc).Take(MaxRoomsInResponse);
					return availableRooms.Select(Convertions.ToContract);
				}
			}
			catch (WebFaultException<string>)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new WebFaultException<string>(ex.Message, HttpStatusCode.InternalServerError);
			}
		}

		public RoomDataContract GetRoom(string roomId, string ticket)
		{
			try
			{
				var connector = GetConnector(ticket);
				var theRoom = connector.GetFilteredRooms().FirstOrDefault(r => string.Equals(r.RoomId, roomId, StringComparison.InvariantCultureIgnoreCase));
				if (theRoom == null)
				{
					throw new WebFaultException<string>("Unable to find the requested room", HttpStatusCode.NotFound);
				}
				var theRoomAvailability = connector.GetAvaialility(LocationResolver.ResolveLocations(new [] { theRoom })).First();
			
				return theRoomAvailability.ToContract();
			}
			catch (WebFaultException<string>)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new WebFaultException<string>(ex.Message, HttpStatusCode.InternalServerError);
			}
		}

		public async Task ReserveRoom(int duration, string roomId, string ticket)
		{
			if (duration <= 0)
			{
				throw new WebFaultException<string>($"{nameof(duration)} should be > 0", HttpStatusCode.BadRequest);
			}
			try
			{
				var connector = GetConnector(ticket);
				var theRoom = connector.GetFilteredRooms().FirstOrDefault(r => string.Equals(r.RoomId, roomId, StringComparison.InvariantCultureIgnoreCase));
				if (theRoom == null)
				{
					throw new WebFaultException<string>("Unable to find the requested room", HttpStatusCode.NotFound);
				}
				if (!await connector.ReserveRoom(theRoom, TimeSpan.FromMinutes(duration)))
				{
					throw new WebFaultException<string>("Unable to reserve the room", HttpStatusCode.Conflict);
				}
			}
			catch (WebFaultException<string>)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new WebFaultException<string>(ex.Message, HttpStatusCode.InternalServerError);
			}
		}

		public IEnumerable<MeetingInfoDataContract> GetMeetings(string ticket)
		{
			try
			{
				var connector = GetConnector(ticket);
				return connector.GetMyMeetings().Select(Convertions.ToContract);
			}
			catch (WebFaultException<string>)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new WebFaultException<string>(ex.Message, HttpStatusCode.InternalServerError);
			}
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
				connector.LocationFilter = LocationResolver.OfSite(site).ToArray(); // filter locations by site
				connector.Connect();
				string ticket = Guid.NewGuid().ToString("N");
				if (!SessionManager.TryAdd(ticket, connector))
				{
					throw new WebFaultException<string>("GUID conflict", HttpStatusCode.InternalServerError);
				}
				return ticket;
			}
			catch (Exception ex)
			{
				throw new WebFaultException<string>(ex.Message, HttpStatusCode.Unauthorized);
			}
		}
	}
}
