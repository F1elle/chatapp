namespace ChatApp.Auth.Common.Abstractions;

public interface IHandler<in TRequest, TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken ct = default);
}