using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SocialProfiles.Core.Domain.Blocks;
using SocialProfiles.Core.Domain.Presets;
using SocialProfiles.Core.Domain.Profiles;
using SocialProfiles.Core.Domain.Social;
using SocialProfiles.Tests.Fakes;
using SocialProfiles.Web.Data;
using SocialProfiles.Web.Data.Repositories;

namespace SocialProfiles.Tests.Data;

public class EfRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly TestDbContextFactory _factory;

    public EfRepositoryTests()
    {
        // База в пам'яті живе, доки відкрите з'єднання.
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;
        _factory = new TestDbContextFactory(options);

        using var db = _factory.CreateDbContext();
        db.Database.EnsureCreated();   // створює таблиці за моделлю

        // Зовнішні ключі вимагають, щоб користувачі існували.
        db.Users.Add(new ApplicationUser { Id = "user-1", UserName = "user1" });

        db.Users.Add(new ApplicationUser { Id = "user-2", UserName = "user2" });
        db.SaveChanges();
    }

    public void Dispose() => _connection.Dispose();

    private static Profile NewProfile() =>
        new Profile("user-1", "anastasia", "Анастасія", new DarkPreset().CreateTheme());

    [Fact]
    public async Task ProfileRepository_AddAndLoad_RestoresThemeAndBlocks()
    {
        var repo = new EfProfileRepository(_factory);
        var profile = NewProfile();

        await repo.AddAsync(profile);
        var loaded = await repo.GetByUsernameAsync("ANASTASIA");   // регістр не важливий

        Assert.NotNull(loaded);
        Assert.Equal(profile.Id, loaded!.Id);
        Assert.Equal(new DarkPreset().CreateTheme().BackgroundColor, loaded.Theme.BackgroundColor);
        Assert.Equal(3, loaded.Blocks.Count);
        Assert.IsType<AboutBlock>(loaded.Blocks[0]);
        Assert.IsType<CommentsBlock>(loaded.Blocks[1]);
        Assert.IsType<FollowersBlock>(loaded.Blocks[2]);
    }

    [Fact]
    public async Task ProfileRepository_Save_PersistsThemeInfoAndBlockOrder()
    {
        var repo = new EfProfileRepository(_factory);
        await repo.AddAsync(NewProfile());


        var loaded = (await repo.GetByUserIdAsync("user-1"))!;
        loaded.ChangeTheme(new LightPreset().CreateTheme());
        loaded.UpdateInfo("Нова назва", "Про мене");
        loaded.MoveBlockDown(loaded.Blocks[0].Id);
        await repo.SaveAsync(loaded);

        var reloaded = (await repo.GetByUserIdAsync("user-1"))!;
        Assert.Equal(new LightPreset().CreateTheme().BackgroundColor, reloaded.Theme.BackgroundColor);
        Assert.Equal("Нова назва", reloaded.DisplayName);
        Assert.IsType<CommentsBlock>(reloaded.Blocks[0]);
        Assert.IsType<AboutBlock>(reloaded.Blocks[1]);
    }

    [Fact]
    public async Task CommentRepository_AddReply_StoresParentLink()
    {
        // Arrange
        var profiles = new EfProfileRepository(_factory);
        var comments = new EfCommentRepository(_factory);
        var profile = NewProfile();
        await profiles.AddAsync(profile);
        var parent = new Comment(profile.Id, "user-2", "Привіт!");
        var reply = parent.CreateReply("user-1", "Дякую!");

        // Act
        await comments.AddAsync(parent);
        await comments.AddAsync(reply);
        var wall = await comments.GetByProfileAsync(profile.Id);

        // Assert
        Assert.Equal(2, wall.Count);
        var loadedParent = wall.Single(c => !c.IsReply);
        var loadedReply = wall.Single(c => c.IsReply);
        Assert.Equal(parent.Id, loadedParent.Id);
        Assert.Equal("Привіт!", loadedParent.Text);
        Assert.Equal(parent.Id, loadedReply.ParentCommentId);
        Assert.Equal("user-1", loadedReply.AuthorId);
    }

    [Fact]
    public async Task CommentRepository_Delete_RemovesComment()
    {
        // Arrange
        var profiles = new EfProfileRepository(_factory);
        var comments = new EfCommentRepository(_factory);
        var profile = NewProfile();
        await profiles.AddAsync(profile);
        var comment = new Comment(profile.Id, "user-2", "Привіт!");
        await comments.AddAsync(comment);

        await comments.DeleteAsync(comment);

        Assert.Empty(await comments.GetByProfileAsync(profile.Id));
        Assert.Null(await comments.GetByIdAsync(comment.Id));
    }

    [Fact]
    public async Task FollowRepository_AddCountRemove_Works()
    {
 
        var follows = new EfFollowRepository(_factory);

        await follows.AddAsync(new Follow("user-1", "user-2"));

        Assert.True(await follows.ExistsAsync("user-1", "user-2"));
        Assert.False(await follows.ExistsAsync("user-2", "user-1"));
        Assert.Equal(1, await follows.CountFollowersAsync("user-2"));
        Assert.Equal(0, await follows.CountFollowersAsync("user-1"));
        Assert.Equal(1, await follows.CountFollowingAsync("user-1"));
        Assert.Equal(0, await follows.CountFollowingAsync("user-2"));

        await follows.RemoveAsync("user-1", "user-2");

        Assert.False(await follows.ExistsAsync("user-2", "user-1"));
        Assert.Equal(0, await follows.CountFollowersAsync("user-2"));
    }

}