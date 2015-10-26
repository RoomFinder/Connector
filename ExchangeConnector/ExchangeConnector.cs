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
		private readonly string _serviceEmail;

		public ExchangeConnector(string username, string password, string serviceEmail)
		{
			_serviceEmail = serviceEmail;
			_service = new ExchangeService(ExchangeVersion.Exchange2010)
			{
				Credentials = new WebCredentials(username, password)
			};

			// service.TraceEnabled = true;
			// service.TraceFlags = TraceFlags.All;
		}

		public StringCollection ActiveGroups { get; set; }

		public void Connect()
		{
			_service.AutodiscoverUrl(_serviceEmail, RedirectionUrlValidationCallback);
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

		private IEnumerable<EmailAddress> LoadRooms(EmailAddress emailAddress)
		{
			return _service.GetRooms(emailAddress);
		}

		private bool FilterActive(EmailAddress emailAddress)
		{
			if (ActiveGroups == null)
				return true;

			return ActiveGroups.Contains(emailAddress.Address);
		}
	}
}
