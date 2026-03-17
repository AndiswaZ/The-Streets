The Streets — Backend (ASP.NET Core Web API)
A minimal, clean Web API for posting short messages (“posts”), built with ASP.NET Core, Entity Framework Core, and SQLite.
Includes Swagger for interactive docs, EF migrations, and simple data seeding.

Status: Local dev is working (Swagger ✅, DB seeded ✅). Next steps: timestamps, pagination, auth, and deployment.


🚀 Features

.NET 8 ASP.NET Core Web API
Entity Framework Core 8 with SQLite (file‑based dev DB)
Automatic DB creation & seeding on startup
Swagger / OpenAPI UI at /swagger
Clean CRUD endpoints for posts
Ready to switch to SQL Server if needed
Good defaults: AsNoTracking() for reads, CancellationToken support (optional), DTO‑ready


🧰 Tech Stack

Runtime: .NET 8
Web: ASP.NET Core Web API
ORM: Entity Framework Core 8
Database (Dev): SQLite (thestreets.db)
Docs: Swagger / Swashbuckle


📂 Project Structure
TheStreets_BE/
- Controllers/
    -TheStreetsController.cs
- Data/
     -TheStreetsDbContext.cs
     - DbSeeder.cs
- Migrations/                # EF Core migrations
- Models/
     - BlogPost.cs
- Program.cs
- appsettings.json
- TheStreets_BE.csproj


🛠️ Getting Started (Local)
1) Prerequisites

.NET 8 SDK
Visual Studio 2022 (or VS Code)
(Optional) DB Browser for SQLite to view thestreets.db

2) Clone & Restore
git clone https://github.com/AndiswaZ/The-Streets.git
cd The-Streets
# open the solution or project:
#   Windows: double-click TheStreets_BE.sln, or
#   CLI:     dotnet restore

3) Configure Connection String (SQLite)
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=thestreets.db"
  }
}

4) Apply Migrations & Run

You can either let Program.cs run db.Database.Migrate() on startup or do it manually:

Manual (Package Manager Console in Visual Studio):
# Set Default project to TheStreets_BE
Add-Migration InitialCreate
Update-Database

Run the API:
dotnet run --project TheStreets_BE

Open Swagger:
https://localhost:xxxx/swagger

🧪 Endpoints
Base route: /api/thestreets

Method            Route            Description                    Body Example

GET      	  /api/thestreets      Greeting (health check)
	
GET	      /api/thestreets/posts   Get all posts (ordered by Id)
	
GET	      /api/thestreets/{id}	  Get a post by Id
	
POST	    /api/thestreets          Create a new post              { "message": "Hi bestie!" }

PUT	      /api/thestreets/{id}    Update an existing post          { "message": "Updated message" }

DELETE	  /api/thestreets/{id}      Delete a post
	

Entity: BlogPost
{
  "id": 1,
  "message": "Hiii Bestie! (seeded)"
}

🗄️ Database Notes

SQLite files are created in the project root:

thestreets.db, thestreets.db-shm, thestreets.db-wal


To browse the DB, open thestreets.db in DB Browser for SQLite.
Seeding happens on startup in DbSeeder.Seed() (kept idempotent).

Ignore local DB files in Git
.gitignore
# Build
bin/
obj/

# VS cache/settings
.vs/
*.user

# SQLite
*.db
*.db-shm
*.db-wal

🔄 Switch to SQL Server (Optional)
If you want to manage the DB in SSMS instead of SQLite:
Program.cs
builder.Services.AddDbContext<TheStreetsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    
appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR-SERVER\\SQLEXPRESS;Database=TheStreets;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}

Then run:
Add-Migration SqlServerInit
Update-Database

Open SSMS → connect to your instance → Databases → TheStreets.

🧹 Common Commands
# Restore packages
dotnet restore

# Build
dotnet build

# Run
dotnet run --project TheStreets_BE

# Create a migration
Add-Migration <Name>

# Apply migrations
Update-Database

# Remove last migration (if not applied)
Remove-Migration

🧩 Troubleshooting


Project shows as (unloaded) in Solution Explorer
The .sln may point to a moved .csproj. Remove the project from the solution, then Add → Existing Project… and reselect TheStreets_BE.csproj.


“Running last successful build” / source mismatch
Clean + close VS → delete bin/, obj/, and the hidden .vs folder → reopen and rebuild.


Microsoft.OpenApi.Models not found
Ensure Swashbuckle.AspNetCore is installed and the using is exactly:
using Microsoft.OpenApi.Models;

EF Core version mismatch
On .NET 8, use EF Core 8.x (e.g., 8.0.8), not 10.x.



🗺️ Roadmap

 Add CreatedAt / UpdatedAt timestamps to BlogPost
 Add pagination to GET /posts (?page=1&pageSize=20)
 Add search/filter (?q=bestie)
 Add DTOs + validation attributes
 Add global exception handling (ProblemDetails)
 Add authentication (JWT)
 Deploy to Azure


🤝 Contributing

Fork the repo
Create a feature branch: git checkout -b feat/awesome
Commit changes: git commit -m "feat: add awesome"
Push branch: git push origin feat/awesome
Open a Pull Request


📜 License
MIT (or your preferred license). Add a LICENSE file at the repo root.

🧷 Credits
Built with love by Andiswa (and Bestie 💖).
Tech: ASP.NET Core, EF Core, SQLite, Swagger.
