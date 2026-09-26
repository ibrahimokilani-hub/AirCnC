using HotelBooking.Core.Domain.ValueObjects;

namespace HotelBooking.Tests.Unit.Domain;

public sealed class GeoDistanceTests
{
    [Fact]
    public void Meters_SamePoint_IsZero() =>
        Assert.Equal(0, GeoDistance.Meters(32.46m, 35.3m, 32.46m, 35.3m));

    /// <summary>One degree of latitude is about 111 km anywhere on the globe.</summary>
    [Fact]
    public void Meters_OneDegreeOfLatitude_IsAboutOneHundredAndElevenKilometres() =>
        Assert.InRange(GeoDistance.Meters(32m, 35m, 33m, 35m), 110_000, 112_000);

    [Fact]
    public void Meters_IsTheSameInEitherDirection() =>
        Assert.Equal(
            GeoDistance.Meters(32.46m, 35.3m, 31.9m, 35.2m),
            GeoDistance.Meters(31.9m, 35.2m, 32.46m, 35.3m));
}