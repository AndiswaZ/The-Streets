namespace TheStreets_BE.Models
{
    public class PostLike
    {
        public int Id { get; set; }

        public int PostId { get; set; }
        public BlogPost Post { get; set; } = default!;

        public string UserId { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}