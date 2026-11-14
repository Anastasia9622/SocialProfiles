using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using SocialProfiles.Core.Application.Interfaces;
using SocialProfiles.Core.Application.Services;
using SocialProfiles.Web.Components;
using SocialProfiles.Web.Components.Account;
using SocialProfiles.Web.Data;
using SocialProfiles.Web.Data.Repositories;
using SocialProfiles.Core.Domain.Media;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
// Фабрика контексту для наших репозиторіїв (короткі контексти на кожну операцію).
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// Звичайний ApplicationDbContext залишаємо для Identity, але беремо його з тієї ж фабрики.
builder.Services.AddScoped<ApplicationDbContext>(provider =>
    provider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext());

// Сховища та сервіси.
builder.Services.AddScoped<IProfileRepository, EfProfileRepository>();
builder.Services.AddScoped<ICommentRepository, EfCommentRepository>();
builder.Services.AddScoped<IFollowRepository, EfFollowRepository>();
builder.Services.AddScoped<ProfileService>();
builder.Services.AddScoped<CommentService>();
builder.Services.AddScoped<FollowService>();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

var uploadsPath = builder.Configuration["Storage:UploadsPath"]
    ?? Path.Combine(builder.Environment.ContentRootPath, "App_Data", "uploads");

builder.Services.AddSingleton<IImageStorage>(new LocalImageStorage(uploadsPath));
builder.Services.AddScoped<ProfileImageService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads",
    OnPrepareResponse = ctx =>
    {

        ctx.Context.Response.Headers["X-Content-Type-Options"] = "nosniff";
        ctx.Context.Response.Headers["Cache-Control"] = "public,max-age=604800";
    }
});

app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapAdditionalIdentityEndpoints();

app.Run();