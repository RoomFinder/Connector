using System;
using System.Globalization;
using FindFreeRoom.ExchangeConnector;

namespace ConnectorWebService
{
	public static class Convertions
	{
		public static RoomDataContract ToContract(this RoomAvailabilityInfo info)
		{
			var contract = new RoomDataContract
			{
				Id = info.Room.RoomId,
				Name = info.Room.Name,
				AvailableFrom = info.Availability.Start.ToRfcDateTimeString(),
				AvailableForMinutes = (int)info.Availability.Duration.TotalMinutes
			};
			if (info.Room.Location != null)
			{
				contract.Site = info.Room.Location.Site;
				contract.Building = info.Room.Location.Building;
				contract.Floor = info.Room.Location.Floor;
			}
			return contract;
		}

		private const string Rfc3339Format = "yyyy-MM-dd'T'HH:mm:ss.fffK";
		public static string ToRfcDateTimeString(this DateTime dt)
		{
			return DateTime.SpecifyKind(dt, DateTimeKind.Utc)
				.ToString(Rfc3339Format, DateTimeFormatInfo.InvariantInfo);
		}
	}
}