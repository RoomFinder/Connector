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
			var x = from.Geometry.X - to.Geometry.X;
			var y = from.Geometry.Y - to.Geometry.Y;
			return Math.Sqrt(x*x + y*y);
		}
	}
}