using Microsoft.EntityFrameworkCore;
using SocialProfiles.Core.Application.Interfaces;
using SocialProfiles.Core.Domain.Social;

namespace SocialProfiles.Web.Data.Repositories;

public class EfFollowRepository : IFollowRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;

    public EfFollowRepository(IDbContextFactory<ApplicationDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<bool> ExistsAsync(string followerId, string followedId)
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Follows.AnyAsync(f => f.FollowerId == followerId && f.FollowedId == followedId);
    }

    public async Task AddAsync(Follow follow)
    {
        await using var db = await _factory.CreateDbContextAsync();
        db.Follows.Add(follow);
        await db.SaveChangesAsync();
    }

    public async Task RemoveAsync(string followerId, string followedId)
    {
        await using var db = await _factory.CreateDbContextAsync();
        await db.Follows

            .Where(f => f.FollowerId == followerId && f.FollowedId == followedId)
            .ExecuteDeleteAsync();
    }

    public async Task<int> CountFollowersAsync(string userId)
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Follows.CountAsync(f => f.FollowedId == userId);
    }

    public async Task<int> CountFollowingAsync(string userId)
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Follows.CountAsync(f => f.FollowerId == userId);
    }
}