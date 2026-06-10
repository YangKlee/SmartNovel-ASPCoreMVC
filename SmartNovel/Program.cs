using Amazon.Runtime;
using Amazon.S3;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SmartNovel.Models;
using SmartNovel.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient<ChapterContentService>();
builder.Services.AddControllersWithViews();
var conString = builder.Configuration.GetConnectionString("SmartNovel");
builder.Services.AddDbContext<SmartTruyenDbContext>(options =>
    options.UseSqlServer(conString));
var r2Section = builder.Configuration.GetSection("CloudflareR2");
var accountId = r2Section["AccountId"];
var accessKey = r2Section["AccessKey"];
var secretKey = r2Section["SecretKey"];
var serviceUrl = $"https://{accountId}.r2.cloudflarestorage.com";
var credentials = new BasicAWSCredentials(accessKey, secretKey);
var config = new AmazonS3Config
{
    ServiceURL = serviceUrl,
};
builder.Services.AddSingleton<IAmazonS3>(new AmazonS3Client(credentials, config));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Nếu User chưa login mà cố tình vào trang cấm, hệ thống sẽ đá về đây
        options.LoginPath = "/auth/Login";

        // Thời gian cookie có hiệu lực 
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
    })
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
        options.SaveTokens = true;
        options.Events = new Microsoft.AspNetCore.Authentication.OAuth.OAuthEvents
        {
            OnRemoteFailure = context =>
            {
                context.Response.Redirect("/Auth/Login");
                context.HandleResponse();
                return Task.CompletedTask;
            }
        };
    });


builder.Services.AddControllersWithViews();
builder.Services.AddTransient<SmartNovel.Services.MailServices>();
builder.Services.AddSingleton<FileStorageServices>();
builder.Services.AddScoped<SmartNovel.Services.MenuDashboardServices>();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
