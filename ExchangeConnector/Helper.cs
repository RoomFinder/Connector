using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using FindFreeRoom.ExchangeConnector.Base;
using Microsoft.Exchange.WebServices.Data;

namespace FindFreeRoom.ExchangeConnector
{
	public static class Helper
	{
		public static TimeInterval CollapseCalendar(IEnumerable<TimeInterval> events)
		{
			var interval = new TimeInterval(DateTime.Now, DateTime.Now.AddHours(10));
			foreach (var ev in events)
			{
				interval = interval - ev;
				Debug.WriteLine($"Event {ev.Start}-{ev.End}");
			}
			Debug.WriteLine($"Narrowed down to {interval.Start}:{interval.End}");
			return interval;
		}
	}
}