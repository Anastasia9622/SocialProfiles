using SocialProfiles.Core.Domain.Social;

namespace SocialProfiles.Core.Application.Interfaces;

public interface ICommentRepository
{
    Task<Comment?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<Comment>> GetByProfileAsync(Guid profileId);
    Task AddAsync(Comment comment);
    Task DeleteAsync(Comment comment);
}