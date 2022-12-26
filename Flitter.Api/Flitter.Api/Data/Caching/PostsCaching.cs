using Nest;

namespace Flitter.Api.Data.Caching
{
    public class PostsCaching : IPostsCaching
    {
        private readonly ElasticClient _client;
        private readonly string _postsIdx;

        public PostsCaching(IConfiguration configuration)
        {
            _postsIdx = configuration.GetSection("Elastic:PostsIndex").Value;

            var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
                .DefaultIndex(_postsIdx)
                .DefaultMappingFor<PostDocument>(i => i.IndexName(_postsIdx));
            _client = new ElasticClient(settings);
        }

        public async Task<List<PostDocument>> GetByUserId(string userId)
        {
            var response = await _client.SearchAsync<PostDocument>(sd => sd
                .Query(d => d
                    .Term(qd => qd
                        .Field(m => m.Text)
                            .Value($"{userId}"))));

            return response.Documents.OrderByDescending(x => x.Id).ToList();
        }

        public async Task<PostDocument> GetById(int id)
        {
            var result = await _client.GetAsync<PostDocument>(id);

            return result.Source;
        }

        public async Task<List<PostDocument>> Search(string query)
        {
            var response = await _client.SearchAsync<PostDocument>(sd => sd
                .Query(d => d
                    .MatchPhrasePrefix(qd => qd
                        .Field(m => m.Text)
                            .Analyzer("standard")
                            .Query($"{query}"))));

            return response.Documents.OrderByDescending(x => x.Id).ToList();
        }

        public async Task AddAsync(PostDocument post)
        {
            var response = await _client.IndexDocumentAsync(post);

            if (!response.IsValid)
            {
                Console.WriteLine($"Failed to index document {response.Id}: {response.ServerError}");
            }
        }

        public async Task UpdateAsync(PostDocument post)
        {
            await AddAsync(post);
        }

        public async Task DeteleAsync(int postId)
        {
            var response = await _client.DeleteAsync<PostDocument>(postId);

            if (!response.IsValid)
            {
                Console.WriteLine($"Failed to delete document {response.Id}: {response.ServerError}");
            }
        }

        public async Task ReIndex(List<PostDocument> posts)
        {
            var response = await _client.IndexManyAsync(posts);

            if (response.Errors)
            {
                foreach (var itemWithError in response.ItemsWithErrors)
                {
                    Console.WriteLine($"Failed to index document {itemWithError.Id}: {itemWithError.Error}");
                }
            }
        }

        public async Task CreateIndex()
        {
            await _client.Indices.CreateAsync(_postsIdx, c => c
                .Map<PostDocument>(m => m.AutoMap()));
        }
    }
}
