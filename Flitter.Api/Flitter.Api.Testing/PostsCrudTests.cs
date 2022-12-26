using Flitter.Api.Data.Caching;
using Flitter.Api.Testing.Helpers;
using Microsoft.Extensions.Configuration;

namespace Flitter.Api.Testing
{
    // https://github.com/dotnet/AspNetCore.Docs.Samples/blob/main/fundamentals/minimal-apis/samples/MinApiTestsSample/UnitTests/TodoMoqTests.cs

    public class PostsCrudTests
    {
        [Fact]
        public async void GetPostReturnsDefaultIfNotExists()
        {
            // Arrange
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            // TODO: use Mock
            var index = new PostsCaching(configuration);
            await index.CreateIndex();

            // Act
            var result = await index.GetById(1);

            // Assert
            Assert.Equal(null, result);
        }

        [Fact]
        public async void GetAllReturnsPostsFromIndex()
        {

        }

        [Fact]
        public async void GetAllPostsByUserIdReturnsUserPosts()
        {

        }

        [Fact]
        public async void GetPostByIdReturnsPostFromIndex()
        {

        }

        [Fact]
        public async void CreatePostCreatesPostInIndex()
        {

        }

        [Fact]
        public async void UpdatePostUpdatesPostInIndex()
        {

        }

        [Fact]
        public async void DeletePostDeletesPostFromIndex()
        {

        }
    }
}