using SocialProfiles.Core.Domain.Blocks;
using SocialProfiles.Core.Domain.Exceptions;
using SocialProfiles.Core.Domain.Presets;
using SocialProfiles.Core.Domain.Profiles;
using SocialProfiles.Core.Domain.Media;

namespace SocialProfiles.Tests.Domain;

public class ProfileTests
{
    private static Profile CreateProfile() =>
        new Profile("user-1", "anastasia", "Анастасія", new DarkPreset().CreateTheme());

    [Fact]
    public void Constructor_CreatesDefaultBlocksInOrder()
    {
        var profile = CreateProfile();

        Assert.Equal(3, profile.Blocks.Count);
        Assert.IsType<AboutBlock>(profile.Blocks[0]);
        Assert.IsType<CommentsBlock>(profile.Blocks[1]);
        Assert.IsType<FollowersBlock>(profile.Blocks[2]);
    }

    [Theory]
    [InlineData("ab")]
    [InlineData("has space")]
    [InlineData("анастасія")]
    [InlineData("")]
    [InlineData("this_name_is_way_too_long")]
    public void Constructor_WithInvalidUsername_Throws(string username)
    {
        Assert.Throws<DomainException>(() =>

            new Profile("user-1", username, "Анастасія", new DarkPreset().CreateTheme()));
    }

    [Fact]
    public void MoveBlockDown_SwapsWithNextBlock()
    {
        var profile = CreateProfile();
        var first = profile.Blocks[0];

        profile.MoveBlockDown(first.Id);

        Assert.Equal(first.Id, profile.Blocks[1].Id);
    }

    [Fact]
    public void MoveBlockUp_FirstBlock_DoesNothing()
    {
        var profile = CreateProfile();
        var first = profile.Blocks[0];

        profile.MoveBlockUp(first.Id);

        Assert.Equal(first.Id, profile.Blocks[0].Id);
    }

    [Fact]
    public void MoveBlockDown_LastBlock_DoesNothing()
    {
        var profile = CreateProfile();
        var last = profile.Blocks[profile.Blocks.Count - 1];
        var before = profile.Blocks.Select(b => b.Id).ToList();

        profile.MoveBlockDown(last.Id);

        var after = profile.Blocks.Select(b => b.Id).ToList();
        Assert.Equal(before, after);
    }

    [Fact]
    public void MoveBlock_UnknownId_Throws()
    {
        var profile = CreateProfile();

        Assert.Throws<DomainException>(() => profile.MoveBlockUp(Guid.NewGuid()));
    }

    [Fact]
    public void HideCommentsBlock_Throws()
    {
        var profile = CreateProfile();
        var comments = profile.Blocks.OfType<CommentsBlock>().Single();

        Assert.Throws<DomainException>(() => comments.SetVisibility(false));
    }

    [Fact]
    public void HideAboutBlock_Works()
    {
        var profile = CreateProfile();
        var about = profile.Blocks.OfType<AboutBlock>().Single();

        about.SetVisibility(false);

        Assert.False(about.IsVisible);
    }

    [Fact]
    public void UpdateInfo_TooLongAbout_Throws()
    {
        var profile = CreateProfile();

        Assert.Throws<DomainException>(() =>
            profile.UpdateInfo("Анастасія", new string('a', 1001)));
    }

    [Fact]
    public void UpdateInfo_EmptyDisplayName_Throws()
    {
        var profile = CreateProfile();

        Assert.Throws<DomainException>(() =>
            profile.UpdateInfo("", "Люблю музику"));
    }

    [Fact]
    public void UpdateInfo_ValidData_ChangesProfile()
    {
        // Arrange
        var profile = CreateProfile();

        // Act
        profile.UpdateInfo("  Анастасія Ткачова  ", "Люблю музику");

        Assert.Equal("Анастасія Ткачова", profile.DisplayName);
        Assert.Equal("Люблю музику", profile.About);
    }

    [Fact]
    public void ChangeTheme_Null_Throws()

    {
        var profile = CreateProfile();

        Assert.Throws<DomainException>(() => profile.ChangeTheme(null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData("http://evil.example/x.png")]
    [InlineData("/uploads/../secret.png")]
    public void SetImage_InvalidPath_Throws(string path)
    {
        var profile = CreateProfile();

        Assert.Throws<DomainException>(() => profile.SetImage(ImageKind.Avatar, path));
    }

    [Fact]
    public void SetImage_ThenRemove_ChangesPath()
    {
        var profile = CreateProfile();
        const string path = "/uploads/0123456789abcdef0123456789abcdef.png";

        profile.SetImage(ImageKind.Banner, path);
        Assert.Equal(path, profile.BannerPath);

        profile.RemoveImage(ImageKind.Banner);
        Assert.Null(profile.BannerPath);
    }
}