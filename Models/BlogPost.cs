namespace TheStreets_BE.Models
{
    public class BlogPost
    {
        public int Id { get; set; }

        // Message-only for now
        public string Message { get; set; } = string.Empty;

        // Ownership & audit
        public string CreatedByUserId { get; set; } = string.Empty;
        public string CreatedByDisplayName { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }

        // Navigation
        public ICollection<PostLike> Likes { get; set; } = new List<PostLike>();
        public ICollection<PostComment> Comments { get; set; } = new List<PostComment>();
    }
}