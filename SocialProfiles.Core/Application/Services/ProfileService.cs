using SocialProfiles.Core.Application.Interfaces;
using SocialProfiles.Core.Domain.Exceptions;
using SocialProfiles.Core.Domain.Presets;
using SocialProfiles.Core.Domain.Profiles;

namespace SocialProfiles.Core.Application.Services;

public class ProfileService
{
    private readonly IProfileRepository _profiles;

    public ProfileService(IProfileRepository profiles)
    {
        _profiles = profiles;
    }

    public async Task<Profile> CreateAsync(string userId, string username, string displayName)
    {
        if (await _profiles.GetByUserIdAsync(userId) is not null)
            throw new DomainException("Профіль для цього користувача вже існує.");

        if (await _profiles.UsernameExistsAsync(username))
            throw new DomainException("Це ім'я користувача вже зайняте.");

        var profile = new Profile(userId, username, displayName, new DarkPreset().CreateTheme());
        await _profiles.AddAsync(profile);
        return profile;
    }

    public async Task<Profile> GetByUsernameAsync(string username) =>
        await _profiles.GetByUsernameAsync(username)
        ?? throw new DomainException("Профіль не знайдено.");


    public async Task UpdateInfoAsync(string userId, string displayName, string about)
    {
        var profile = await GetByUserIdAsync(userId);
        profile.UpdateInfo(displayName, about);
        await _profiles.SaveAsync(profile);
    }

    public async Task ChangeThemeAsync(string userId, Theme theme)
    {
        var profile = await GetByUserIdAsync(userId);
        profile.ChangeTheme(theme);
        await _profiles.SaveAsync(profile);
    }

    public async Task ApplyPresetAsync(string userId, ThemePreset preset) =>
        await ChangeThemeAsync(userId, preset.CreateTheme());

    public async Task MoveBlockAsync(string userId, Guid blockId, bool up)
    {
        var profile = await GetByUserIdAsync(userId);

        if (up)
            profile.MoveBlockUp(blockId);
        else
            profile.MoveBlockDown(blockId);

        await _profiles.SaveAsync(profile);
    }

    private async Task<Profile> GetByUserIdAsync(string userId) =>
        await _profiles.GetByUserIdAsync(userId)

        ?? throw new DomainException("Профіль не знайдено.");
}