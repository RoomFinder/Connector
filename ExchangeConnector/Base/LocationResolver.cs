using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;

namespace FindFreeRoom.ExchangeConnector.Base
{
	public class LocationResolver
	{
		// maps RoomList email to its location
		private readonly Dictionary<string, Location> _locationMap = new Dictionary<string, Location>();

		public void Load(string locationMapFileName)
		{
			_locationMap.Clear();
			foreach (var line in File.ReadAllLines(locationMapFileName))
			{
				var items = line.Split(',', ';');
				if (items.Length != 4 && items.Length != 7)
				{
					// TODO: Specific exception
					throw new Exception($"Invalid data in {locationMapFileName}");
				}
				_locationMap.Add(items[0], CreateLocation(items));
			}
		}

		private static Location CreateLocation(string[] items)
		{
			var location = new Location
			{
				Site = items[1],
				Building = items[2],
				Floor = items[3]
			};
			if (items.Length == 7)
			{
				location.Geometry = new Geometry
				{
					X = float.Parse(items[4]),
					Y = float.Parse(items[5]),
					Elevation = float.Parse(items[6])
				};
			}
			return location;
		}

		public IEnumerable<string> OfSite(string currentSite)
		{
			return _locationMap
				.Where(x => string.Equals(x.Value.Site, currentSite, StringComparison.InvariantCultureIgnoreCase))
				.Select(x => x.Key);
		}
		
		public IEnumerable<RoomInfo> ResolveLocations(IEnumerable<RoomInfo> rooms)
		{
			return rooms.Select(x => new RoomInfo {
				RoomId = x.RoomId,
				Name = x.Name,
				LocationId = x.LocationId,
				Location = _locationMap[x.LocationId]
			});
		}
	}
}
