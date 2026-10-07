using ChatApp.Common.Abstractions;
using ChatApp.Common.Infrastructure.Messaging.Events;
using ChatApp.Common.Infrastructure.Outbox;
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
    private readonly IOutboxWriter _outboxWriter;

    public CreateUserProfileHandler(
        UserDbContext dbContext,
        IOutboxWriter outboxWriter,
        ILogger<CreateUserProfileHandler> logger
    )
    {
        _dbContext = dbContext;
        _logger = logger;
        _outboxWriter = outboxWriter;
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

        _outboxWriter.Enqueue(
            new UserProfileUpdatedEvent(
                Id: request.Id,
                DisplayName: request.DisplayName,
                AvatarUrl: null
            )
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
