using System.Text;

namespace SocialProfiles.Tests.Fakes;

public static class TestImages
{
    public static byte[] Png(int size = 64) =>
        With(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, size);

    public static byte[] Jpeg(int size = 64) =>
        With(new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }, size);

    public static byte[] Gif(int size = 64) =>
        With(Encoding.ASCII.GetBytes("GIF89a"), size);

    public static byte[] WebP(int size = 64)
    {
        var data = new byte[Math.Max(size, 12)];
        Encoding.ASCII.GetBytes("RIFF").CopyTo(data, 0);
        Encoding.ASCII.GetBytes("WEBP").CopyTo(data, 8);
        return data;
    }

    private static byte[] With(byte[] header, int size)
    {
        var data = new byte[Math.Max(size, header.Length)];
        header.CopyTo(data, 0);
        return data;
    }
}