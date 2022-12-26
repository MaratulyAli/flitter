namespace Flitter.Api.Data.Caching
{
    public class PostDocument
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int? OriginalPostId { get; set; }
        public string UserId { get; set; }
        public int LikesCount { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
