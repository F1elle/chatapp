namespace ChatApp.Chat.Common.Abstractions;

public interface IHandler<in TRequest, TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken ct);
}