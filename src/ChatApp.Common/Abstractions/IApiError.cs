namespace ChatApp.Common.Abstractions;

public interface IApiError
{
    public string Code { get; }
    public int HttpStatus { get; }
}
