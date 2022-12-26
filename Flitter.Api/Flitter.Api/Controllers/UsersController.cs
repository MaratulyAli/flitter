using Flitter.Api.Data;
using Flitter.Api.Dtos;
using Flitter.Api.Models;
using Flitter.Api.Models.AuthDb;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Flitter.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly KeycloakContext _context;

        public UsersController(KeycloakContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userId = Helpers.Helper.GetUserId(HttpContext.User.Identity);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return NotFound(id);
            }

            return Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody] UserUpdate userUpdate)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userUpdate.Id);

            if (user == null)
            {
                return NotFound(userUpdate.Id);
            }

            user.FirstName = user.FirstName;
            user.LastName = user.LastName;
            user.UserName = user.UserName;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existingPost = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (existingPost == null)
            {
                return NotFound(id);
            }

            _context.Users.Remove(existingPost);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
