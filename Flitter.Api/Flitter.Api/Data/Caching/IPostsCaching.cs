using Flitter.Api.Models;

namespace Flitter.Api.Data.Caching
{
    public interface IPostsCaching
    {
        public Task<List<PostDocument>> GetByUserId(string userId);
        public Task<PostDocument> GetById(int id);
        public Task<List<PostDocument>> Search(string query);
        public Task AddAsync(PostDocument post);
        public Task UpdateAsync(PostDocument post);
        public Task DeteleAsync(int postId);
        public Task ReIndex(List<PostDocument> posts);
        public Task CreateIndex();
    }
}
