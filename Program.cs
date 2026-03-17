using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TheStreets_BE.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TheStreets API",
        Version = "v1",
        Description = "API for posts"
    });
});

// SQLite
var cs = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TheStreetsDbContext>(options =>
    options.UseSqlite(cs));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TheStreetsDbContext>();
    db.Database.Migrate();  // apply migrations at startup
    DbSeeder.Seed(db);
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TheStreets API v1");
    c.RoutePrefix = "swagger";
});

app.MapControllers();
app.Run();