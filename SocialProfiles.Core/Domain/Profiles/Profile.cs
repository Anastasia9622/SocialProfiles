using System.Text.RegularExpressions;
using SocialProfiles.Core.Domain.Blocks;
using SocialProfiles.Core.Domain.Exceptions;
using SocialProfiles.Core.Domain.Media;

namespace SocialProfiles.Core.Domain.Profiles;

public class Profile
{
    private static readonly Regex UsernamePattern = new(@"^[a-zA-Z0-9_]{3,20}$");
    private readonly List<ProfileBlock> _blocks = new();

    public Guid Id { get; private set; } = Guid.NewGuid();
    public string UserId { get; private set; }
    public string Username { get; private set; }
    public string DisplayName { get; private set; }
    public string About { get; private set; } = "";
    public string? AvatarPath { get; private set; }
    public string? BannerPath { get; private set; }
    public Theme Theme { get; private set; }

    public IReadOnlyList<ProfileBlock> Blocks =>
        _blocks.OrderBy(b => b.Position).ToList();

    public Profile(string userId, string username, string displayName, Theme theme)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new DomainException("Не вказано користувача.");
        if (!UsernamePattern.IsMatch(username ?? ""))
            throw new DomainException("Ім'я користувача: 3-20 символів, лише латиниця, цифри та _.");

        UserId = userId;
        Username = username!;

        DisplayName = ValidateDisplayName(displayName);
        Theme = theme ?? throw new DomainException("Тема не може бути порожньою.");

        AddBlock(new AboutBlock());
        AddBlock(new CommentsBlock());
        AddBlock(new FollowersBlock());
    }

    public void UpdateInfo(string displayName, string about)
    {
        if (about is null || about.Length > 1000)
            throw new DomainException("Текст «Про мене» має бути не довшим за 1000 символів.");

        DisplayName = ValidateDisplayName(displayName);
        About = about;
    }

    public void ChangeTheme(Theme theme) =>
        Theme = theme ?? throw new DomainException("Тема не може бути порожньою.");

    public string? BackgroundImagePath { get; private set; }

    public string? GetImagePath(ImageKind kind) => kind switch
    {
        ImageKind.Avatar => AvatarPath,
        ImageKind.Banner => BannerPath,
        ImageKind.Background => BackgroundImagePath,
        _ => throw new DomainException("Невідомий тип зображення.")
    };

    public void SetImage(ImageKind kind, string? path)
    {
        // Шлях потрапляє в CSS і в src, тому приймаємо лише шляхи, створені нашим сховищем.
        if (path is not null && !ImageUpload.IsValidPublicPath(path))
            throw new DomainException("Некоректний шлях до зображення.");

        switch (kind)
        {
            case ImageKind.Avatar: AvatarPath = path; break;
            case ImageKind.Banner: BannerPath = path; break;
            case ImageKind.Background: BackgroundImagePath = path; break;
            default: throw new DomainException("Невідомий тип зображення.");
        }
    }

    public void RemoveImage(ImageKind kind) => SetImage(kind, null);

    public void AddBlock(ProfileBlock block)
    {
        block.Position = _blocks.Count;
        _blocks.Add(block);
    }

    public void MoveBlockUp(Guid blockId) => MoveBlock(blockId, -1);

    public void MoveBlockDown(Guid blockId) => MoveBlock(blockId, +1);

    private void MoveBlock(Guid blockId, int offset)
    {
        var ordered = _blocks.OrderBy(b => b.Position).ToList();

        int index = ordered.FindIndex(b => b.Id == blockId);
        if (index < 0)
            throw new DomainException("Блок не знайдено.");

        int target = index + offset;
        if (target < 0 || target >= ordered.Count)
            return;   // блок уже на краю списку, змінювати нічого

        (ordered[index].Position, ordered[target].Position) =
            (ordered[target].Position, ordered[index].Position);
    }

    private static string ValidateDisplayName(string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName) || displayName.Trim().Length > 50)
            throw new DomainException("Назва профілю: від 1 до 50 символів.");

        return displayName.Trim();
    }

    // Потрібен лише для EF Core, коли він відновлює профіль із бази.
    private Profile()
    {
        UserId = null!;
        Username = null!;
        DisplayName = null!;
        Theme = null!;
    }
}