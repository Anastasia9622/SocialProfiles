using SocialProfiles.Core.Application.Services;
using SocialProfiles.Core.Domain.Exceptions;
using SocialProfiles.Core.Domain.Profiles;
using SocialProfiles.Tests.Fakes;

namespace SocialProfiles.Tests.Services;

public class CommentServiceTests
{
    private readonly InMemoryProfileRepository _profiles = new();
    private readonly InMemoryCommentRepository _comments = new();
    private readonly CommentService _service;
    private readonly Profile _wall;

    public CommentServiceTests()
    {
        _service = new CommentService(_comments, _profiles);

        var profileService = new ProfileService(_profiles);
        _wall = profileService.CreateAsync("owner-1", "anastasia", "Анастасія").Result;
    }

    [Fact]
    public async Task AddAsync_ValidData_StoresComment()
    {
        var comment = await _service.AddAsync(_wall.Id, "guest-1", "Крута сторінка!");

        var wall = await _service.GetWallAsync(_wall.Id);
        Assert.Single(wall);
        Assert.Equal(comment.Id, wall[0].Id);
    }


    [Fact]
    public async Task AddAsync_UnknownProfile_Throws()
    {
        await Assert.ThrowsAsync<DomainException>(() =>
            _service.AddAsync(Guid.NewGuid(), "guest-1", "Привіт"));
    }

    [Fact]
    public async Task ReplyAsync_CreatesReplyOnSameWall()
    {
        var parent = await _service.AddAsync(_wall.Id, "guest-1", "Привіт");

        var reply = await _service.ReplyAsync(parent.Id, "owner-1", "Дякую!");

        Assert.True(reply.IsReply);
        Assert.Equal(parent.Id, reply.ParentCommentId);
        Assert.Equal(2, _comments.Count);
    }

    [Fact]
    public async Task ReplyAsync_OnReply_Throws()
    {
        var parent = await _service.AddAsync(_wall.Id, "guest-1", "Привіт");
        var reply = await _service.ReplyAsync(parent.Id, "owner-1", "Дякую!");

        await Assert.ThrowsAsync<DomainException>(() =>
            _service.ReplyAsync(reply.Id, "guest-1", "Будь ласка"));
    }

    [Fact]
    public async Task DeleteAsync_ByAuthor_RemovesComment()
    {

        var comment = await _service.AddAsync(_wall.Id, "guest-1", "Привіт");

        await _service.DeleteAsync(comment.Id, "guest-1");

        Assert.Equal(0, _comments.Count);
    }

    [Fact]
    public async Task DeleteAsync_ByWallOwner_RemovesComment()
    {
        var comment = await _service.AddAsync(_wall.Id, "guest-1", "Привіт");

        await _service.DeleteAsync(comment.Id, "owner-1");

        Assert.Equal(0, _comments.Count);
    }

    [Fact]
    public async Task DeleteAsync_ByStranger_ThrowsAndKeepsComment()
    {
        var comment = await _service.AddAsync(_wall.Id, "guest-1", "Привіт");

        await Assert.ThrowsAsync<DomainException>(() =>
            _service.DeleteAsync(comment.Id, "stranger"));

        Assert.Equal(1, _comments.Count);
    }

    [Fact]
    public async Task DeleteAsync_ParentComment_AlsoRemovesReplies()
    {
        var parent = await _service.AddAsync(_wall.Id, "guest-1", "Привіт");

        await _service.ReplyAsync(parent.Id, "owner-1", "Дякую!");
        await _service.ReplyAsync(parent.Id, "guest-2", "І я!");

        await _service.DeleteAsync(parent.Id, "guest-1");

        Assert.Equal(0, _comments.Count);
    }
}