namespace SocialProfiles.Core.Application.Interfaces;

public interface IImageStorage
{
    // Зберігає файл і повертає публічний шлях, наприклад /uploads/ab12...ef.png
    Task<string> SaveAsync(byte[] content, string extension);

    Task DeleteAsync(string publicPath);
}