using HotelBooking.Core.Domain.Common;

namespace HotelBooking.Core.Application.Abstractions.Messaging;

public interface ICommand<TResponse>;

public interface ICommandHandler<in TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    Task<Result<TResponse>> HandleAsync(TCommand command, CancellationToken cancellationToken);
}
