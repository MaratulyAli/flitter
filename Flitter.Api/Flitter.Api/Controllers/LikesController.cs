using AutoMapper;
using Flitter.Api.Data;
using Flitter.Api.Data.Caching;
using Flitter.Api.Dtos;
using Flitter.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flitter.Api.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class LikesController : ControllerBase
    {
        private readonly FlitterDbContext _context;
        private readonly IPostsCaching _postsCaching;
        private readonly IMapper _mapper;

        public LikesController(
            FlitterDbContext context,
            IPostsCaching postsCaching,
            IMapper mapper)
        {
            _context = context;
            _postsCaching = postsCaching;
            _mapper = mapper;
        }

        [HttpGet("{postId}/[controller]")]
        public async Task<IActionResult> Get(int postId)
        {
            var post = await _context.Posts
                .Include(x => x.Likes)
                .FirstOrDefaultAsync(x => x.Id == postId);

            if (post == null)
                return NotFound(postId);

            return Ok(post.Likes);
        }

        [Authorize]
        [HttpPost("[controller]")]
        public async Task<IActionResult> Post([FromBody] LikeCreate likeCreate)
        {
            var userId = Helpers.Helper.GetUserId(HttpContext.User.Identity);
            var now = DateTime.UtcNow;
            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(x => x.PostId == likeCreate.PostId && x.UserId == userId);

            var post = new Post();
            try
            {
                post = await GetPost(likeCreate.PostId);
            }
            catch (PostNotFoundException)
            {
                return NotFound(likeCreate.PostId);
            }

            if (existingLike == null)
            {
                var like = new Like
                {
                    PostId = likeCreate.PostId,
                    UserId = userId,
                    CreatedAt = now,
                    CreatedBy = userId
                };

                await _context.Likes.AddAsync(like);
                await _context.SaveChangesAsync();

                post.LikesCount++;
            }
            else
            {
                _context.Likes.Remove(existingLike);
                await _context.SaveChangesAsync();

                post.LikesCount--;
            }

            await UpdatePost(post);

            return NoContent();
        }

        private async Task<Post> GetPost(int postId)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(x => x.Id == postId);

            if (post == null)
            {
                throw new PostNotFoundException(postId);
            }

            return post;
        }

        private async Task UpdatePost(Post post)
        {
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();

            var postDoc = _mapper.Map<PostDocument>(post);
            await _postsCaching.UpdateAsync(postDoc);
        }
    }

    class PostNotFoundException : Exception
    {
        public int PostId { get; set; }

        public PostNotFoundException(int postId)
        {
            PostId = postId;
        }
    }
}
