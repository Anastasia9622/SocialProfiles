using SocialProfiles.Core.Domain.Exceptions;

namespace SocialProfiles.Core.Domain.Social;

public class Comment
{
    public const int MaxLength = 500;

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid ProfileId { get; private set; }
    public string AuthorId { get; private set; }
    public Guid? ParentCommentId { get; private set; }
    public string Text { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public bool IsReply => ParentCommentId.HasValue;

    // Публічний конструктор створює головний коментар.
    public Comment(Guid profileId, string authorId, string text)
        : this(profileId, authorId, text, null)
    {
    }

    // Приватний конструктор використовується також для створення відповідей.
    private Comment(Guid profileId, string authorId, string text, Guid? parentId)
    {
        if (profileId == Guid.Empty)
            throw new DomainException("Не вказано стіну для коментаря.");
        if (string.IsNullOrWhiteSpace(authorId))
            throw new DomainException("Не вказано автора коментаря.");

        text = text?.Trim() ?? "";
        if (text.Length == 0 || text.Length > MaxLength)
            throw new DomainException($"Коментар має містити від 1 до {MaxLength} символів.");

        ProfileId = profileId;
        AuthorId = authorId;
        Text = text;
        ParentCommentId = parentId;
    }

    public Comment CreateReply(string authorId, string text)
    {
        if (IsReply)
            throw new DomainException("Відповідати можна лише на головний коментар.");

        return new Comment(ProfileId, authorId, text, Id);
    }

    public bool CanBeDeletedBy(string userId, string wallOwnerId) =>
        !string.IsNullOrWhiteSpace(userId) &&
        (userId == AuthorId || userId == wallOwnerId);
}