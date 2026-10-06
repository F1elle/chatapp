using ChatApp.Common.Abstractions;
using ChatApp.User.Features.Common;
using ChatApp.User.Infrastructure.Data;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.User.Features.CreateUserProfile;

public class CreateUserProfileHandler
    : IHandler<CreateUserProfileRequest, Result<CreateUserProfileResponse, UserError>>
{
    private readonly UserDbContext _dbContext;
    private readonly ILogger<CreateUserProfileHandler> _logger;

    public CreateUserProfileHandler(
        UserDbContext dbContext,
        ILogger<CreateUserProfileHandler> logger
    )
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<CreateUserProfileResponse, UserError>> Handle(
        CreateUserProfileRequest request,
        CancellationToken ct
    )
    {
        var exists = await _dbContext.UserProfiles.AnyAsync(p => p.Id == request.Id, ct);
        if (exists)
            return new CreateUserProfileResponse();

        _dbContext.UserProfiles.Add(
            new Domain.UserProfile
            {
                Id = request.Id,
                Email = request.Email,
                DisplayName = request.DisplayName,
                CreatedAt = request.CreatedAt,
                UserTag = Guid.CreateVersion7().ToString(),
            }
        );

        try
        {
            await _dbContext.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while creating user profile: {Id} {ex}", request.Id, ex);
            throw;
        }

        return new CreateUserProfileResponse();
    }
}
