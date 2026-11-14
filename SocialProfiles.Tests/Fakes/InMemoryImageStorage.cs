using SocialProfiles.Core.Application.Interfaces;

namespace SocialProfiles.Tests.Fakes;

public class InMemoryImageStorage : IImageStorage
{
    private readonly Dictionary<string, byte[]> _files = new();

    public int Count => _files.Count;
    public bool Exists(string path) => _files.ContainsKey(path);

    public Task<string> SaveAsync(byte[] content, string extension)
    {
        var path = "/uploads/" + Guid.NewGuid().ToString("N") + extension;
        _files[path] = content;
        return Task.FromResult(path);
    }

    public Task DeleteAsync(string publicPath)
    {
        _files.Remove(publicPath);
        return Task.CompletedTask;
    }
}