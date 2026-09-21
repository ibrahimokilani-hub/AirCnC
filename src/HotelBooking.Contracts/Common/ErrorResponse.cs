using System.Text.Json.Serialization;

namespace HotelBooking.Contracts.Common;

public sealed record ErrorResponse(
    int Status,
    string Message,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    IReadOnlyDictionary<string, string[]>? Errors = null);