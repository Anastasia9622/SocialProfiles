using SocialProfiles.Core.Domain.Exceptions;
using SocialProfiles.Core.Domain.Social;

namespace SocialProfiles.Tests.Domain;

public class FollowTests
{
    [Fact]
    public void Constructor_ValidData_CreatesFollow()
    {
        var follow = new Follow("user-1", "user-2");

        Assert.Equal("user-1", follow.FollowerId);
        Assert.Equal("user-2", follow.FollowedId);
    }

    [Fact]
    public void Constructor_FollowSelf_Throws()
    {
        Assert.Throws<DomainException>(() => new Follow("user-1", "user-1"));
    }

    [Fact]
    public void Constructor_EmptyFollowerId_Throws()
    {
        Assert.Throws<DomainException>(() => new Follow("", "user-1"));
    }

    [Fact]
    public void Constructor_EmptyFollowedId_Throws()
    {
        Assert.Throws<DomainException>(() => new Follow("user-1", ""));
    }
}