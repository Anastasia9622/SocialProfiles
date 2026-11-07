using SocialProfiles.Core.Domain.Social;

namespace SocialProfiles.Core.Application.Interfaces;

public interface IFollowRepository
{
    Task<bool> ExistsAsync(string followerId, string followedId);
    Task AddAsync(Follow follow);
    Task RemoveAsync(string followerId, string followedId);
    Task<int> CountFollowersAsync(string userId);
    Task<int> CountFollowingAsync(string userId);
}