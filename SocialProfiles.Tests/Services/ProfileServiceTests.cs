using SocialProfiles.Core.Application.Services;
using SocialProfiles.Core.Domain.Blocks;
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

    [Fact]
    public async Task IsUsernameAvailableAsync_FreeName_ReturnsTrue()
    {
        // Сховище порожнє, тому будь-яке ім'я вільне.
        bool available = await _service.IsUsernameAvailableAsync("anastasia");

        Assert.True(available);
    }

    [Fact]
    public async Task IsUsernameAvailableAsync_TakenName_ReturnsFalse()
    {
        await _service.CreateAsync("user-1", "anastasia", "Анастасія");

        bool available = await _service.IsUsernameAvailableAsync("anastasia");

        Assert.False(available);
    }

    [Fact]
    public async Task IsUsernameAvailableAsync_TakenNameInDifferentCase_ReturnsFalse()
    {
        await _service.CreateAsync("user-1", "anastasia", "Анастасія");

        bool available = await _service.IsUsernameAvailableAsync("ANASTASIA");

        Assert.False(available);
    }

    [Fact]
    public async Task GetByUserIdAsync_ExistingUser_ReturnsProfile()
    {

        await _service.CreateAsync("user-1", "anastasia", "Анастасія");

        var profile = await _service.GetByUserIdAsync("user-1");

        Assert.Equal("anastasia", profile.Username);
        Assert.Equal("user-1", profile.UserId);
    }

    [Fact]
    public async Task GetByUserIdAsync_UnknownUser_Throws()
    {
        await Assert.ThrowsAsync<DomainException>(() => _service.GetByUserIdAsync("ghost"));
    }

    [Fact]
    public async Task MoveBlockAsync_Down_ChangesOrderAndSaves()
    {
        var profile = await _service.CreateAsync("user-1", "anastasia", "Анастасія");
        var firstId = profile.Blocks[0].Id;

        await _service.MoveBlockAsync("user-1", firstId, up: false);

        Assert.Equal(firstId, profile.Blocks[1].Id);
        Assert.Equal(1, _repo.SaveCount);
    }

    [Fact]
    public async Task MoveBlockAsync_Up_ChangesOrderAndSaves()
    {
        var profile = await _service.CreateAsync("user-1", "anastasia", "Анастасія");
        var lastId = profile.Blocks[2].Id;

        await _service.MoveBlockAsync("user-1", lastId, up: true);

        Assert.IsType<CommentsBlock>(profile.Blocks[2]);
        Assert.Equal(lastId, profile.Blocks[1].Id);
        Assert.Equal(1, _repo.SaveCount);
    }

    [Fact]
    public async Task GetProfilesByUserIdsAsync_ReturnsKnownProfilesOnly()
    {
        await _service.CreateAsync("user-1", "first", "Перша");
        await _service.CreateAsync("user-2", "second", "Друга");

        var result = await _service.GetProfilesByUserIdsAsync(new[] { "user-1", "user-2", "user-1", "ghost" });

        Assert.Equal(2, result.Count);
        Assert.Equal("first", result["user-1"].Username);
        Assert.False(result.ContainsKey("ghost"));
    }
}