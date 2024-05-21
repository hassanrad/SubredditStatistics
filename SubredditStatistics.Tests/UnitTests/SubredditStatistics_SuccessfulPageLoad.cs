using SubredditStatistics.Controllers;
using SubredditStatistics.Models;
using SubredditStatistics.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Reddit.Things;

namespace SubredditStatistics.Tests.UnitTests
{
    public class SubredditStatistics_SuccessfulPageLoad : SubredditStatistics_BaseTest
    {
        private readonly string subredditName = "funnysigns";

        [Fact]
        public async Task Page_ReturnsAViewResult_WithAListOfPosts()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<SubredditStatisticsController>>();
            var mockRedditClientService = new Mock<IRedditClientService>();
            mockRedditClientService.Setup(reddit => reddit.GetPostsWithMostUpVotes(subredditName))
                .Returns(GetTestPosts);
            var controller = new SubredditStatisticsController(mockLogger.Object, mockRedditClientService.Object);

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<SubredditStatisticsModel>(
                viewResult.ViewData.Model);
            Assert.Equal(2, model.PostsWithMostUpVotes.Count());
        }

        [Fact]
        public async Task Page_ReturnsAViewResult_WithAListOfUsers()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<SubredditStatisticsController>>();
            var mockRedditClientService = new Mock<IRedditClientService>();
            mockRedditClientService.Setup(reddit => reddit.GetUsersWithMostPosts(subredditName))
                .Returns(GetTestUsers);
            var controller = new SubredditStatisticsController(mockLogger.Object, mockRedditClientService.Object);

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<SubredditStatisticsModel>(
                viewResult.ViewData.Model);
            Assert.Equal(2, model.UsersWithMostPosts.Count());
        }

        private IOrderedEnumerable<Post> GetTestPosts()
        {
            var post1 = new Post();
            post1.Title = "haha funny sign";
            post1.Author = "iamareddituser";
            post1.Ups = 512;
            post1.Permalink = "/r/funnysigns/comments/1cx68ta/haha_funny_sign/";

            var post2 = new Post();
            post1.Title = "haha another funny sign";
            post1.Author = "iamalsoareddituser";
            post1.Ups = 2900;
            post1.Permalink = "/r/funnysigns/comments/1cx5wvx/haha_another_funny_sign/";

            var posts = new List<Post>
            {
                post1, post2
            };
            return posts.OrderByDescending(x => x.Ups);
        }

        private IOrderedEnumerable<KeyValuePair<string, int>> GetTestUsers()
        {
            var users = new Dictionary<string, int> {
                { "iamareddituser", 1 },
                { "iamalsoareddituser", 1 }
            };

            return users.OrderByDescending(x => x.Value);
        }
    }
}
