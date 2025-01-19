using Microsoft.EntityFrameworkCore;
using UniversityRanking.Models;
using UniversityRanking.Models.University;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton(new FileUserStore("users.json"));
builder.Services.AddAuthentication("Cookies")
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Путь к логину
        options.LogoutPath = "/Account/Logout"; // Путь к выходу
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Время действия куки
        options.SlidingExpiration = true; // Продление действия куки при активности
        options.Cookie.IsEssential = true; // Куки не будут сохраняться между сеансами
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.Expiration = null; // Куки будут существовать только во время сеанса браузера
    });

builder.Services.AddDbContext<UniversityContext>(options =>
{
    options.UseSqlite(builder.Configuration["UniversityDatabase:ConnectionString"]);
});

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();