using FashionStore.Domain.Entities;
using FashionStore.Domain.Interfaces;
using FashionStore.Infrastructure.Context;
using FashionStore.Infrastructure.UnitOfWork;
using FashionStore.Web.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


// ==========================================
// CONEXIÓN SQL SERVER
// ==========================================

builder.Services.AddDbContext<FashionStoreDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));


// ==========================================
// IDENTITY
// ==========================================

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        // LOGIN

        options.SignIn.RequireConfirmedAccount = false;

        // PASSWORD

        options.Password.RequireDigit = true;

        options.Password.RequireUppercase = true;

        options.Password.RequireLowercase = true;

        options.Password.RequireNonAlphanumeric = false;

        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<FashionStoreDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();


// ==========================================
// COOKIE LOGIN
// ==========================================

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";

    options.AccessDeniedPath = "/Identity/Account/Login";
});


// ==========================================
// MVC
// ==========================================

builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();


// ==========================================
// AUTOMAPPER
// ==========================================

builder.Services.AddAutoMapper(
    AppDomain.CurrentDomain.GetAssemblies());


// ==========================================
// UNIT OF WORK
// ==========================================

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


// ==========================================
// EMAIL SENDER
// ==========================================

builder.Services.AddTransient<IEmailSender, EmailSender>();


var app = builder.Build();


// ==========================================
// PIPELINE
// ==========================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();


// ==========================================
// AUTH
// ==========================================

app.UseAuthentication();

app.UseAuthorization();


// ==========================================
// RUTAS MVC
// ==========================================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


// ==========================================
// RAZOR PAGES
// ==========================================

app.MapRazorPages();

app.Run();