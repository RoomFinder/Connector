using System.Runtime.Serialization;

namespace ConnectorWebService
{
	[DataContract]
	public class RoomDataContract
	{
		[DataMember(Name = "name")]
		public string Name;
		[DataMember(Name = "availableFrom")]
		public string AvailableFrom;
		[DataMember(Name = "availableForMinutes")]
		public int AvailableForMinutes;
	}
}
