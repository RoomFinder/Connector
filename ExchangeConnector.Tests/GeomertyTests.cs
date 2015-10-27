using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FindFreeRoom.ExchangeConnector.Base;
using NUnit.Framework;

namespace ExchangeConnector.Tests
{
	[TestFixture]
	public class GeomertyTests
	{
		[Test]
		public void DistanceTest()
		{
			var locs = new[]
			{
				new Geometry(37.324761, -121.999824, 120.0),
				new Geometry(37.324761, -121.999824, 140.0),
				new Geometry(37.325322, -121.997707, 140.0),
				new Geometry(37.326043, -121.998811, 120.0),
				new Geometry(37.326043, -121.998811, 140.0),
				new Geometry(37.325911, -122.000898, 120.0),
				new Geometry(37.325911, -122.000898, 140.0),
				new Geometry(37.324953, -121.998867, 120.0),
				new Geometry(37.324953, -121.998867, 140.0)
			};

			var me = new Geometry(37.324761, -121.999824, 120.0);

			foreach (var loc in locs)
			{
				Debug.WriteLine(Distance.Calculate(me, loc));
			}
		}
	}
}
