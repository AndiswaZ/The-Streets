using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpGet]
        public ActionResult<string> SayHi() => Ok("Hiii Bestie!");

        [HttpGet("posts")]
        public async Task<ActionResult<IEnumerable<BlogPost>>> GetAll(CancellationToken ct)
        {
            var posts = await _db.Posts.AsNoTracking().OrderBy(p => p.Id).ToListAsync(ct);
            return Ok(posts);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<BlogPost>> GetById(int id, CancellationToken ct)
        {
            var post = await _db.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, ct);
            return post is null ? NotFound() : Ok(post);
        }

        [HttpPost]
        public async Task<ActionResult<BlogPost>> Create([FromBody] BlogPost input, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(input.Message)) return BadRequest("Message is required.");
            var post = new BlogPost { Message = input.Message.Trim() };
            _db.Posts.Add(post);
            await _db.SaveChangesAsync(ct);
            return CreatedAtAction(nameof(GetById), new { id = post.Id }, post);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<BlogPost>> Update(int id, [FromBody] BlogPost input, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(input.Message)) return BadRequest("Message is required.");
            var post = await _db.Posts.FindAsync(new object[] { id }, ct);
            if (post is null) return NotFound();
            post.Message = input.Message.Trim();
            await _db.SaveChangesAsync(ct);
            return Ok(post);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var post = await _db.Posts.FindAsync(new object[] { id }, ct);
            if (post is null) return NotFound();
            _db.Posts.Remove(post);
            await _db.SaveChangesAsync(ct);
            return NoContent();
        }
    }
}