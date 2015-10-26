using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
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

		public string[] ActiveLocations { get; set; }

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

		// Print only rooms in active lists or everything
		public void PrintActive()
		{
			var rooms = _service.GetRoomLists().Where(FilterActive).SelectMany(LoadRooms);
			foreach (var room in rooms)
			{
				Console.WriteLine(room);
			}
		}

		public IEnumerable<string> GetActiveRooms()
		{
			return _service.GetRoomLists().Where(FilterActive).SelectMany(LoadRooms).Select(i => i.Address);
		}

		public IEnumerable<Tuple<string, string>> GetFilteredRooms()
		{
			foreach (var list in _service.GetRoomLists().Where(FilterActive))
			{
				foreach (var room in LoadRooms(list))
				{
					yield return new Tuple<string, string>(list.Address, room.Address);
				}
			}
		}

		private IEnumerable<EmailAddress> LoadRooms(EmailAddress emailAddress)
		{
			return _service.GetRooms(emailAddress);
		}

		public IEnumerable<string> LoadRooms(string emailAddress)
		{
			return _service.GetRooms(new EmailAddress(emailAddress)).Select(email => email.Address);
		}

		private bool FilterActive(EmailAddress emailAddress)
		{
			if (ActiveLocations == null)
				return true;

			return ActiveLocations.Contains(emailAddress.Address);
		}
	}
}
