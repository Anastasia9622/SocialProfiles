using System.Text;

namespace SocialProfiles.Core.Domain.Media;

public enum ImageFormat
{
    Png,
    Jpeg,
    Gif,
    WebP
}

public static class ImageSignature
{
    private static readonly byte[] Png = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
    private static readonly byte[] Jpeg = { 0xFF, 0xD8, 0xFF };
    private static readonly byte[] Gif87 = Encoding.ASCII.GetBytes("GIF87a");
    private static readonly byte[] Gif89 = Encoding.ASCII.GetBytes("GIF89a");
    private static readonly byte[] Riff = Encoding.ASCII.GetBytes("RIFF");
    private static readonly byte[] WebPTag = Encoding.ASCII.GetBytes("WEBP");

    // Повертає формат або null, якщо файл не є підтримуваним зображенням.
    public static ImageFormat? Detect(ReadOnlySpan<byte> data)
    {
        if (StartsWith(data, Png)) return ImageFormat.Png;
        if (StartsWith(data, Jpeg)) return ImageFormat.Jpeg;
        if (StartsWith(data, Gif87) || StartsWith(data, Gif89)) return ImageFormat.Gif;

        // WebP: "RIFF" + 4 байти довжини + "WEBP"
        if (data.Length >= 12 && StartsWith(data, Riff) &&
            data.Slice(8, 4).SequenceEqual(new ReadOnlySpan<byte>(WebPTag)))
            return ImageFormat.WebP;


        return null;
    }

    private static bool StartsWith(ReadOnlySpan<byte> data, byte[] prefix) =>
        data.Length >= prefix.Length &&
        data.Slice(0, prefix.Length).SequenceEqual(new ReadOnlySpan<byte>(prefix));
}