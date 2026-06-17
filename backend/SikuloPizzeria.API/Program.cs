using SikuloPizzeria.Core.Interfaces;
using SikuloPizzeria.Core.Services;
using SikuloPizzeria.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

const string AngularCorsPolicy = "AngularClient";

// Services de l'API
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Services du Core et de l'Infrastructure
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<ICategorieService, CategorieService>();
builder.Services.AddScoped<IProduitService, ProduitService>();

// Autorisation du frontend Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy(AngularCorsPolicy, policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:4200",
                "http://127.0.0.1:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Création de l'application
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Middleware exécutés après builder.Build()
app.UseCors(AngularCorsPolicy);

app.UseAuthorization();

app.MapControllers();

app.Run();
