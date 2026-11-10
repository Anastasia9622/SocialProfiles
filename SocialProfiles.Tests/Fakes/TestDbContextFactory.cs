using Microsoft.EntityFrameworkCore;
using SocialProfiles.Web.Data;

namespace SocialProfiles.Tests.Fakes;

public class TestDbContextFactory : IDbContextFactory<ApplicationDbContext>
{
    private readonly DbContextOptions<ApplicationDbContext> _options;

    public TestDbContextFactory(DbContextOptions<ApplicationDbContext> options)
    {
        _options = options;
    }

    public ApplicationDbContext CreateDbContext() => new ApplicationDbContext(_options);
}