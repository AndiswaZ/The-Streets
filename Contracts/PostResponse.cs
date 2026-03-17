namespace TheStreets_BE.Contracts
{
    public sealed class PostResponse
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;

        public string CreatedByUserId { get; set; } = string.Empty;
        public string CreatedByDisplayName { get; set; } = string.Empty;

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }

        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
    }
}