using AutoMapper;
using Flitter.Api.Data;
using Flitter.Api.Data.Caching;
using Flitter.Api.Dtos;
using Flitter.Api.Models;
using Flitter.Api.Models.AuthDb;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flitter.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly FlitterDbContext _context;
        private readonly KeycloakContext _keycloakContext;
        private readonly IPostsCaching _postsCaching;
        private readonly IMapper _mapper;

        public PostsController(
            FlitterDbContext context,
            KeycloakContext keycloakContext,
            IPostsCaching postsCaching,
            IMapper mapper)
        {
            _context = context;
            _keycloakContext = keycloakContext;
            _postsCaching = postsCaching;
            _mapper = mapper;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string query)
        {
            var userId = Helpers.Helper.GetUserId(HttpContext.User.Identity);
            var postDocs = await _postsCaching.Search(query);
            var mappedPostDocs = _mapper.Map<List<PostView>>(postDocs);

            if (postDocs.Count == 0)
            {
                var posts = await _context.Posts
                    .Include(p => p.Poll).ThenInclude(p => p.Options)
                    .Where(p => p.Text.Contains(query))
                    .OrderByDescending(x => x.Id)
                    .ToListAsync();
                var mappedPosts = _mapper.Map<List<PostView>>(posts);

                await AddUsersToPosts(mappedPosts);
                await AddLikedToPosts(mappedPosts, userId);
                await AddPollToPosts(mappedPosts, userId);

                return Ok(mappedPosts);
            }

            await AddUsersToPosts(mappedPostDocs);
            await AddLikedToPosts(mappedPostDocs, userId);
            await AddPollToPosts(mappedPostDocs, userId);

            return Ok(mappedPostDocs);
        }

        [HttpGet("users/{userId}")]
        public async Task<IActionResult> GetByUserId(string userId)
        {
            var currentUserId = Helpers.Helper.GetUserId(HttpContext.User.Identity);
            var postDocs = await _postsCaching.GetByUserId(userId);
            var mappedPostDocs = _mapper.Map<List<PostView>>(postDocs);

            if (postDocs.Count == 0)
            {
                var posts = await _context.Posts
                    .Include(p => p.Poll).ThenInclude(p => p.Options)
                    .Where(p => p.UserId == userId)
                    .OrderByDescending(x => x.Id)
                    .ToListAsync();
                var mappedPosts = _mapper.Map<List<PostView>>(posts);

                await AddUsersToPosts(mappedPosts);
                await AddLikedToPosts(mappedPosts, currentUserId);
                await AddPollToPosts(mappedPosts, currentUserId);

                return Ok(mappedPosts);
            }

            await AddUsersToPosts(mappedPostDocs);
            await AddLikedToPosts(mappedPostDocs, currentUserId);
            await AddPollToPosts(mappedPostDocs, currentUserId);

            return Ok(mappedPostDocs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var userId = Helpers.Helper.GetUserId(HttpContext.User.Identity);
            var postDoc = await _postsCaching.GetById(id);

            if (postDoc == null)
            {
                var post = await _context.Posts.FirstOrDefaultAsync(x => x.Id == id);

                if (post == null)
                {
                    return NotFound(id);
                }

                var mappedPost = _mapper.Map<PostView>(post);

                await AddUsersToPosts(new List<PostView> { mappedPost });
                await AddLikedToPosts(new List<PostView> { mappedPost }, userId);
                await AddPollToPosts(new List<PostView> { mappedPost }, userId);

                return Ok(mappedPost);
            }

            var mappedPostDoc = _mapper.Map<PostView>(postDoc);

            await AddUsersToPosts(new List<PostView> { mappedPostDoc });
            await AddLikedToPosts(new List<PostView> { mappedPostDoc }, userId);
            await AddPollToPosts(new List<PostView> { mappedPostDoc }, userId);

            return Ok(mappedPostDoc);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PostCreate postCreate)
        {
            var userId = Helpers.Helper.GetUserId(HttpContext.User.Identity);
            var now = DateTime.UtcNow;

            var post = new Post
            {
                Text = postCreate.Text?.Trim(),
                UserId = userId,
                CreatedAt = now,
                CreatedBy = userId,
            };

            if (postCreate.Poll != null)
            {
                CreatePoll(postCreate.Poll, post);
            }

            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();

            var postDoc = _mapper.Map<PostDocument>(post);
            await _postsCaching.AddAsync(postDoc);

            var mappedPost = _mapper.Map<PostView>(post);
            await AddUsersToPosts(new List<PostView> { mappedPost });

            return Ok(mappedPost);
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] PostUpdate postUpdate)
        {
            var userId = Helpers.Helper.GetUserId(HttpContext.User.Identity);
            var post = await _context.Posts
                .Include(x => x.Poll).ThenInclude(x => x.Options)
                .FirstOrDefaultAsync(x => x.Id == postUpdate.Id && x.UserId == userId);

            if (post == null)
            {
                return NotFound(postUpdate.Id);
            }

            post.Text = postUpdate.Text?.Trim();

            if (postUpdate.Poll != null)
            {
                if (post.Poll != null)
                {
                    UpdatePoll(postUpdate.Poll, post);
                }
                else
                {
                    CreatePoll(postUpdate.Poll, post);
                }
            }
            else
            {
                post.Poll = null;
            }

            await _context.SaveChangesAsync();

            var postDoc = _mapper.Map<PostDocument>(post);
            await _postsCaching.UpdateAsync(postDoc);

            var mappedPost = _mapper.Map<PostView>(post);
            await AddPollToPosts(new List<PostView> { mappedPost }, userId);

            return Ok(mappedPost);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = Helpers.Helper.GetUserId(HttpContext.User.Identity);
            var existingPost = await _context.Posts.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (existingPost == null)
            {
                return NotFound(id);
            }

            _context.Posts.Remove(existingPost);
            await _context.SaveChangesAsync();

            await _postsCaching.DeteleAsync(id);

            return NoContent();
        }

        [Authorize]
        [HttpPost("polls/vote")]
        public async Task<IActionResult> Vote([FromBody] OptionVoteCreate optionVoteCreate)
        {
            var userId = Helpers.Helper.GetUserId(HttpContext.User.Identity);
            var now = DateTime.UtcNow;

            var post = await _context.Posts
                .Include(x => x.Poll).ThenInclude(x => x.Options)
                .FirstOrDefaultAsync(x => x.Id == optionVoteCreate.PostId);

            if (post == null || post.Poll == null)
            {
                return NotFound(optionVoteCreate.PostId);
            }

            var option = post.Poll.Options.FirstOrDefault(x => x.Id == optionVoteCreate.OptionId);

            if (option == null)
            {
                return NotFound(optionVoteCreate.OptionId);
            }

            option.VotesCount++;

            if (await _context.OptionVotes.AnyAsync(x => x.OptionId == optionVoteCreate.OptionId
                && x.UserId == userId))
            {
                return BadRequest("Can vote only once");
            }

            var optionVote = new OptionVote
            {
                OptionId = optionVoteCreate.OptionId,
                PollId = optionVoteCreate.PollId,
                UserId = userId,
                CreatedAt = now,
                CreatedBy = userId,
            };

            await _context.OptionVotes.AddAsync(optionVote);
            await _context.SaveChangesAsync();

            var mappedPost = _mapper.Map<PostView>(post);
            await AddPollToPosts(new List<PostView> { mappedPost }, userId);

            return Ok(mappedPost.Poll.Options);
        }

        private async Task AddUsersToPosts(List<PostView> posts)
        {
            var userIds = posts.Select(p => p.UserId);
            var users = await _keycloakContext.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();

            foreach (var post in posts)
            {
                var user = users.FirstOrDefault(u => u.Id == post.UserId);

                if (user == null)
                {
                    continue;
                }

                post.User = _mapper.Map<UserView>(user);
            }
        }

        private async Task AddLikedToPosts(List<PostView> posts, string userId)
        {
            var postIds = posts.Select(x => x.Id);
            var likes = await _context.Likes.Where(x => x.UserId == userId && postIds.Contains(x.PostId)).ToListAsync();

            foreach (var post in posts)
            {
                var like = likes.FirstOrDefault(l => l.PostId == post.Id);

                post.Liked = like != null;
            }
        }

        private async Task AddPollToPosts(List<PostView> posts, string userId)
        {
            var postIds = posts.Select(x => x.Id);
            var polls = await _context.Polls
                .Include(x => x.Options.OrderBy(x => x.Id))
                .Where(x => postIds.Contains(x.PostId))
                .ToListAsync();
            var pollIds = polls.Select(x => x.Id);
            var optionVotes = await _context.OptionVotes
                .Where(x => pollIds.Contains(x.PollId))
                .ToListAsync();

            foreach (var post in posts)
            {
                var poll = polls.FirstOrDefault(l => l.PostId == post.Id);
                if (poll == null) { continue; }

                post.Poll = _mapper.Map<PollView>(poll);

                var optionVote = optionVotes.FirstOrDefault(x => x.PollId == poll.Id && x.UserId == userId);
                if (optionVote == null) { continue; }

                var votedOption = post.Poll.Options.FirstOrDefault(x => x.Id == optionVote.OptionId);
                if (votedOption == null) { continue; }

                votedOption.Voted = true;
            }
        }

        private async Task CreatePoll(PollCreate poll, Post post)
        {
            post.Poll = new Poll
            {
                ExpiresAt = poll.ExpiresAt,
                CreatedBy = post.UserId,
                CreatedAt = post.CreatedAt,
                UserId = post.UserId,
            };

            var options = new List<Option>();
            foreach (var optionCreate in poll.Options)
            {
                var option = new Option
                {
                    Text = optionCreate.Text?.Trim(),
                    VotesCount = 0,
                    CreatedBy = post.UserId,
                    CreatedAt = post.CreatedAt
                };
                options.Add(option);
            }

            post.Poll.Options = options;
        }

        private async Task UpdatePoll(PollCreate poll, Post post)
        {
            post.Poll.ExpiresAt = poll.ExpiresAt;

            var options = new List<Option>();
            foreach (var optionCreate in poll.Options)
            {
                if (optionCreate.Id == null)
                {
                    var option = new Option
                    {
                        Text = optionCreate.Text?.Trim(),
                        VotesCount = 0,
                        CreatedBy = post.UserId,
                        CreatedAt = post.CreatedAt
                    };
                    options.Add(option);
                }
                else
                {
                    var option = post.Poll.Options.FirstOrDefault(x => x.Id == optionCreate.Id);

                    if (option == null) { continue; }

                    option.Text = optionCreate.Text?.Trim();
                    options.Add(option);
                }
            }

            post.Poll.Options = options;
        }
    }
}
