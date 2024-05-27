//using DAL.Models;
//using DAL;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using BLL;

//var builder = WebApplication.CreateBuilder(args);
//var config = builder.Configuration;
//var constr = config.GetConnectionString("SqlConnection");

//builder.Services.AddDbContext<SoundContext>(options =>
//	options.UseSqlServer(constr));

////builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
////	options.LoginPath ="/Users/Login");
using BLL;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
var constr = config.GetConnectionString("SqlConnection");

builder.Services.AddDbContext<SoundContext>(options =>
    options.UseSqlServer(constr));

builder.Services.AddAuthorization();

builder.Services.AddScoped<MediaService>();
builder.Services.AddScoped<MusicFinderService>();
builder.Services.AddHttpClient(); // Регистрируем HttpClient

// Добавляем OpenAIService как Scoped сервис
builder.Services.AddScoped<OpenAIService>(sp => new OpenAIService(
    sp.GetRequiredService<HttpClient>(),
    builder.Configuration["OpenAI:ApiKey"],
    sp.GetRequiredService<MusicFinderService>()
));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60); // Установка времени жизни сессии
    options.Cookie.HttpOnly = true; // Сессионные куки доступны только через HTTP-запросы
    options.Cookie.IsEssential = true; // Сессионные куки необходимы для работы приложения
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
app.UseSession();
app.Run();



//builder.Services.AddAuthorization();

//builder.Services.AddScoped<MediaService>();
//builder.Services.AddScoped<MusicFinderService>();
////builder.Services.AddHttpClient<OpenAIService>();

////// Добавляем API ключ
////builder.Services.AddSingleton(sp => new OpenAIService(sp.GetRequiredService<HttpClient>(), "sk-proj-L7NflgdwxFNXBcZsgs9XT3BlbkFJLgZmc7tg0ZxAAV8qtjPB"));


//builder.Services.AddSingleton(sp => new OpenAIService(
//    sp.GetRequiredService<HttpClient>(),
//    builder.Configuration["OpenAI:sk-proj-L7NflgdwxFNXBcZsgs9XT3BlbkFJLgZmc7tg0ZxAAV8qtjPB"],
//    sp.GetRequiredService<MusicFinderService>()
//));

//// Add services to the container.
//builder.Services.AddControllersWithViews();
//builder.Services.AddSession(options =>
//{
//    options.IdleTimeout = TimeSpan.FromMinutes(60); // Установка времени жизни сессии
//    options.Cookie.HttpOnly = true; // Сессионные куки доступны только через HTTP-запросы
//    options.Cookie.IsEssential = true; // Сессионные куки необходимы для работы приложения
//});


//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//	app.UseExceptionHandler("/Home/Error");
//	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//	app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();

//app.UseRouting();

//app.UseAuthentication();
//app.UseAuthorization();

//app.MapControllerRoute(
//	name: "default",
//	pattern: "{controller=Home}/{action=Index}/{id?}");
//app.UseSession();
//app.Run();
