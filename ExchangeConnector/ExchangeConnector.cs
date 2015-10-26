using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using FindFreeRoom.ExchangeConnector.Base;
using Microsoft.Exchange.WebServices.Data;

namespace FindFreeRoom.ExchangeConnector
{
	public class ExchangeConnector
	{
		private readonly ExchangeService _service;
		private readonly string _serverUrl;
		private readonly string _serviceEmail;

		public ExchangeConnector(string username, string password, string serverUrl, string serviceEmail)
		{
			_serverUrl = serverUrl;
			_serviceEmail = serviceEmail;
			_service = new ExchangeService(ExchangeVersion.Exchange2010);
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

		public void PrintAvaialility(IEnumerable<RoomInfo> rooms)
		{
			var attendees =
				rooms.Select(
					r => new AttendeeInfo {AttendeeType = MeetingAttendeeType.Room, ExcludeConflicts = false, SmtpAddress = r.RoomId}).ToList();
			attendees.Add(new AttendeeInfo()
			{
				SmtpAddress = _serviceEmail,
				AttendeeType = MeetingAttendeeType.Organizer
			});

			var timeWindow = new TimeWindow(DateTime.Now, DateTime.Now.AddDays(1));

			AvailabilityOptions options = new AvailabilityOptions
			{	
				MeetingDuration = 30,
				RequestedFreeBusyView = FreeBusyViewType.FreeBusy
			};
			var availabilities = _service.GetUserAvailability(attendees, timeWindow, AvailabilityData.FreeBusy, options);

			var table = attendees.Zip(availabilities.AttendeesAvailability, (attendee, availability) =>
			{
				var info = Helper.CollapseCalendar(availability.CalendarEvents.Select(x => new TimeInterval(x.StartTime, x.EndTime)));
				return new Tuple<string, DateTime, TimeSpan>(attendee.SmtpAddress, info.Start, info.End - info.Start);
			});
			foreach (var tuple in table)
			{
				Console.WriteLine($"{tuple.Item1} is available from {tuple.Item2} for {tuple.Item3}");
			}
		}
	}
}
