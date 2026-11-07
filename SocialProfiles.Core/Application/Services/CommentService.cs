using SocialProfiles.Core.Application.Interfaces;
using SocialProfiles.Core.Domain.Exceptions;
using SocialProfiles.Core.Domain.Social;

namespace SocialProfiles.Core.Application.Services;

public class CommentService
{
    private readonly ICommentRepository _comments;
    private readonly IProfileRepository _profiles;

    public CommentService(ICommentRepository comments, IProfileRepository profiles)
    {
        _comments = comments;
        _profiles = profiles;
    }

    public Task<IReadOnlyList<Comment>> GetWallAsync(Guid profileId) =>
        _comments.GetByProfileAsync(profileId);

    public async Task<Comment> AddAsync(Guid profileId, string authorId, string text)
    {
        var wall = await _profiles.GetByIdAsync(profileId)
                   ?? throw new DomainException("Профіль не знайдено.");

        var comment = new Comment(wall.Id, authorId, text);
        await _comments.AddAsync(comment);
        return comment;
    }

    public async Task<Comment> ReplyAsync(Guid parentId, string authorId, string text)
    {

        var parent = await _comments.GetByIdAsync(parentId)
                     ?? throw new DomainException("Коментар не знайдено.");

        var reply = parent.CreateReply(authorId, text);
        await _comments.AddAsync(reply);
        return reply;
    }

    public async Task DeleteAsync(Guid commentId, string userId)
    {
        var comment = await _comments.GetByIdAsync(commentId)
                      ?? throw new DomainException("Коментар не знайдено.");

        var wall = await _profiles.GetByIdAsync(comment.ProfileId)
                   ?? throw new DomainException("Профіль не знайдено.");

        if (!comment.CanBeDeletedBy(userId, wall.UserId))
            throw new DomainException("Немає прав на видалення цього коментаря.");

        // Разом із головним коментарем видаляємо й відповіді на нього.
        var wallComments = await _comments.GetByProfileAsync(comment.ProfileId);
        var replies = wallComments.Where(c => c.ParentCommentId == comment.Id).ToList();

        foreach (var reply in replies)
            await _comments.DeleteAsync(reply);

        await _comments.DeleteAsync(comment);
    }
}