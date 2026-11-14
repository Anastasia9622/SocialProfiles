using System.Text;
using SocialProfiles.Core.Domain.Exceptions;
using SocialProfiles.Core.Domain.Media;
using SocialProfiles.Tests.Fakes;

namespace SocialProfiles.Tests.Domain;

public class ImageUploadTests
{
    public static IEnumerable<object[]> ValidImages => new[]
    {
        new object[] { TestImages.Png(), ImageFormat.Png, ".png" },
        new object[] { TestImages.Jpeg(), ImageFormat.Jpeg, ".jpg" },
        new object[] { TestImages.Gif(), ImageFormat.Gif, ".gif" },
        new object[] { TestImages.WebP(), ImageFormat.WebP, ".webp" }
    };

    [Theory]
    [MemberData(nameof(ValidImages))]
    public void Constructor_ValidImage_DetectsFormat(byte[] data, ImageFormat format, string extension)
    {
        var upload = new ImageUpload(ImageKind.Avatar, data);

        Assert.Equal(format, upload.Format);
        Assert.Equal(extension, upload.Extension);
    }

    [Theory]
    [InlineData("<script>alert(1)</script>")]
    [InlineData("<svg xmlns=\"http://www.w3.org/2000/svg\"></svg>")]
    [InlineData("просто текст у файлі з розширенням png")]
    public void Constructor_NotAnImage_Throws(string content)

    {
        var data = Encoding.UTF8.GetBytes(content);

        Assert.Throws<DomainException>(() => new ImageUpload(ImageKind.Avatar, data));
    }

    [Fact]
    public void Constructor_EmptyFile_Throws()
    {
        Assert.Throws<DomainException>(() => new ImageUpload(ImageKind.Avatar, Array.Empty<byte>()));
    }

    [Fact]
    public void Constructor_AvatarAtLimit_IsAccepted()
    {
        var data = TestImages.Png((int)ImageUpload.MaxBytes(ImageKind.Avatar));

        var upload = new ImageUpload(ImageKind.Avatar, data);

        Assert.Equal(data.Length, upload.SizeBytes);
    }

    [Fact]
    public void Constructor_AvatarOverLimit_Throws()
    {
        var data = TestImages.Png((int)ImageUpload.MaxBytes(ImageKind.Avatar) + 1);

        Assert.Throws<DomainException>(() => new ImageUpload(ImageKind.Avatar, data));
    }

    [Fact]
    public void Constructor_SameFileIsAllowedForBannerButNotAvatar()

    {
        var data = TestImages.Gif(3 * 1024 * 1024);

        Assert.Throws<DomainException>(() => new ImageUpload(ImageKind.Avatar, data));
        Assert.Equal(ImageFormat.Gif, new ImageUpload(ImageKind.Banner, data).Format);
    }

    [Theory]
    [InlineData("/uploads/0123456789abcdef0123456789abcdef.png", true)]
    [InlineData("/uploads/0123456789abcdef0123456789abcdef.gif", true)]
    [InlineData("/uploads/../secret.png", false)]
    [InlineData("/uploads/abc.png", false)]
    [InlineData("/uploads/0123456789abcdef0123456789abcdef.svg", false)]
    [InlineData("/uploads/0123456789ABCDEF0123456789ABCDEF.png", false)]
    [InlineData("http://evil.example/0123456789abcdef0123456789abcdef.png", false)]
    [InlineData("", false)]
    public void IsValidPublicPath_AcceptsOnlyServerGeneratedPaths(string path, bool expected)
    {
        Assert.Equal(expected, ImageUpload.IsValidPublicPath(path));
    }
}