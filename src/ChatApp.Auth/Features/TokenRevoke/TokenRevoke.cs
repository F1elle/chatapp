namespace ChatApp.Auth.Features.TokenRevoke;

public sealed record TokenRevokeRequest(string RefreshToken);

public sealed record TokenRevokeResponse();