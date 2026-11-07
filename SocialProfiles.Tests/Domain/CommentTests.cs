using SocialProfiles.Core.Domain.Exceptions;
using SocialProfiles.Core.Domain.Social;

namespace SocialProfiles.Tests.Domain;

public class CommentTests
{
    private static readonly Guid ProfileId = Guid.NewGuid();

    private static Comment CreateComment() =>
        new Comment(ProfileId, "author-1", "Крута сторінка!");

    [Fact]
    public void Constructor_ValidData_CreatesTopLevelComment()
    {
        var comment = CreateComment();

        Assert.False(comment.IsReply);
        Assert.Null(comment.ParentCommentId);
        Assert.Equal("Крута сторінка!", comment.Text);
    }

    [Fact]
    public void Constructor_TrimsText()
    {
        var comment = new Comment(ProfileId, "author-1", "  Привіт  ");

        Assert.Equal("Привіт", comment.Text);
    }

    [Theory]
    [InlineData(""), InlineData("   ")]
    public void Constructor_EmptyText_Throws(string text)
    {
        Assert.Throws<DomainException>(() => new Comment(ProfileId, "author-1", text));
    }

    [Fact]
    public void Constructor_TextAtMaxLength_IsAccepted()
    {
        var comment = new Comment(ProfileId, "author-1", new string('a', Comment.MaxLength));

        Assert.Equal(Comment.MaxLength, comment.Text.Length);
    }

    [Fact]
    public void Constructor_TextTooLong_Throws()
    {
        Assert.Throws<DomainException>(() =>
            new Comment(ProfileId, "author-1", new string('a', Comment.MaxLength + 1)));
    }

    [Fact]
    public void CreateReply_ReturnsReplyOnSameWall()
    {
        var parent = CreateComment();

        var reply = parent.CreateReply("author-2", "Дякую!");

        Assert.True(reply.IsReply);
        Assert.Equal(parent.Id, reply.ParentCommentId);
        Assert.Equal(parent.ProfileId, reply.ProfileId);
        Assert.Equal("author-2", reply.AuthorId);

    }

    [Fact]
    public void CreateReply_OnReply_Throws()
    {
        var reply = CreateComment().CreateReply("author-2", "Дякую!");

        Assert.Throws<DomainException>(() => reply.CreateReply("author-3", "І я!"));
    }

    [Theory]
    [InlineData("author-1", "owner-1", true)]    // автор
    [InlineData("owner-1", "owner-1", true)]     // власник стіни
    [InlineData("stranger", "owner-1", false)]   // сторонній
    [InlineData("", "owner-1", false)]           // порожній користувач
    public void CanBeDeletedBy_ChecksAuthorOrWallOwner(
        string userId, string wallOwnerId, bool expected)
    {
        var comment = CreateComment();

        Assert.Equal(expected, comment.CanBeDeletedBy(userId, wallOwnerId));
    }
}