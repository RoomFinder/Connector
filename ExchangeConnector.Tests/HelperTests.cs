using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FindFreeRoom.ExchangeConnector.Base;
using NUnit.Framework;

namespace ExchangeConnector.Tests
{
	[TestFixture]
    public class HelperTests
    {
		[Test]
		public void TimeIntervalTests()
		{
			var dateTime = DateTime.Now;
			new TimeInterval(dateTime, DateTime.Now.AddMinutes(5));
		}
    }
}
