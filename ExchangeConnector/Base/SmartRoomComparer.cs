using System;
using System.Collections.Generic;

namespace FindFreeRoom.ExchangeConnector.Base
{
	public class SmartRoomComparer : IComparer<RoomAvailabilityInfo>
	{
		private static readonly TimeSpan NegligibleWaitingTime = TimeSpan.FromMinutes(5);
		private static readonly TimeSpan MinimalDesiredMeetingTime = TimeSpan.FromMinutes(25);
		// How harder it is to wait for a minute than to walk one meter
		private static readonly double TimeToDistanceImportanceRatio = 10;
		private readonly Location _location;

		public SmartRoomComparer(Location location)
		{
			_location = location;
		}

		public int Compare(RoomAvailabilityInfo x, RoomAvailabilityInfo y)
		{
			bool xAvailable = x.Availability.Start < DateTime.Now + NegligibleWaitingTime && x.Availability.End > DateTime.Now + MinimalDesiredMeetingTime;
			bool yAvailable = y.Availability.Start < DateTime.Now + NegligibleWaitingTime && y.Availability.End > DateTime.Now + MinimalDesiredMeetingTime;
			if (xAvailable != yAvailable)
			{
				return xAvailable ? -1 : 1;
			}
			double distanceToX = Distance.Calculate(_location, x.Room.Location);
			double distanceToY = Distance.Calculate(_location, y.Room.Location);
			double xAvailableIn = Math.Max((x.Availability.Start - DateTime.Now).TotalMinutes, 0);
			double yAvailableIn = Math.Max((y.Availability.Start - DateTime.Now).TotalMinutes, 0);
			double xDifficulty = xAvailableIn * TimeToDistanceImportanceRatio + distanceToX;
			double yDifficulty = yAvailableIn * TimeToDistanceImportanceRatio + distanceToY;

			// room is better if its difficulty is less
			return xDifficulty.CompareTo(yDifficulty);
		}
	}
}