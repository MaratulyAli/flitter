namespace Flitter.Api.Models
{
    public class Image : BaseModel
    {
        public string Url { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; }
    }
}
