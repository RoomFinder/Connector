using System.Runtime.Serialization;

namespace ConnectorWebService
{
	[DataContract]
	public class RoomDataContract
	{
		[DataMember(Name = "id")]
		public string Id;
		[DataMember(Name = "name")]
		public string Name;
		[DataMember(Name = "availableFrom")]
		public string AvailableFrom;
		[DataMember(Name = "availableForMinutes")]
		public int AvailableForMinutes;

		// extended fields
		[DataMember(Name = "site", EmitDefaultValue = false)]
		public string Site;
		[DataMember(Name = "building", EmitDefaultValue = false)]
		public string Building;
		[DataMember(Name = "floor", EmitDefaultValue = false)]
		public string Floor;
	}
}
