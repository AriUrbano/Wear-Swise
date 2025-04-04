var builder = WebApplication.CreateBuilder(args);

// Añade estos servicios en ESTE ORDEN:
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache(); // Para sesiones
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Añade el servicio de autorización
builder.Services.AddAuthorization();

var app = builder.Build();

// Configura el pipeline en ESTE ORDEN:
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Estos middlewares en ORDEN CORRECTO:
app.UseAuthentication(); // Primero autenticación
app.UseAuthorization();  // Luego autorización
app.UseSession();       // Después sesión

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();