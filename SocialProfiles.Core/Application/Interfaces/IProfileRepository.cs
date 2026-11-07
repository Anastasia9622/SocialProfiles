using SocialProfiles.Core.Domain.Profiles;

namespace SocialProfiles.Core.Application.Interfaces;

public interface IProfileRepository
{
    Task<Profile?> GetByIdAsync(Guid id);
    Task<Profile?> GetByUserIdAsync(string userId);
    Task<Profile?> GetByUsernameAsync(string username);
    Task<bool> UsernameExistsAsync(string username);
    Task AddAsync(Profile profile);
    Task SaveAsync(Profile profile);
}