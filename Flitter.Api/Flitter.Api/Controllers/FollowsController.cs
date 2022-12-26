using Flitter.Api.Data;
using Flitter.Api.Models;
using Flitter.Api.Models.AuthDb;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flitter.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowsController : ControllerBase
    {
        private readonly KeycloakContext _context;

        public FollowsController(KeycloakContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFollowers(string userId)
        {
            var followers = await _context.Follows
                .Where(x => x.UserToId == userId)
                .ToListAsync();

            return Ok(followers);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Follow follow)
        {
            await _context.Follows.AddAsync(follow);
            await _context.SaveChangesAsync();

            return Ok(follow);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string userId)
        {
            var existingFollow = await _context.Follows
                .FirstOrDefaultAsync(x => x.UserToId == userId);

            if (existingFollow == null)
            {
                return NotFound(userId);
            }

            _context.Follows.Remove(existingFollow);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
