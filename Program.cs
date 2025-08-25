using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configure Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/User/UserLogin"; // Redirect if not authenticated
        options.LogoutPath = "/User/logout";   // Logout path
        options.ExpireTimeSpan = TimeSpan.FromDays(7); // Default expiration time
        options.SlidingExpiration = true; // Renew expiration automatically
    });

builder.Services.AddAuthorization();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register FluentValidation services
builder.Services.AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters();

// Register validators from the current assembly
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// Login
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseSession();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

