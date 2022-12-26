using System.ComponentModel.DataAnnotations;

namespace Flitter.Api.Models
{
    public class Comment : BaseModel
    {
        [Required]
        [MaxLength(10000)]
        public string Text { get; set; }

        public int PostId { get; set; }
        public Post Post { get; set; }
        public string UserId { get; set; }
    }
}
