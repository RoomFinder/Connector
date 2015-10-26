using System;
using System.Collections.Generic;
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
				if (items.Length != 4)
				{
					// TODO: Specific exception
					throw new Exception($"Invalid data in {locationMapFileName}");
				}
				_locationMap.Add(items[0], new Location
				{
					Site = items[1],
					Building = items[2],
					Floor = items[3]
				});
			}
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
