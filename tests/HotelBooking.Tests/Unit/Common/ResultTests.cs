using HotelBooking.Core.Domain.Common;

namespace HotelBooking.Tests.Unit.Common;

public sealed class ResultTests
{
    private static readonly Error NotFound = Error.NotFound("City.NotFound", "No such city.");

    [Fact]
    public void Success_HasNoError()
    {
        var result = Result.Success();

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Failure_CarriesTheError()
    {
        var result = Result<int>.Failure(NotFound);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NotFound, result.Error!.Type);
        Assert.Equal("City.NotFound", result.Error.Code);
    }

    [Fact]
    public void Value_OnSuccess_ReturnsIt() =>
        Assert.Equal(7, Result<int>.Success(7).Value);

    /// <summary>Reading the value of a failure is a bug in the caller, not a runtime condition.</summary>
    [Fact]
    public void Value_OnFailure_Throws() =>
        Assert.Throws<InvalidOperationException>(() => Result<int>.Failure(NotFound).Value);
}