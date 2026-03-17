using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TheStreets_BE.Data;
using TheStreets_BE.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TheStreets API",
        Version = "v1",
        Description = "API for posts with ownership, likes, and comments"
    });

    // Optional: show custom header usage in Swagger (as API keys)
    c.AddSecurityDefinition("X-User-Id", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "X-User-Id",
        Type = SecuritySchemeType.ApiKey,
        Description = "Required for write operations (simulated auth)"
    });
    c.AddSecurityDefinition("X-User-Role", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "X-User-Role",
        Type = SecuritySchemeType.ApiKey,
        Description = "Optional. Use 'SuperAdmin' to allow cross-user deletes."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme{ Reference = new OpenApiReference{ Type = ReferenceType.SecurityScheme, Id = "X-User-Id" }}, new List<string>()
        }
    });
});

// SQLite
var cs = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TheStreetsDbContext>(options => options.UseSqlite(cs));

// Dev header auth

builder.Services
    .AddAuthentication("DevHeader")
    .AddScheme<AuthenticationSchemeOptions, DevHeaderAuthHandler>("DevHeader", _ => { });


builder.Services.AddAuthorization();

var app = builder.Build();

// Apply migrations + seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TheStreetsDbContext>();
    db.Database.Migrate();
    DbSeeder.Seed(db);
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();