﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using FindFreeRoom.ExchangeConnector.Base;

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
			connector.LocationFilter = locations.OfSite(currentSite).ToArray(); // filter locations by site

			connector.Connect();
			var roomsNearby = connector.GetFilteredRooms();
			var roomsWithLocations = locations.ResolveLocations(roomsNearby);

			foreach (var room in connector.GetAvaialility(roomsWithLocations))
			{
				if (room.Availability == TimeInterval.Zero)
				{
					Console.WriteLine($"{room.Room.Name} is not available in the nearest future");
				}
				else
				{
					Console.WriteLine($"{room.Room.Name} is available from {room.Availability.Start} for {room.Availability.Duration.TotalMinutes} minutes");
				}
			}
		}
	}
}
