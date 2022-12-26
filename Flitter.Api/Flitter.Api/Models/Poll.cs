using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Flitter.Api.Models
{
    public class Poll : BaseModel
    {
        public DateTime? ExpiresAt { get; set; }
        public List<Option> Options { get; set; }
        public int PostId { get; set; }
        [JsonIgnore]
        public Post Post { get; set; }
        public string UserId { get; set; }
    }

    public class Option : BaseModel
    {
        [Required]
        [MaxLength(100)]
        public string Text { get; set; }

        public int VotesCount { get; set; }
        public int PollId { get; set; }
        [JsonIgnore]
        public Poll Poll { get; set; }
    }

    public class OptionVote : BaseModel
    {
        public int OptionId { get; set; }
        public int PollId { get; set; }
        public string UserId { get; set; }
    }
}
