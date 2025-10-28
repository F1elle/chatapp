namespace ChatApp.Auth.Features.TokenRefresh;

public record TokenRefreshRequest(string RefreshToken);

public record TokenRefreshResponse(
    string AccessToken,
    string RefreshToken
);