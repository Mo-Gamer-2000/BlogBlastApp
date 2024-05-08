using BlogBlast.Components;
using BlogBlast.Components.Account;
using BlogBlast.Data;
using BlogBlast.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options => options.DetailedErrors = true);
// Set DetailedErrors to true, to help debugging,
// error where text editor is not rendering at 1st instance, but if page is refreshed, then it loads

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

// Retrieve connection string from configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
// Add DbContextFactory to services, using SQL Server
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
// Add database developer page exception filter
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configure Identity
builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false) // Changed to false as I do not need this option
    .AddRoles<IdentityRole>() // Call Identity Role - I have moved this up
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

// Use a no-op email sender for Identity
builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

// Registering services in the dependency injection container
builder.Services
    // Registering SeedService for seeding data
    .AddTransient<ISeedService, SeedService>()
    // Registering CategoryService for managing categories
    .AddTransient<ICategoryService, CategoryService>()
    // Registering PostAdminService for managing blog posts
    .AddTransient<IPostAdminService, PostAdminService>()
    // Registering PostService for displaying blog posts
    .AddTransient<IPostService, PostService>()
    // Registering SubscriptionService for blog subscription
    .AddTransient<ISubscriptionService, SubscriptionService>();

var app = builder.Build();

// Called the method with services as parameter
await SeedAsync(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();

// Created Seed Async Method to create Service
static async Task SeedAsync(IServiceProvider services)
{
    var scope = services.CreateScope();
    var seedService = scope.ServiceProvider.GetRequiredService<ISeedService>();
    await seedService.SeedDataAsync();
}