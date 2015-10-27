using System;
using System.Collections.Generic;
using System.ServiceModel.Security.Tokens;
using FindFreeRoom.ExchangeConnector;
using FindFreeRoom.ExchangeConnector.Base;

namespace ConnectorWebService
{
	public class Service : IService
	{
		public IEnumerable<RoomDataContract> GetRoomAvailabilityInfo()
		{
			var info = new RoomAvailabilityInfo(
				new TimeInterval(DateTime.Now, DateTime.Now.AddMinutes(60)),
				new RoomInfo
				{
					RoomId = "room@company.com",
					Location = new Location {
						Site = "City",
						Building = "Building #1",
						Floor = "Lower"
					},
					LocationId = "building1_lower@company.com",
					Name = "Room"
				});
			yield return info.ToContract();
		}
	}
}
