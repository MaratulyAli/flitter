namespace Flitter.Api.Models
{
    public class Follow : BaseModel
    {
        public string UserToId { get; set; }
        public string UserFromId { get; set; }
    }
}
