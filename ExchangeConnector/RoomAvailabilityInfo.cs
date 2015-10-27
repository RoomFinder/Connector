using FindFreeRoom.ExchangeConnector.Base;

namespace FindFreeRoom.ExchangeConnector
{
	public class RoomAvailabilityInfo
	{
		public TimeInterval Availability;
		public RoomInfo Room;

		public RoomAvailabilityInfo(TimeInterval availability, RoomInfo room)
		{
			Availability = availability;
			Room = room;
		}
	}
}