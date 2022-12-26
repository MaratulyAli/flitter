using System.ComponentModel.DataAnnotations;

namespace Flitter.Api.Models
{
    public class Post : BaseModel
    {
        [Required]
        [MaxLength(10000)]
        public string Text { get; set; }
        public int? OriginalPostId { get; set; }
        public Post OriginalPost { get; set; }
        public string UserId { get; set; }
        public int LikesCount { get; set; }
        public List<Image> Images { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Like> Likes { get; set; }
        public Poll Poll { get; set; }
    }
}
