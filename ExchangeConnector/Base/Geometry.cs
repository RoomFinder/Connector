namespace FindFreeRoom.ExchangeConnector.Base
{
	public class Geometry
	{
		public Geometry(double latitude, double longitude, double elevation)
		{
			Latitude = latitude;
			Longitude = longitude;
			Elevation = elevation;
		}

		public double Latitude;
		public double Longitude;
		public double Elevation;
	}
}