using HotelBooking.Core.Domain.Common;

namespace HotelBooking.Core.Application.Abstractions.Messaging;

public interface IQuery<TResponse>;

public interface IQueryHandler<in TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    Task<Result<TResponse>> HandleAsync(TQuery query, CancellationToken cancellationToken);
}
