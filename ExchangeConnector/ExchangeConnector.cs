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
	}
}
