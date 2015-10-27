using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FindFreeRoom.ExchangeConnector.Base;
using Microsoft.Exchange.WebServices.Data;
using Task = System.Threading.Tasks.Task;

namespace FindFreeRoom.ExchangeConnector
{
	public class ExchangeConnector
	{
		private static readonly TimeSpan CreationTimeout = TimeSpan.FromSeconds(60);
		private readonly ExchangeService _service = new ExchangeService(ExchangeVersion.Exchange2010);
		private readonly string _serverUrl;
		private readonly string _serviceEmail;

		public ExchangeConnector(string serviceEmail) :
			this(null, null, null, serviceEmail)
		{
		}

		public ExchangeConnector(string username, string password, string serverUrl, string serviceEmail)
		{
			if (serviceEmail == null) throw new ArgumentNullException(nameof(serviceEmail));

			_serverUrl = serverUrl;
			_serviceEmail = serviceEmail;

			if (string.IsNullOrEmpty(username))
			{
				_service.UseDefaultCredentials = true;
			}
			else
			{
				_service.Credentials = new WebCredentials(username, password);
			}

			//_service.TraceEnabled = true;
			//_service.TraceFlags = TraceFlags.All;
		}

		public void Connect()
		{
			if (string.IsNullOrEmpty(_serverUrl))
			{
				_service.AutodiscoverUrl(_serviceEmail, RedirectionUrlValidationCallback);
			}
			else
			{
				_service.Url = new Uri(_serverUrl);
			}
		}

		private static bool RedirectionUrlValidationCallback(string redirectionUrl)
		{
			// The default for the validation callback is to reject the URL.
			bool result = false;

			Uri redirectionUri = new Uri(redirectionUrl);

			// Validate the contents of the redirection URL. In this simple validation
			// callback, the redirection URL is considered valid if it is using HTTPS
			// to encrypt the authentication credentials. 
			if (redirectionUri.Scheme == "https")
			{
				result = true;
			}
			return result;
		}

		public string[] LocationFilter { get; set; }

		public IEnumerable<RoomInfo> GetFilteredRooms()
		{
			foreach (var list in _service.GetRoomLists().Where(FilterActive))
			{
				foreach (var room in LoadRooms(list))
				{
					yield return new RoomInfo
					{
						LocationId = list.Address,
						RoomId = room.Address,
						Name = room.Name
					};
				}
			}
		}

		private IEnumerable<EmailAddress> LoadRooms(EmailAddress emailAddress)
		{
			return _service.GetRooms(emailAddress);
		}

		private bool FilterActive(EmailAddress emailAddress)
		{
			if (LocationFilter == null)
				return true;

			return LocationFilter.Contains(emailAddress.Address);
		}

		public IEnumerable<string> GetAllRoomLists()
		{
			return _service.GetRoomLists().Select(x => x.Address);
		}

		public IEnumerable<RoomAvailabilityInfo> GetAvaialility(IEnumerable<RoomInfo> rooms)
		{
			// TODO: Investigate
			const int chunkSize = 40;
			var roomsArray = rooms.ToArray();
			var result = Enumerable.Empty<RoomAvailabilityInfo>();
			for (int i = 0; i < roomsArray.Length; i += chunkSize)
			{
				result = result.Concat(GetAvaialilityInternal(roomsArray.Skip(i).Take(chunkSize)));
			}
			return result;
		}

		private IEnumerable<RoomAvailabilityInfo> GetAvaialilityInternal(IEnumerable<RoomInfo> rooms)
		{
			var roomsArray = rooms.ToArray();
			var attendees =
				roomsArray.Select(
					r => new AttendeeInfo { AttendeeType = MeetingAttendeeType.Room, ExcludeConflicts = false, SmtpAddress = r.RoomId }).ToList();
			var timeWindow = new TimeWindow(DateTime.Now, DateTime.Now.AddDays(1));

			AvailabilityOptions options = new AvailabilityOptions
			{
				MeetingDuration = 30,
				RequestedFreeBusyView = FreeBusyViewType.FreeBusy
			};
			var availabilities = _service.GetUserAvailability(attendees, timeWindow, AvailabilityData.FreeBusy, options);

			Debug.Assert(roomsArray.Length == availabilities.AttendeesAvailability.Count, "Invalid server response");
			return roomsArray.Zip(availabilities.AttendeesAvailability, (room, availability) =>
			{
				var info = Helper.CollapseCalendar(availability.CalendarEvents.Select(x => new TimeInterval(x.StartTime, x.EndTime)));
				return new RoomAvailabilityInfo(info, room);
			});
		}

		public Task<bool> ReserveRoom(RoomInfo room, TimeSpan duration)
		{
			Appointment appointment = new Appointment(_service);

			// Set the properties on the appointment object to create the appointment.
			appointment.Subject = "Room reservation";
			appointment.Body = $"Automatically created by FindMeRoomOSS on {DateTime.Now}";
			appointment.Start = DateTime.Now;
			appointment.End = appointment.Start + duration;
			appointment.Location = room.RoomId;
			appointment.RequiredAttendees.Add(room.RoomId);

			// Save the appointment to your calendar.
			appointment.Save(SendInvitationsMode.SendToAllAndSaveCopy);

			PropertySet psPropSet = new PropertySet(BasePropertySet.FirstClassProperties);

			// Verify that the appointment was created by using the appointment's item ID.
			appointment = Item.Bind(_service, appointment.Id, psPropSet) as Appointment;
			if (appointment == null)
			{
				return Task.FromResult(false);
			}
			return Task.Run(async () =>
				{
					var finishTime = DateTime.Now + CreationTimeout;
					Attendee roomAttendee;
					// wait till the room accepts
					do
					{
						Debug.WriteLine("Waiting for response...");
						await Task.Delay(1000);

						// refresh appointment data
						appointment = Item.Bind(_service, appointment.Id, psPropSet) as Appointment;
						if (appointment == null)
						{
							Debug.WriteLine("Appointment has been deleted");
							return false;
						}
						roomAttendee = appointment.RequiredAttendees.FirstOrDefault(att => att.Address == room.RoomId);
						if (roomAttendee == null)
						{
							Debug.WriteLine("Someone is messing with our appointment");
							return false;
						}
					} while (DateTime.Now < finishTime && (
							!roomAttendee.ResponseType.HasValue ||
							roomAttendee.ResponseType == MeetingResponseType.Unknown ||
							roomAttendee.ResponseType == MeetingResponseType.NoResponseReceived
						));
					Debug.WriteLine($"Response is {roomAttendee.ResponseType}...");
					return roomAttendee.ResponseType.HasValue && roomAttendee.ResponseType.Value == MeetingResponseType.Accept;
				});
		}
	}
}
