using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Configuración esencial
builder.Services.AddControllersWithViews();

// Configuración de sesión
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configuración de autenticación CORREGIDA
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.LogoutPath = "/Account/Logout";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        // Solo requerir HTTPS en producción
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment() 
            ? CookieSecurePolicy.None 
            : CookieSecurePolicy.Always;
    });

// Configuración de autorización MODIFICADA
builder.Services.AddAuthorization(options =>
{
    // Elimina la política de fallback que requería autenticación global
    // options.FallbackPolicy = null;
    
    // En su lugar, usa [Authorize] en controladores/acciones específicas
});

var app = builder.Build();

// Middleware en ORDEN CORRECTO
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication(); // Primero autenticación
app.UseAuthorization();  // Luego autorización
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();