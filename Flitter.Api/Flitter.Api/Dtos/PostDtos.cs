using Flitter.Api.Models;

namespace Flitter.Api.Dtos
{
    public class PostCreate
    {
        public string Text { get; set; }
        public PollCreate Poll { get; set; }
    }

    public class PostUpdate
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public PollCreate Poll { get; set; }
    }

    public class PostView
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int? OriginalPostId { get; set; }
        public Post OriginalPost { get; set; }
        public string UserId { get; set; }
        public int LikesCount { get; set; }
        public bool Liked { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserView User { get; set; }
        public List<Like> Likes { get; set; }
        public PollView Poll { get; set; }
    }

    public class LikeCreate
    {
        public int PostId { get; set; }
    }
}
