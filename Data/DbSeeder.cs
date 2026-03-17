using TheStreets_BE.Models;

namespace TheStreets_BE.Data
{
    public static class DbSeeder
    {
        public static void Seed(TheStreetsDbContext db)
        {
            if (db.Posts.Any()) return;
            db.Posts.Add(new BlogPost { Message = "Hiii Bestie! (seeded)" });
            db.SaveChanges();
        }
    }
}