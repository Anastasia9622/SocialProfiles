using SocialProfiles.Core.Application.Interfaces;
using SocialProfiles.Core.Domain.Exceptions;
using SocialProfiles.Core.Domain.Media;

namespace SocialProfiles.Core.Application.Services;

public class ProfileImageService
{
    private readonly IProfileRepository _profiles;
    private readonly IImageStorage _storage;

    public ProfileImageService(IProfileRepository profiles, IImageStorage storage)
    {
        _profiles = profiles;
        _storage = storage;
    }

    public async Task<string> UploadAsync(string userId, ImageKind kind, Stream content)
    {
        var profile = await _profiles.GetByUserIdAsync(userId)
                      ?? throw new DomainException("Профіль не знайдено.");

        var data = await ReadLimitedAsync(content, ImageUpload.MaxBytes(kind));
        var image = new ImageUpload(kind, data); 

        var oldPath = profile.GetImagePath(kind);
        var newPath = await _storage.SaveAsync(data, image.Extension);

        try
        {
            profile.SetImage(kind, newPath);
            await _profiles.SaveAsync(profile);
        }
        catch
        {
            await _storage.DeleteAsync(newPath);  
            throw;
        }

        if (oldPath is not null)
            await _storage.DeleteAsync(oldPath);

        return newPath;
    }

    public async Task RemoveAsync(string userId, ImageKind kind)
    {
        var profile = await _profiles.GetByUserIdAsync(userId)
                      ?? throw new DomainException("Профіль не знайдено.");

        var oldPath = profile.GetImagePath(kind);
        if (oldPath is null)
            return;

        profile.RemoveImage(kind);
        await _profiles.SaveAsync(profile);
        await _storage.DeleteAsync(oldPath);
    }

    private static async Task<byte[]> ReadLimitedAsync(Stream content, long maxBytes)
    {
        using var buffer = new MemoryStream();
        var chunk = new byte[81920];
        int read;

        while ((read = await content.ReadAsync(chunk)) > 0)
        {
            buffer.Write(chunk, 0, read);
            if (buffer.Length > maxBytes)
                break;
        }

        return buffer.ToArray();
    }
}