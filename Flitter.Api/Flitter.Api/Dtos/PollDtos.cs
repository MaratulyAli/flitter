namespace Flitter.Api.Dtos
{
    public class PollView
    {
        public int Id { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public List<OptionView> Options { get; set; }
        public int PostId { get; set; }
        public string UserId { get; set; }
    }

    public class OptionView
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int VotesCount { get; set; }
        public bool Voted { get; set; }
        public int PollId { get; set; }
    }

    public class PollCreate
    {
        public DateTime? ExpiresAt { get; set; }
        public List<OptionCreate> Options { get; set; }
    }

    public class OptionCreate
    {
        public int? Id { get; set; }
        public string Text { get; set; }
    }

    public class OptionVoteCreate
    {
        public int OptionId { get; set; }
        public int PollId { get; set; }
        public int PostId { get; set; }
    }
}