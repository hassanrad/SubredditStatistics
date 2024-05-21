using Reddit.Things;

namespace SubredditStatistics.Services
{
    public interface IRedditClientService
    {
        bool StartTrackingSubreddit(string subreddit);
        void StopTrackingSubreddit(string subreddit);
        void UpdateCollectedPosts();
        IOrderedEnumerable<Post> GetPostsWithMostUpVotes(string subreddit);
        IOrderedEnumerable<KeyValuePair<string, int>> GetUsersWithMostPosts(string subreddit);
    }
}
