using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SocialProfiles.Core.Domain.Presets;
using SocialProfiles.Core.Domain.Profiles;

namespace SocialProfiles.Tests.Domain;

public class PresetTests
{
    [Fact]
    public void DarkPreset_CreatesExpectedTheme()
    {
        var preset = new DarkPreset();

        var theme = preset.CreateTheme();

        Assert.Equal("#1b1033", theme.BackgroundColor);
        Assert.Equal("#f3eaff", theme.TextColor);
    }

    [Fact]
    public void LightPreset_CreatesExpectedTheme()
    {
        var preset = new LightPreset();

        var theme = preset.CreateTheme();

        Assert.Equal("#ffffff", theme.BackgroundColor);
        Assert.Equal("#1b1033", theme.TextColor);
    }

    [Fact]
    public void AllPresets_CreateValidThemes()
    {
        Assert.All(PresetCatalog.All, preset =>
            Assert.True(preset.CreateTheme().Validate(),
                $"Пресет {preset.Name} має некоректну тему"));
    }

    [Fact]
    public void AllPresets_HaveReadableContrast()
    {
        Assert.All(PresetCatalog.All, preset =>
            Assert.True(preset.CreateTheme().ContrastRatio() >= 4.5,
                $"Пресет {preset.Name} має низький контраст"));
    }

    [Fact]
    public void AllPresets_HaveUniqueNonEmptyNames()
    {
        var names = PresetCatalog.All.Select(p => p.Name).ToList();

        Assert.All(names, name => Assert.False(string.IsNullOrWhiteSpace(name)));
        Assert.Equal(names.Count, names.Distinct().Count());
    }

    [Fact]
    public void CreateTheme_ReturnsNewObjectEachTime()
    {
        ThemePreset preset = new DarkPreset();   // змінна базового типу, об'єкт нащадка

        var first = preset.CreateTheme();
        var second = preset.CreateTheme();

        Assert.NotSame(first, second);
    }

    [Fact]
    public void Polymorphism_DifferentPresetsGiveDifferentThemes()
    {
        ThemePreset dark = new DarkPreset();
        ThemePreset light = new LightPreset();

        Assert.NotEqual(dark.CreateTheme().BackgroundColor, light.CreateTheme().BackgroundColor);
    }
}
