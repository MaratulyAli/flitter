namespace Flitter.Api.Models
{
    public class Like : BaseModel
    {
        public int PostId { get; set; }
        public Post Post { get; set; }
        public string UserId { get; set; }
    }
}
