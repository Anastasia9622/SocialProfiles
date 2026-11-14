using System.Text.RegularExpressions;
using SocialProfiles.Core.Domain.Exceptions;

namespace SocialProfiles.Core.Domain.Media;

public class ImageUpload
{
    // Єдиний допустимий вигляд публічного шляху: /uploads/<32 hex>.<розширення>
    private static readonly Regex PublicPath = new(@"^/uploads/[0-9a-f]{32}\.(png|jpg|gif|webp)\z");

    public ImageKind Kind { get; }
    public ImageFormat Format { get; }
    public long SizeBytes { get; }

    public string Extension => Format switch
    {
        ImageFormat.Png => ".png",
        ImageFormat.Jpeg => ".jpg",
        ImageFormat.Gif => ".gif",
        ImageFormat.WebP => ".webp",
        _ => throw new DomainException("Невідомий формат зображення.")
    };

    public static long MaxBytes(ImageKind kind) => kind switch
    {
        ImageKind.Avatar => 2L * 1024 * 1024,
        ImageKind.Banner => 5L * 1024 * 1024,
        ImageKind.Background => 8L * 1024 * 1024,
        _ => throw new DomainException("Невідомий тип зображення.")
    };

    public static bool IsValidPublicPath(string? path) =>

        path is not null && PublicPath.IsMatch(path);

    public ImageUpload(ImageKind kind, byte[] data)
    {
        if (data is null || data.Length == 0)
            throw new DomainException("Файл порожній.");

        if (data.Length > MaxBytes(kind))
            throw new DomainException(
                $"Файл завеликий. Максимум для цього зображення: {MaxBytes(kind) / (1024 * 1024)} МБ.");

        Format = ImageSignature.Detect(data)
                 ?? throw new DomainException("Підтримуються лише PNG, JPG, GIF та WebP.");

        Kind = kind;
        SizeBytes = data.Length;
    }
}