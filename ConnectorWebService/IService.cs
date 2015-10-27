using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using FindFreeRoom.ExchangeConnector;

namespace ConnectorWebService
{
	[ServiceContract]
	public interface IService
	{
		[OperationContract]
		[WebGet(UriTemplate = "", ResponseFormat = WebMessageFormat.Json)]
		IEnumerable<RoomDataContract> GetRoomAvailabilityInfo();
	}
}