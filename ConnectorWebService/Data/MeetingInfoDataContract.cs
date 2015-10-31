using System.Runtime.Serialization;

namespace ConnectorWebService
{
	[DataContract]
	public class MeetingInfoDataContract
	{
		[DataMember(Name = "name")]
		public string Name;
		[DataMember(Name = "startTime")]
		public string StartTime;
		[DataMember(Name = "endTime")]
		public string EndTime;

		// extended fields
		[DataMember(Name = "site", EmitDefaultValue = false)]
		public string Site;
		[DataMember(Name = "building", EmitDefaultValue = false)]
		public string Building;
		[DataMember(Name = "floor", EmitDefaultValue = false)]
		public string Floor;
		[DataMember(Name = "room")]
		public string RoomName;
	}
}