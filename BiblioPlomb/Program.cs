using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BiblioPlomb.Data;
using BiblioPlomb.Services;

var builder = WebApplication.CreateBuilder(args);

// base de données pour utiliser SQLite
builder.Services.AddDbContext<BiblioPlombDB>(opt => opt.UseSqlite("Data Source=BiblioPlomb.db"));

//builder.Services.AddDbContext<BiblioPlombDB>(options =>
//    options.UseSqlite(builder.Configuration.GetConnectionString("BiblioPlombContext")));



// les services pour les contrôleurs et les vues
builder.Services.AddControllersWithViews();

// Ajout des services
builder.Services.AddScoped<LivreService>();
builder.Services.AddScoped<GenreService>();
builder.Services.AddScoped<AuteurService>();

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

//Généré par le controller
app.UseHttpsRedirection();
app.UseStaticFiles(); // Pour les fichiers statiques (CSS, JS, images, etc.) 
app.UseRouting();
app.UseAuthorization();

//routes pour les contrôleurs
app.MapControllerRoute(
    name: "default", 
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "livre", 
    pattern: "Create/{action=Index}/{id?}", 
    defaults: new { controller = "Livres" });

app.MapControllerRoute(
    name: "genre",
    pattern: "Create/{action=Index}/{id?}",
    defaults: new { controller = "Genres" });

// Appeler la méthode pour importer le JSON
//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    var livreService = services.GetRequiredService<LivreService>();
//    await livreService.ImporJsonLivres(Path.Combine("data", "livres.json"));
//}

app.Run();
