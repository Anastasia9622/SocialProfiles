using SocialProfiles.Core.Domain.Exceptions;

namespace SocialProfiles.Core.Domain.Social;

public class Follow
{
    public string FollowerId { get; private set; }
    public string FollowedId { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public Follow(string followerId, string followedId)
    {
        if (string.IsNullOrWhiteSpace(followerId) || string.IsNullOrWhiteSpace(followedId))
            throw new DomainException("Не вказано користувача для підписки.");
        if (followerId == followedId)
            throw new DomainException("Не можна підписатися на себе.");

        FollowerId = followerId;
        FollowedId = followedId;
    }
}