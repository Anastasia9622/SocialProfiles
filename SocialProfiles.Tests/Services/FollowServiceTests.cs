using SocialProfiles.Core.Application.Services;
using SocialProfiles.Core.Domain.Exceptions;
using SocialProfiles.Tests.Fakes;

namespace SocialProfiles.Tests.Services;

public class FollowServiceTests
{
    private readonly InMemoryProfileRepository _profiles = new();
    private readonly InMemoryFollowRepository _follows = new();
    private readonly FollowService _service;

    public FollowServiceTests()
    {
        _service = new FollowService(_follows, _profiles);

        var profileService = new ProfileService(_profiles);
        profileService.CreateAsync("user-1", "first", "Перша").Wait();
        profileService.CreateAsync("user-2", "second", "Друга").Wait();
    }

    [Fact]
    public async Task FollowAsync_ValidUsers_CreatesFollow()
    {
        await _service.FollowAsync("user-1", "user-2");

        Assert.True(await _service.IsFollowingAsync("user-1", "user-2"));
    }

    [Fact]
    public async Task FollowAsync_Twice_Throws()
    {

        await _service.FollowAsync("user-1", "user-2");

        await Assert.ThrowsAsync<DomainException>(() =>
            _service.FollowAsync("user-1", "user-2"));
    }

    [Fact]
    public async Task FollowAsync_Self_Throws()
    {
        await Assert.ThrowsAsync<DomainException>(() =>
            _service.FollowAsync("user-1", "user-1"));
    }

    [Fact]
    public async Task FollowAsync_UnknownTarget_Throws()
    {
        await Assert.ThrowsAsync<DomainException>(() =>
            _service.FollowAsync("user-1", "ghost"));
    }

    [Fact]
    public async Task UnfollowAsync_ExistingFollow_RemovesIt()
    {
        await _service.FollowAsync("user-1", "user-2");
        Assert.True(await _service.IsFollowingAsync("user-1", "user-2"));

        await _service.UnfollowAsync("user-1", "user-2");

        Assert.False(await _service.IsFollowingAsync("user-1", "user-2"));
    }

    [Fact]
    public async Task UnfollowAsync_WhenNotFollowing_Throws()
    {
        await Assert.ThrowsAsync<DomainException>(() =>
            _service.UnfollowAsync("user-1", "user-2"));
    }

    [Fact]
    public async Task CountFollowersAsync_CountsOnlyFollowersOfThatUser()
    {
        await _service.FollowAsync("user-1", "user-2");

        int followersOfUser2 = await _service.CountFollowersAsync("user-2");
        int followersOfUser1 = await _service.CountFollowersAsync("user-1");

        Assert.Equal(1, followersOfUser2);
        Assert.Equal(0, followersOfUser1);
    }

    [Fact]
    public async Task CountFollowingAsync_CountsOnlyOutgoingFollows()
    {
        await _service.FollowAsync("user-1", "user-2");

        int followingOfUser1 = await _service.CountFollowingAsync("user-1");
        int followingOfUser2 = await _service.CountFollowingAsync("user-2");

        Assert.Equal(1, followingOfUser1);
        Assert.Equal(0, followingOfUser2);
    }
}