using System;

namespace FindFreeRoom.ExchangeConnector.Base
{
	public class Distance
	{
		public double DegreeToMeters = 1.0;
		public double DefaultDistance = 1000.0; // in case if location geometry is unknown, it is always 1 kilometer 

		public double Calculate(Location from, Location to)
		{
			if (from.Geometry == null || to.Geometry == null)
			{
				return DefaultDistance;
			}
			return CalculateP(from.Geometry, to.Geometry) + Math.Abs(from.Geometry.Elevation - to.Geometry.Elevation);
		}

		public static double Calculate(Geometry from, Geometry to)
		{
			return CalculateP(from, to) + Math.Abs(from.Elevation - to.Elevation);
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