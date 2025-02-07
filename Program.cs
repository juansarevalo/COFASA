using CoreContable.Middleware;
using CoreContable.Services;
using CoreContable.Utils;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Serilog;
using DbContext = CoreContable.DbContext;

var builder = WebApplication.CreateBuilder(args);

// Configuración de logging con Serilog
builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
    loggerConfiguration
        .ReadFrom.Configuration(hostingContext.Configuration)
        .Enrich.FromLogContext()
);

// Política de autorización para usuarios autenticados
var onlyAuthenticatedUsers = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();

// Servicios y configuraciones de MVC, Razor Pages y DbContext
builder.Services.AddControllersWithViews(opts =>
    opts.Filters.Add(new AuthorizeFilter(onlyAuthenticatedUsers))
);
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddDbContext<DbContext>(options => {
    options.UseSqlServer(
        "name=DefaultWin",
        o => o.UseCompatibilityLevel(120)
    ).EnableSensitiveDataLogging();
});

// Configuración de autenticación con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
        options.AccessDeniedPath = "/security/login";
        options.LoginPath = "/security/login";
    });

// Configuración de sesiones
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Registro de servicios y repositorios
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISecurityRepository, SecurityRepository>();
builder.Services.AddScoped<ICiasRepository, CiasRepository>();
builder.Services.AddScoped<IDmgCuentasRepository, DmgCuentasRepository>();
builder.Services.AddScoped<ICentroCostoRepository, CentroCostoRepository>();
builder.Services.AddScoped<ICentroCuentaRepository, CentroCuentaRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
builder.Services.AddScoped<IUserCiaRepository, UserCiaRepository>();
builder.Services.AddSingleton<ICoreHash, CoreHash>();
builder.Services.AddScoped<AccountUtils>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();
app.UseMiddleware<MachineAuthorizationMiddleware>();

// Middleware y configuración de la aplicación
app.UseDeveloperExceptionPage();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapDefaultControllerRoute();

app.UseSession();
app.MapControllerRoute(name: "default", pattern: "{controller=Security}/{action=Login}");

app.Run();
