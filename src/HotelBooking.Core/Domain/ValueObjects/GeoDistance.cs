namespace HotelBooking.Core.Domain.ValueObjects;

public static class GeoDistance
{
    private const double EarthRadiusMeters = 6_371_000;

    public static int Meters(decimal fromLatitude, decimal fromLongitude, decimal toLatitude, decimal toLongitude)
    {
        var lat1 = ToRadians((double)fromLatitude);
        var lat2 = ToRadians((double)toLatitude);
        var deltaLat = ToRadians((double)(toLatitude - fromLatitude));
        var deltaLon = ToRadians((double)(toLongitude - fromLongitude));

        var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                Math.Cos(lat1) * Math.Cos(lat2) * Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return (int)Math.Round(EarthRadiusMeters * c);
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180;
}