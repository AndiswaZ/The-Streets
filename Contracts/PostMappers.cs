using TheStreets_BE.Models;

namespace TheStreets_BE.Contracts
{
    public static class PostMappers
    {
        public static PostResponse ToResponse(this BlogPost p)
            => new PostResponse
            {
                Id = p.Id,
                Message = p.Message,
                CreatedByUserId = p.CreatedByUserId,
                CreatedByDisplayName = p.CreatedByDisplayName,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                LikeCount = p.Likes?.Count ?? 0,
                CommentCount = p.Comments?.Count ?? 0
            };
    }
}