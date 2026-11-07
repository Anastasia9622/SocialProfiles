using SocialProfiles.Core.Application.Interfaces;
using SocialProfiles.Core.Domain.Profiles;

namespace SocialProfiles.Tests.Fakes;

public class InMemoryProfileRepository : IProfileRepository
{
    private readonly List<Profile> _items = new();

    public int SaveCount { get; private set; }

    public Task<Profile?> GetByIdAsync(Guid id) =>
        Task.FromResult(_items.FirstOrDefault(p => p.Id == id));

    public Task<Profile?> GetByUserIdAsync(string userId) =>
        Task.FromResult(_items.FirstOrDefault(p => p.UserId == userId));

    public Task<Profile?> GetByUsernameAsync(string username) =>
        Task.FromResult(_items.FirstOrDefault(p =>
            string.Equals(p.Username, username, StringComparison.OrdinalIgnoreCase)));

    public Task<bool> UsernameExistsAsync(string username) =>
        Task.FromResult(_items.Any(p =>
            string.Equals(p.Username, username, StringComparison.OrdinalIgnoreCase)));

    public Task AddAsync(Profile profile)
    {
        _items.Add(profile);
        return Task.CompletedTask;
    }

    public Task SaveAsync(Profile profile)

    {
        SaveCount++;
        return Task.CompletedTask;
    }
}