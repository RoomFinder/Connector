using System;
using System.ComponentModel;
using System.Diagnostics;

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
			var connector = new ExchangeConnector.ExchangeConnector(props.username, props.password, props.serviceEmail);

			string currentSite;
			LocationResolver locations = new LocationResolver(); // TODO: we have persistent map: email -> site, building, floor
			connector.ActiveLocations = locations.OfSite(currentSite); // filter locations by site

			connector.Connect();
			var roomsNearby = connector.GetActiveRooms();
			var roomsWithLocations = locations.ResolveLocations(roomsNearby);


		}
	}
}
