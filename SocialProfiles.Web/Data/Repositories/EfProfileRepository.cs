using Microsoft.EntityFrameworkCore;
using SocialProfiles.Core.Application.Interfaces;
using SocialProfiles.Core.Domain.Profiles;

namespace SocialProfiles.Web.Data.Repositories;

public class EfProfileRepository : IProfileRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;

    public EfProfileRepository(IDbContextFactory<ApplicationDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<Profile?> GetByIdAsync(Guid id)
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Profiles.Include(p => p.Blocks).FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Profile?> GetByUserIdAsync(string userId)
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Profiles.Include(p => p.Blocks).FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task<Profile?> GetByUsernameAsync(string username)
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Profiles.Include(p => p.Blocks).FirstOrDefaultAsync(p => p.Username == username);
    }


    public async Task<bool> UsernameExistsAsync(string username)
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Profiles.AnyAsync(p => p.Username == username);
    }

    public async Task AddAsync(Profile profile)
    {
        await using var db = await _factory.CreateDbContextAsync();
        db.Profiles.Add(profile);
        await db.SaveChangesAsync();
    }

    public async Task SaveAsync(Profile profile)
    {
        await using var db = await _factory.CreateDbContextAsync();
        db.Profiles.Update(profile);
        await db.SaveChangesAsync();
    }
}