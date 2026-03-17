using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheStreets_BE.Contracts;
using TheStreets_BE.Data;
using TheStreets_BE.Models;

namespace TheStreets_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TheStreetsController : ControllerBase
    {
        private readonly TheStreetsDbContext _db;

        public TheStreetsController(TheStreetsDbContext db) => _db = db;

        private string? CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);
        private string CurrentUserName => User.Identity?.Name ?? "Anonymous";
        private bool IsSuperAdmin => User.IsInRole("SuperAdmin");

        private const string Prefix = "Word on the street is ";

        // GET: api/thestreets (greeting)
        [HttpGet]
        [AllowAnonymous]
        public ActionResult<string> SayHi() => Ok("Hiii Bestie!");

        // GET: api/thestreets/posts
        [HttpGet("posts")]
        [AllowAnonymous]

        public async Task<ActionResult<IEnumerable<PostResponse>>> GetAll(CancellationToken ct)
        {
            // Project directly to DTO with counts
            var posts = await _db.Posts
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new PostResponse
                {
                    Id = p.Id,
                    Message = p.Message,
                    CreatedByUserId = p.CreatedByUserId,
                    CreatedByDisplayName = p.CreatedByDisplayName,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    LikeCount = p.Likes.Count,
                    CommentCount = p.Comments.Count
                })
                .ToListAsync(ct);

            return Ok(posts);
        }


        // GET: api/thestreets/{id}
        [HttpGet("{id:int}")]
        [AllowAnonymous]

        public async Task<ActionResult<PostResponse>> GetById(int id, CancellationToken ct)
        {
            var post = await _db.Posts
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => new PostResponse
                {
                    Id = p.Id,
                    Message = p.Message,
                    CreatedByUserId = p.CreatedByUserId,
                    CreatedByDisplayName = p.CreatedByDisplayName,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    LikeCount = p.Likes.Count,
                    CommentCount = p.Comments.Count
                })
                .FirstOrDefaultAsync(ct);

            return post is null ? NotFound() : Ok(post);
        }


        // POST: api/thestreets
        [HttpPost]
        [Authorize]

        public async Task<ActionResult<PostResponse>> Create([FromBody] PostCreateRequest request, CancellationToken ct)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            if (string.IsNullOrWhiteSpace(CurrentUserId)) return Unauthorized("Missing X-User-Id header.");

            var msg = request.Message.Trim();
            if (!msg.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase))
                msg = Prefix + msg;

            var post = new BlogPost
            {
                Message = msg,
                CreatedByUserId = CurrentUserId!,
                CreatedByDisplayName = CurrentUserName,
                CreatedAt = DateTimeOffset.UtcNow
            };

            _db.Posts.Add(post);
            await _db.SaveChangesAsync(ct);

            // materialize counts
            post = await _db.Posts
                .Include(p => p.Likes).Include(p => p.Comments)
                .FirstAsync(p => p.Id == post.Id, ct);

            return CreatedAtAction(nameof(GetById), new { id = post.Id }, post.ToResponse());
        }

        // PUT: api/thestreets/{id}
        [HttpPut("{id:int}")]
        [Authorize]

        public async Task<ActionResult<PostResponse>> Update(int id, [FromBody] PostUpdateRequest request, CancellationToken ct)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            if (string.IsNullOrWhiteSpace(CurrentUserId)) return Unauthorized("Missing X-User-Id header.");

            var post = await _db.Posts.Include(p => p.Likes).Include(p => p.Comments)
                                      .FirstOrDefaultAsync(p => p.Id == id, ct);
            if (post is null) return NotFound();

            var isOwner = string.Equals(post.CreatedByUserId, CurrentUserId, StringComparison.Ordinal);
            if (!isOwner) return Forbid();

            var msg = request.Message.Trim();
            if (!msg.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase))
                msg = Prefix + msg;

            post.Message = msg;
            post.UpdatedAt = DateTimeOffset.UtcNow;

            await _db.SaveChangesAsync(ct);
            return Ok(post.ToResponse());
        }

        // DELETE: api/thestreets/{id}
        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(CurrentUserId)) return Unauthorized("Missing X-User-Id header.");

            var post = await _db.Posts.FindAsync(new object[] { id }, ct);
            if (post is null) return NotFound();

            // Owner or SuperAdmin can delete
            var isOwner = string.Equals(post.CreatedByUserId, CurrentUserId, StringComparison.Ordinal);
            if (!isOwner && !IsSuperAdmin) return Forbid();

            _db.Posts.Remove(post);
            await _db.SaveChangesAsync(ct);

            return NoContent();
        }

        // POST: api/thestreets/{id}/like
        [HttpPost("{id:int}/like")]
        [Authorize]
        public async Task<IActionResult> Like(int id, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(CurrentUserId)) return Unauthorized();

            var post = await _db.Posts.FindAsync(new object[] { id }, ct);
            if (post is null) return NotFound();

            var exists = await _db.Likes.AnyAsync(l => l.PostId == id && l.UserId == CurrentUserId, ct);
            if (!exists)
            {
                _db.Likes.Add(new PostLike { PostId = id, UserId = CurrentUserId!, CreatedAt = DateTimeOffset.UtcNow });
                await _db.SaveChangesAsync(ct);
            }

            var count = await _db.Likes.CountAsync(l => l.PostId == id, ct);
            return Ok(new { postId = id, likes = count });
        }

        // DELETE: api/thestreets/{id}/like
        [HttpDelete("{id:int}/like")]
        [Authorize]
        public async Task<IActionResult> Unlike(int id, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(CurrentUserId)) return Unauthorized();

            var like = await _db.Likes.FirstOrDefaultAsync(l => l.PostId == id && l.UserId == CurrentUserId, ct);
            if (like is null) return NoContent();

            _db.Likes.Remove(like);
            await _db.SaveChangesAsync(ct);

            var count = await _db.Likes.CountAsync(l => l.PostId == id, ct);
            return Ok(new { postId = id, likes = count });
        }

        // GET: api/thestreets/{id}/likes/count
        [HttpGet("{id:int}/likes/count")]
        [AllowAnonymous]
        public async Task<IActionResult> LikeCount(int id, CancellationToken ct)
        {
            var count = await _db.Likes.CountAsync(l => l.PostId == id, ct);
            return Ok(new { postId = id, likes = count });
        }

        // POST: api/thestreets/{id}/comments
        [HttpPost("{id:int}/comments")]
        [Authorize]
        public async Task<ActionResult<PostComment>> AddComment(int id, [FromBody] CommentCreateRequest request, CancellationToken ct)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            if (string.IsNullOrWhiteSpace(CurrentUserId)) return Unauthorized();

            var post = await _db.Posts.FindAsync(new object[] { id }, ct);
            if (post is null) return NotFound();

            var c = new PostComment
            {
                PostId = id,
                UserId = CurrentUserId!,
                UserDisplayName = CurrentUserName,
                Message = request.Message.Trim(),
                CreatedAt = DateTimeOffset.UtcNow
            };

            _db.Comments.Add(c);
            await _db.SaveChangesAsync(ct);

            return CreatedAtAction(nameof(GetComments), new { id }, c);
        }

        // GET: api/thestreets/{id}/comments
        [HttpGet("{id:int}/comments")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<PostComment>>> GetComments(int id, CancellationToken ct)
        {
            var comments = await _db.Comments
                .AsNoTracking()
                .Where(c => c.PostId == id)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync(ct);

            return Ok(comments);
        }

        // DELETE: api/thestreets/{postId}/comments/{commentId}
        [HttpDelete("{postId:int}/comments/{commentId:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int postId, int commentId, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(CurrentUserId)) return Unauthorized();

            var c = await _db.Comments.FirstOrDefaultAsync(x => x.Id == commentId && x.PostId == postId, ct);
            if (c is null) return NotFound();

            var isOwner = string.Equals(c.UserId, CurrentUserId, StringComparison.Ordinal);
            if (!isOwner && !IsSuperAdmin) return Forbid();

            _db.Comments.Remove(c);
            await _db.SaveChangesAsync(ct);
            return NoContent();
        }
    }
}
