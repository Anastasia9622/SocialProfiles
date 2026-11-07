using SocialProfiles.Core.Application.Interfaces;
using SocialProfiles.Core.Domain.Social;

namespace SocialProfiles.Tests.Fakes;

public class InMemoryFollowRepository : IFollowRepository
{
    private readonly List<Follow> _items = new();

    public Task<bool> ExistsAsync(string followerId, string followedId) =>
        Task.FromResult(_items.Any(f => f.FollowerId == followerId && f.FollowedId == followedId));

    public Task AddAsync(Follow follow)
    {
        _items.Add(follow);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string followerId, string followedId)
    {
        _items.RemoveAll(f => f.FollowerId == followerId && f.FollowedId == followedId);
        return Task.CompletedTask;
    }

    public Task<int> CountFollowersAsync(string userId) =>
        Task.FromResult(_items.Count(f => f.FollowedId == userId));

    public Task<int> CountFollowingAsync(string userId) =>
        Task.FromResult(_items.Count(f => f.FollowerId == userId));
}