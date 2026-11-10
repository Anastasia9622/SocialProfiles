using Microsoft.EntityFrameworkCore;
using SocialProfiles.Core.Application.Interfaces;
using SocialProfiles.Core.Domain.Social;

namespace SocialProfiles.Web.Data.Repositories;

public class EfCommentRepository : ICommentRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;

    public EfCommentRepository(IDbContextFactory<ApplicationDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<Comment?> GetByIdAsync(Guid id)
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Comments.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IReadOnlyList<Comment>> GetByProfileAsync(Guid profileId)
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Comments
            .Where(c => c.ProfileId == profileId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(Comment comment)
    {
        await using var db = await _factory.CreateDbContextAsync();
        db.Comments.Add(comment);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Comment comment)
    {
        await using var db = await _factory.CreateDbContextAsync();
        db.Comments.Remove(comment);
        await db.SaveChangesAsync();
    }
}