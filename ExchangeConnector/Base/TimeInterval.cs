using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindFreeRoom.ExchangeConnector.Base
{
	public class TimeInterval
	{
		public static readonly TimeInterval Zero = new TimeInterval(DateTime.MinValue, DateTime.MinValue);

		public DateTime Start;
		public DateTime End;

		public TimeSpan Duration => End - Start;

		public TimeInterval(DateTime start, DateTime end)
		{
			Start = start;
			End = end;
		}

		public static TimeInterval operator - (TimeInterval first, TimeInterval second)
		{
			if (first == null) throw new ArgumentNullException(nameof(first));
			if (second == null) throw new ArgumentNullException(nameof(second));
			if (second.End <= first.Start || second.Start >= first.End)
			{
				return first;
			}

			if (second.Start <= first.Start && second.End >= first.End)
			{
				return Zero;
			}

			if (second.Start <= first.Start)
			{
				return new TimeInterval(second.End, first.End);
			}

			return new TimeInterval(first.Start, second.Start);
		} 
	}
}
