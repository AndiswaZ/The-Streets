namespace TheStreets_BE.Models
{
    public class PostComment
    {
        public int Id { get; set; }

        public int PostId { get; set; }
        public BlogPost Post { get; set; } = default!;

        public string UserId { get; set; } = string.Empty;
        public string UserDisplayName { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}