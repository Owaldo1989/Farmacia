using Farmacia.DAL;

var builder = WebApplication.CreateBuilder(args);

// Razor Pages
builder.Services.AddRazorPages();
builder.Services.AddTransient<UsuarioDAL>();
builder.Services.AddTransient<ArticuloDAL>();
builder.Services.AddTransient<CategoriaDAL>();
builder.Services.AddTransient<DashboardDAL>();
builder.Services.AddTransient<LaboratorioDAL>();
builder.Services.AddTransient<ProveedorDAL>();
builder.Services.AddTransient<UnidadDAL>();
builder.Services.AddTransient<UsuarioDAL>();
builder.Services.AddTransient<FacturaDAL>();
builder.Services.AddTransient<ProductoDAL>();
    builder.Services.AddTransient<TipoCambioDAL>();
// Cache para Session
builder.Services.AddDistributedMemoryCache();

// Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(365); // 1 años (ajusta)
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;

    // Cookie persistente (no se borra al cerrar el navegador)
    options.Cookie.MaxAge = TimeSpan.FromDays(365); // 1 años
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();



app.MapRazorPages();

app.Run();
