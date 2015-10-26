using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindFreeRoom.ConnectorConsole
{
	class LocationResolver
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

		public IEnumerable<RoomInfo> ResolveLocations(IEnumerable<Tuple<string, string>> locations)
		{
			return locations.Select(x => new RoomInfo {
				EMail = x.Item2,
				Location = _locationMap[x.Item1]
			});
		}
	}

	internal class RoomInfo
	{
		public string EMail;
		public Location Location;
	}
}
