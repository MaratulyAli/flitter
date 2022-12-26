using Flitter.Api.Data;
using Flitter.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flitter.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly FlitterDbContext _context;

        public CommentsController(FlitterDbContext context)
        {
            _context = context;
        }

        [HttpGet("posts/{postId}")]
        public async Task<IActionResult> Get(int postId)
        {
            var comments = await _context.Comments
                .Where(c => c.PostId == postId)
                .ToListAsync();

            return Ok(comments);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();

            return Ok(comment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody] Comment comment)
        {
            var existingComment = await _context.Comments
                .FirstOrDefaultAsync(x => x.Id == comment.Id);

            if (existingComment == null)
            {
                return NotFound(comment.Id);
            }

            existingComment.Text = existingComment.Text;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existingComment = await _context.Comments
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existingComment == null)
            {
                return NotFound(id);
            }

            _context.Comments.Remove(existingComment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
