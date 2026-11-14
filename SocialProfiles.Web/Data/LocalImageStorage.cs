using SocialProfiles.Core.Application.Interfaces;
using SocialProfiles.Core.Domain.Media;
using SocialProfiles.Web.Components;

namespace SocialProfiles.Web.Data;

public class LocalImageStorage : IImageStorage
{
    private readonly string _folder;

    public LocalImageStorage(string folder)
    {
        _folder = folder;
        Directory.CreateDirectory(_folder);
    }

    public async Task<string> SaveAsync(byte[] content, string extension)
    {
        var name = Guid.NewGuid().ToString("N") + extension;
        await File.WriteAllBytesAsync(Path.Combine(_folder, name), content);
        return "/uploads/" + name;
    }

    public Task DeleteAsync(string publicPath)
    {
        if (!ImageUpload.IsValidPublicPath(publicPath))
            return Task.CompletedTask;

        var file = Path.Combine(_folder, Path.GetFileName(publicPath));
        if (File.Exists(file))
            File.Delete(file);

        return Task.CompletedTask;
    }
}