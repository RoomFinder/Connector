using System;

namespace FindFreeRoom.ExchangeConnector.Base
{
	public static class Distance
	{
		// in case if location geometry is unknown
		private static readonly double DefaultDistanceBetweenSites = 1000000.0; // 1000 km
		private static readonly double DefaultDistanceBetweenBuildings = 1000.0; // 1 km
		private static readonly double DefaultDistanceBetweenFloors = 100.0; // 100 m
		private static readonly double DefaultDistanceBetweenRooms = 20.0; // 20 m

		public static double Calculate(Location from, Location to)
		{
			if (from == null || to == null)
			{
				return DefaultDistanceBetweenSites;
			}
			if (from.Geometry == null || to.Geometry == null)
			{
				if (from.Site != to.Site)
				{
					return DefaultDistanceBetweenSites;
				}
				if (from.Building != to.Building)
				{
					return DefaultDistanceBetweenBuildings;
				}
				if (from.Floor != to.Floor)
				{
					return DefaultDistanceBetweenFloors;
				}
				return DefaultDistanceBetweenRooms;
			}
			return CalculateP(from.Geometry, to.Geometry) + Math.Abs(from.Geometry.Elevation - to.Geometry.Elevation);
		}

		private static double CalculateP(Geometry from, Geometry to) // lat is Y
		{
			var R = 6371000; // km
			var dLat = rad(to.Latitude - from.Latitude);
			var dLon = rad(to.Longitude - from.Longitude);
			var lat1 = rad(from.Latitude);
			var lat2 = rad(to.Latitude);

			var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
					Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
			var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
			return R * c;
		}

		private static double rad(double degrees)
		{
			return degrees*Math.PI/180;
		}
	}
}