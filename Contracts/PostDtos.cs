using System.ComponentModel.DataAnnotations;

namespace TheStreets_BE.Contracts
{
    public sealed class PostCreateRequest
    {
        [Required, MinLength(1), MaxLength(2000)]
        public string Message { get; set; } = string.Empty;
    }

    public sealed class PostUpdateRequest
    {
        [Required, MinLength(1), MaxLength(2000)]
        public string Message { get; set; } = string.Empty;
    }

    public sealed class CommentCreateRequest
    {
        [Required, MinLength(1), MaxLength(2000)]
        public string Message { get; set; } = string.Empty;
    }
}