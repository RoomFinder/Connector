using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace FindFreeRoom.ConnectorConsole
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Connector started.");

			try
			{
				DoWork();
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
				Console.WriteLine(ex);
			} 
		}

		static void DoWork()
		{
			var props = Properties.Settings.Default;
			var connector = new ExchangeConnector.ExchangeConnector(props.username, props.password, props.serverUrl, props.serviceEmail);

			string currentSite = props.currentSite;
			LocationResolver locations = new LocationResolver(); // TODO: we have persistent map: email -> site, building, floor
			locations.Load("locationMap.csv");
			connector.ActiveLocations = locations.OfSite(currentSite).ToArray(); // filter locations by site

			connector.Connect();
			var roomsNearby = connector.GetFilteredRooms().Select(i => new Tuple<string, string>(i.LocationId, i.RoomId));
			var roomsWithLocations = locations.ResolveLocations(roomsNearby);

		}
	}
}
