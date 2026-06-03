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

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Nếu User chưa login mà cố tình vào trang cấm, hệ thống sẽ đá về đây
        options.LoginPath = "/auth/Login";

        // Thời gian cookie có hiệu lực 
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
    });

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
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
