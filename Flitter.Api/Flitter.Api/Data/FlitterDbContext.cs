using Flitter.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Flitter.Api.Data
{
    public class FlitterDbContext : DbContext
    {
        public FlitterDbContext(DbContextOptions<FlitterDbContext> options) : base(options)
        { }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Poll> Polls { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<OptionVote> OptionVotes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //    => optionsBuilder.UseNpgsql(_configuration.GetConnectionString("FlitterDb"));
    }
}
