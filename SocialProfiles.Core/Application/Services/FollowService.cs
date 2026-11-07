using SocialProfiles.Core.Application.Interfaces;
using SocialProfiles.Core.Domain.Exceptions;
using SocialProfiles.Core.Domain.Social;

namespace SocialProfiles.Core.Application.Services;

public class FollowService
{
    private readonly IFollowRepository _follows;
    private readonly IProfileRepository _profiles;

    public FollowService(IFollowRepository follows, IProfileRepository profiles)
    {
        _follows = follows;
        _profiles = profiles;
    }

    public async Task FollowAsync(string followerId, string targetUserId)
    {
        // Конструктор Follow сам відхилить порожні значення та підписку на себе.
        var follow = new Follow(followerId, targetUserId);

        if (await _profiles.GetByUserIdAsync(targetUserId) is null)
            throw new DomainException("Користувача не знайдено.");

        if (await _follows.ExistsAsync(followerId, targetUserId))
            throw new DomainException("Ви вже підписані на цього користувача.");

        await _follows.AddAsync(follow);
    }

    public async Task UnfollowAsync(string followerId, string targetUserId)
    {
        if (!await _follows.ExistsAsync(followerId, targetUserId))
            throw new DomainException("Ви не підписані на цього користувача.");

        await _follows.RemoveAsync(followerId, targetUserId);
    }

    public Task<bool> IsFollowingAsync(string followerId, string targetUserId) =>
        _follows.ExistsAsync(followerId, targetUserId);

    public Task<int> CountFollowersAsync(string userId) => _follows.CountFollowersAsync(userId);

    public Task<int> CountFollowingAsync(string userId) => _follows.CountFollowingAsync(userId);
}