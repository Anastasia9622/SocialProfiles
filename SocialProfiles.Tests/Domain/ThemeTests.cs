using SocialProfiles.Core.Domain.Exceptions;
using SocialProfiles.Core.Domain.Profiles;

namespace SocialProfiles.Tests.Domain;

public class ThemeTests
{
    [Fact]
    public void Constructor_WithValidColors_CreatesTheme()
    {
        // Arrange + Act
        var theme = new Theme("#1b1033", "#2a1a4d", "#f3eaff", "#ff5fa2");

        // Assert
        Assert.Equal("#1b1033", theme.BackgroundColor);
        Assert.True(theme.Validate());
    }

    [Theory]
    [InlineData("red")]
    [InlineData("#12345")]
    [InlineData("#GGGGGG")]
    [InlineData("")]
    public void Constructor_WithInvalidColor_ThrowsDomainException(string badColor)
    {
        Assert.Throws<DomainException>(() =>
            new Theme(badColor, "#2a1a4d", "#f3eaff", "#ff5fa2"));
    }

    [Fact]
    public void ToCssVariables_ContainsAllColors()
    {

        var theme = new Theme("#1b1033", "#2a1a4d", "#f3eaff", "#ff5fa2");

        var css = theme.ToCssVariables();

        Assert.Contains("--bg-color:#1b1033", css);
        Assert.Contains("--accent-color:#ff5fa2", css);
    }

    [Fact]
    public void ContrastRatio_BlackOnWhite_Is21()
    {
        var theme = new Theme("#ffffff", "#2a1a4d", "#000000", "#ff5fa2");

        double ratio = theme.ContrastRatio();

        Assert.Equal(21.0, ratio, 2);
    }

    [Fact]
    public void ContrastRatio_DoesNotDependOnOrder()
    {
        var blackOnWhite = new Theme("#ffffff", "#2a1a4d", "#000000", "#ff5fa2");
        var whiteOnBlack = new Theme("#000000", "#2a1a4d", "#ffffff", "#ff5fa2");

        Assert.Equal(blackOnWhite.ContrastRatio(), whiteOnBlack.ContrastRatio(), 2);
    }

    [Fact]
    public void ContrastRatio_SameColors_Is1()
    {
        var theme = new Theme("#336699", "#2a1a4d", "#336699", "#ff5fa2");

        Assert.Equal(1.0, theme.ContrastRatio(), 2);
    }

    [Theory]
    [InlineData("#ff0000", "#ffffff", 4.0)]    // чистий червоний на білому
    [InlineData("#0000ff", "#ffffff", 8.59)]   // чистий синій на білому
    [InlineData("#767676", "#ffffff", 4.54)]   // сірий, ще проходить поріг 4.5
    [InlineData("#777777", "#ffffff", 4.48)]   // сірий, трохи нижче порога
    [InlineData("#f3eaff", "#1b1033", 15.4)]   // текст і фон твоєї темної теми
    public void ContrastRatio_KnownColors_MatchesExpected(
        string textColor, string backgroundColor, double expected)
    {
        var theme = new Theme(backgroundColor, "#2a1a4d", textColor, "#ff5fa2");

        Assert.Equal(expected, theme.ContrastRatio(), 1);
    }
}