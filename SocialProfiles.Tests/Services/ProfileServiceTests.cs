using SocialProfiles.Core.Application.Services;
using SocialProfiles.Core.Domain.Exceptions;
using SocialProfiles.Core.Domain.Presets;
using SocialProfiles.Tests.Fakes;

namespace SocialProfiles.Tests.Services;

public class ProfileServiceTests
{
    private readonly InMemoryProfileRepository _repo = new();
    private readonly ProfileService _service;

    public ProfileServiceTests()
    {
        _service = new ProfileService(_repo);
    }

    [Fact]
    public async Task CreateAsync_NewUser_CreatesProfileWithDarkTheme()
    {
        var profile = await _service.CreateAsync("user-1", "anastasia", "Анастасія");

        Assert.Equal("anastasia", profile.Username);
        Assert.Equal(new DarkPreset().CreateTheme().BackgroundColor, profile.Theme.BackgroundColor);
        Assert.NotNull(await _repo.GetByUserIdAsync("user-1"));
    }

    [Fact]
    public async Task CreateAsync_DuplicateUsername_Throws()
    {
        await _service.CreateAsync("user-1", "anastasia", "Анастасія");


        await Assert.ThrowsAsync<DomainException>(() =>
            _service.CreateAsync("user-2", "Anastasia", "Інша"));
    }

    [Fact]
    public async Task CreateAsync_UserAlreadyHasProfile_Throws()
    {
        await _service.CreateAsync("user-1", "anastasia", "Анастасія");

        await Assert.ThrowsAsync<DomainException>(() =>
            _service.CreateAsync("user-1", "second", "Анастасія"));
    }

    [Fact]
    public async Task GetByUsernameAsync_Unknown_Throws()
    {
        await Assert.ThrowsAsync<DomainException>(() => _service.GetByUsernameAsync("nobody"));
    }

    [Fact]
    public async Task ApplyPresetAsync_ChangesThemeAndSaves()
    {
        await _service.CreateAsync("user-1", "anastasia", "Анастасія");

        await _service.ApplyPresetAsync("user-1", new LightPreset());

        var profile = await _service.GetByUsernameAsync("anastasia");
        Assert.Equal(new LightPreset().CreateTheme().BackgroundColor, profile.Theme.BackgroundColor);
        Assert.Equal(1, _repo.SaveCount);
    }

    [Fact]

    public async Task UpdateInfoAsync_UnknownUser_Throws()
    {
        await Assert.ThrowsAsync<DomainException>(() =>
            _service.UpdateInfoAsync("ghost", "Ім'я", "Про мене"));
    }
}