using FashionStore.Domain.Entities;
using FashionStore.Domain.Interfaces;
using FashionStore.Infrastructure.Context;
using FashionStore.Infrastructure.UnitOfWork;
using FashionStore.Web.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


// ==========================================
// SERILOG CONFIGURATION
// ==========================================

builder.Host.UseSerilog((context, services, config) =>
{
    config
        .MinimumLevel.Debug()
        .WriteTo.Console()
        .WriteTo.File(
            "logs/fashionstore-.txt",
            rollingInterval: Serilog.RollingInterval.Day,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
        );
});


// ==========================================
// DATABASE CONNECTION - SUPABASE (PostgreSQL) EXCLUSIVO
// ==========================================
// La aplicación trabaja SOLO con Supabase
// No hay fallback a SQL Server

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Supabase requiere password del environment
var password = Environment.GetEnvironmentVariable("SUPABASE_PASSWORD") ?? 
               builder.Configuration["SUPABASE_PASSWORD"];

if (string.IsNullOrEmpty(password))
{
    throw new InvalidOperationException("❌ SUPABASE_PASSWORD requerido en variable de entorno o appsettings");
}

if (connectionString != null && connectionString.Contains("${SUPABASE_PASSWORD}"))
{
    connectionString = connectionString.Replace("${SUPABASE_PASSWORD}", password);
}

builder.Services.AddDbContext<FashionStoreDbContext>(options =>
    options.UseNpgsql(connectionString, x => x.MigrationsHistoryTable("__EFMigrationsHistory", "public"))
        .LogTo(Console.WriteLine, LogLevel.Information));


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

    options.AccessDeniedPath = "/Identity/Account/AccessDenied";

    options.SlidingExpiration = true;

    options.ExpireTimeSpan = TimeSpan.FromDays(30);
});


// ==========================================
// MVC
// ==========================================

builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();


// ==========================================
// MEMORY CACHE
// ==========================================

builder.Services.AddMemoryCache();


// ==========================================
// SERVICIOS
// ==========================================

builder.Services.AddScoped<IConfiguracionSistemaService, ConfiguracionSistemaService>();


// ==========================================
// SERVICIOS DE VENTAS
// ==========================================

builder.Services.AddScoped<ICarritoServiceWeb, CarritoService>();
builder.Services.AddScoped<IBuscadorProductosWeb, BuscadorProductos>();
builder.Services.AddScoped<IServicioVentasWeb, ServicioVentas>();


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

    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRouting();

// ==========================================
// AUTH
// ==========================================

app.UseAuthentication();

app.UseAuthorization();

// ==========================================
// MIDDLEWARE: Redirigir a Login si no autenticado
// ==========================================

app.Use(async (context, next) =>
{
    // Si la ruta es la ra�z "/" o "/Index.html"
    if (context.Request.Path == "/" || context.Request.Path == "/index.html")
    {
        // Si no est� autenticado, redirigir a Login
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            context.Response.Redirect("/Identity/Account/Login");
            return;
        }
        // Si est� autenticado, redirigir al Dashboard
        else
        {
            context.Response.Redirect("/Home/Index");
            return;
        }
    }

    await next(context);
});


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

// ==========================================
// INICIALIZACIÓN DE ROLES
// ==========================================

await FashionStore.Infrastructure.Data.DbInitializer.Initialize(app);

app.Run();