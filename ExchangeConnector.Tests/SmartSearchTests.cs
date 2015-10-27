using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FindFreeRoom.ExchangeConnector;
using FindFreeRoom.ExchangeConnector.Base;
using NUnit.Framework;

namespace ExchangeConnector.Tests
{
	[TestFixture]
	public class SmartSearchTests
	{
		private readonly Location _center = new Location { Geometry = new Geometry(0, 0, 0) };
		private TimeInterval _availableNow;
		private TimeInterval _availableVerySoon;
		private TimeInterval _availableLater;
		private TimeInterval _availableForShortPeriod;

		[SetUp]
		public void SetUp()
		{
			_availableNow = new TimeInterval(DateTime.Now, DateTime.Now.AddHours(1));
			_availableVerySoon = new TimeInterval(DateTime.Now.AddMinutes(3), DateTime.Now.AddHours(1));
			_availableLater = new TimeInterval(DateTime.Now.AddHours(2), DateTime.Now.AddHours(4));
			_availableForShortPeriod = new TimeInterval(DateTime.Now, DateTime.Now.AddMinutes(13));
		}

		[Test]
		public void DistanceComparerTest()
		{
			var room1 = new RoomAvailabilityInfo(_availableNow, new RoomInfo
			{
				Location = new Location
				{
					Geometry = new Geometry(1, 1, 0)
				}
			});
			var room2 = new RoomAvailabilityInfo(_availableNow, new RoomInfo
			{
				Location = new Location
				{
					Geometry = new Geometry(2, 2, 0)
				}
			});
			var comparer = new SmartRoomComparer(_center);
			Assert.That(comparer.Compare(room1, room2), Is.LessThan(0));
			Assert.That(comparer.Compare(room2, room1), Is.GreaterThan(0));

			Assert.That(new SmartSearch(_center).Sort(new[] { room1, room2 }), Is.EqualTo(new[] { room1, room2 }));
		}

		[Test]
		public void NegligibleTimeComparerTest()
		{
			var room1 = new RoomAvailabilityInfo(_availableVerySoon, new RoomInfo
			{
				Location = new Location
				{
					Geometry = new Geometry(1, 1, 0)
				}
			});
			var room2 = new RoomAvailabilityInfo(_availableNow, new RoomInfo
			{
				Location = new Location
				{
					Geometry = new Geometry(2, 2, 0)
				}
			});
			var comparer = new SmartRoomComparer(_center);
			Assert.That(comparer.Compare(room1, room2), Is.LessThan(0));
			Assert.That(comparer.Compare(room2, room1), Is.GreaterThan(0));

			Assert.That(new SmartSearch(_center).Sort(new[] { room1, room2 }), Is.EqualTo(new[] { room1, room2 }));
		}

		[Test]
		public void NonNegligibleTimeComparerTest()
		{
			var room1 = new RoomAvailabilityInfo(_availableLater, new RoomInfo
			{
				Location = new Location
				{
					Geometry = new Geometry(1, 1, 0)
				}
			});
			var room2 = new RoomAvailabilityInfo(_availableNow, new RoomInfo
			{
				Location = new Location
				{
					Geometry = new Geometry(2, 2, 0)
				}
			});
			var comparer = new SmartRoomComparer(_center);
			Assert.That(comparer.Compare(room1, room2), Is.GreaterThan(0));
			Assert.That(comparer.Compare(room2, room1), Is.LessThan(0));

			Assert.That(new SmartSearch(_center).Sort(new[] { room1, room2 }), Is.EqualTo(new[] { room2, room1 }));
		}

		[Test]
		public void SmallWindowComparerTest()
		{
			var room1 = new RoomAvailabilityInfo(_availableForShortPeriod, new RoomInfo
			{
				Location = new Location
				{
					Geometry = new Geometry(1, 1, 0)
				}
			});
			var room2 = new RoomAvailabilityInfo(_availableNow, new RoomInfo
			{
				Location = new Location
				{
					Geometry = new Geometry(2, 2, 0)
				}
			});
			var comparer = new SmartRoomComparer(_center);
			Assert.That(comparer.Compare(room1, room2), Is.GreaterThan(0));
			Assert.That(comparer.Compare(room2, room1), Is.LessThan(0));

			Assert.That(new SmartSearch(_center).Sort(new[] { room1, room2 }), Is.EqualTo(new[] { room2, room1 }));
		}
	}
}
