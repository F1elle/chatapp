namespace ChatApp.Auth.Features.TokenRefresh;

public sealed record TokenRefreshRequest(string RefreshToken);

public sealed record TokenRefreshResponse(
    string AccessToken,
    string RefreshToken
);