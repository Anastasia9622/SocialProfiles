using SocialProfiles.Core.Application.Interfaces;
using SocialProfiles.Core.Domain.Social;

namespace SocialProfiles.Tests.Fakes;

public class InMemoryCommentRepository : ICommentRepository
{
    private readonly List<Comment> _items = new();

    public int Count => _items.Count;

    public Task<Comment?> GetByIdAsync(Guid id) =>
        Task.FromResult(_items.FirstOrDefault(c => c.Id == id));

    public Task<IReadOnlyList<Comment>> GetByProfileAsync(Guid profileId) =>
        Task.FromResult<IReadOnlyList<Comment>>(
            _items.Where(c => c.ProfileId == profileId).OrderBy(c => c.CreatedAt).ToList());

    public Task AddAsync(Comment comment)
    {
        _items.Add(comment);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Comment comment)
    {
        _items.Remove(comment);
        return Task.CompletedTask;
    }
}