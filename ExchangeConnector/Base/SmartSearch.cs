using System.Collections.Generic;
using System.Linq;

namespace FindFreeRoom.ExchangeConnector.Base
{
	public class SmartSearch
	{
		private readonly IComparer<RoomAvailabilityInfo> _comparer;

		public SmartSearch(Location location)
		{
			_comparer = new SmartRoomComparer(location);
		}

		public IEnumerable<RoomAvailabilityInfo> Sort(IEnumerable<RoomAvailabilityInfo> rooms)
		{
			return rooms.OrderBy(x => x, _comparer);
		}
	}
}
