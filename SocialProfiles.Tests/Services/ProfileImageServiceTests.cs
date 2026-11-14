using SocialProfiles.Core.Application.Services;
using SocialProfiles.Core.Domain.Exceptions;
using SocialProfiles.Core.Domain.Media;
using SocialProfiles.Core.Domain.Profiles;
using SocialProfiles.Tests.Fakes;

namespace SocialProfiles.Tests.Services;

public class ProfileImageServiceTests
{
    private readonly InMemoryProfileRepository _repo = new();
    private readonly InMemoryImageStorage _storage = new();
    private readonly ProfileImageService _images;
    private readonly Profile _profile;

    public ProfileImageServiceTests()
    {
        _images = new ProfileImageService(_repo, _storage);
        _profile = new ProfileService(_repo).CreateAsync("user-1", "anastasia", "Анастасія").Result;
    }

    [Fact]
    public async Task UploadAsync_ValidAvatar_SetsPathAndStoresFile()
    {
        var path = await _images.UploadAsync("user-1", ImageKind.Avatar, new MemoryStream(TestImages.Png()));

        Assert.Equal(path, _profile.AvatarPath);
        Assert.True(_storage.Exists(path));
        Assert.Equal(1, _repo.SaveCount);
    }

    [Fact]

    public async Task UploadAsync_Gif_IsAcceptedForBackground()
    {
        var path = await _images.UploadAsync("user-1", ImageKind.Background, new MemoryStream(TestImages.Gif()));

        Assert.Equal(path, _profile.BackgroundImagePath);
        Assert.EndsWith(".gif", path);
    }

    [Fact]
    public async Task UploadAsync_Replacement_DeletesOldFile()
    {
        var first = await _images.UploadAsync("user-1", ImageKind.Avatar, new MemoryStream(TestImages.Png()));
        var second = await _images.UploadAsync("user-1", ImageKind.Avatar, new MemoryStream(TestImages.Jpeg()));

        Assert.NotEqual(first, second);
        Assert.False(_storage.Exists(first));
        Assert.True(_storage.Exists(second));
        Assert.Equal(1, _storage.Count);
    }

    [Fact]
    public async Task UploadAsync_NotAnImage_ThrowsAndChangesNothing()
    {
        var fake = System.Text.Encoding.UTF8.GetBytes("<script>alert(1)</script>");

        await Assert.ThrowsAsync<DomainException>(() =>
            _images.UploadAsync("user-1", ImageKind.Avatar, new MemoryStream(fake)));

        Assert.Null(_profile.AvatarPath);
        Assert.Equal(0, _storage.Count);
    }


    [Fact]
    public async Task UploadAsync_TooLarge_ThrowsAndChangesNothing()
    {
        var big = TestImages.Png((int)ImageUpload.MaxBytes(ImageKind.Avatar) + 1);

        await Assert.ThrowsAsync<DomainException>(() =>
            _images.UploadAsync("user-1", ImageKind.Avatar, new MemoryStream(big)));

        Assert.Null(_profile.AvatarPath);
        Assert.Equal(0, _storage.Count);
    }

    [Fact]
    public async Task UploadAsync_UnknownUser_Throws()
    {
        await Assert.ThrowsAsync<DomainException>(() =>
            _images.UploadAsync("ghost", ImageKind.Avatar, new MemoryStream(TestImages.Png())));
    }

    [Fact]
    public async Task RemoveAsync_ExistingImage_ClearsPathAndDeletesFile()
    {
        var path = await _images.UploadAsync("user-1", ImageKind.Banner, new MemoryStream(TestImages.Png()));

        await _images.RemoveAsync("user-1", ImageKind.Banner);

        Assert.Null(_profile.BannerPath);
        Assert.False(_storage.Exists(path));
    }
}