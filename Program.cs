using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configure Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/LoginPage"; // Placeholder for login path
        options.LogoutPath = "/Account/LogoutPage"; // Placeholder for logout path
        options.ExpireTimeSpan = TimeSpan.FromDays(7); // Session expiration (unchanged)
        options.SlidingExpiration = true; // Renew expiration automatically
    });

builder.Services.AddAuthorization();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register FluentValidation services
builder.Services.AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters();

// Register validators from current assembly
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// Session & Context
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();

var app = builder.Build();

// Configure HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Shared/ErrorPage"); // Placeholder error handler
}

app.UseSession();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Main}/{action=Start}/{id?}"); // Placeholder route

app.Run();
