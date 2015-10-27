using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading.Tasks;
using FindFreeRoom.ExchangeConnector;

namespace ConnectorWebService
{
	[ServiceContract]
	public interface IService
	{
		[OperationContract]
		[WebGet(UriTemplate = "rooms?ticket={ticket}", ResponseFormat = WebMessageFormat.Json)]
		IEnumerable<RoomDataContract> GetRooms(string ticket);

		[OperationContract]
		[WebGet(UriTemplate = "rooms/{roomId}?ticket={ticket}", ResponseFormat = WebMessageFormat.Json)]
		RoomDataContract GetRoom(string roomId, string ticket);

		[OperationContract]
		[WebInvoke(Method = "POST", UriTemplate = "rooms/{roomId}?ticket={ticket}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		Task ReserveRoom(int duration, string roomId, string ticket);

		[OperationContract]
		[WebInvoke(Method = "POST", UriTemplate = "login", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		string Login(string username, string password, string email, string site, string serviceUrl);
	}
}