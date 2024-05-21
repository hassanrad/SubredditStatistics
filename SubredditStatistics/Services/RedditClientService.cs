using Reddit;
using Reddit.Controllers.EventArgs;

namespace SubredditStatistics.Services
{
    public sealed class RedditClientService : IRedditClientService
    {
        private readonly RedditClient? _redditClient;
        private readonly bool _prePopulateWithData;
        private Dictionary<string, Reddit.Things.Post> postsSinceAppStart;

        public RedditClientService(IConfiguration config)
        {
            _prePopulateWithData = config["RedditClient:PrePopulateWithData"] == "true";
            // Create a new Reddit.NET instance
            _redditClient = new RedditClient(appId: config["RedditClient:AppID"], 
                                            refreshToken: config["RedditClient:RefreshToken"], 
                                            appSecret: config["RedditClient:AppSecret"],
                                            accessToken: config["RedditClient:AccessToken"]);
            postsSinceAppStart = new Dictionary<string, Reddit.Things.Post>();
        }

        public bool StartTrackingSubreddit(string subredditName)
        {
            // Get controller for the selected subreddit
            Reddit.Controllers.Subreddit subreddit = _redditClient.Subreddit(subredditName);

            // Asynchronously monitor subreddit for new posts and add event handler
            if (!subreddit.Posts.NewPostsIsMonitored())
            {
                if (_prePopulateWithData)
                {
                    // Pre-populate collection with the initial 100 newest posts on the subreddit
                    subreddit.Posts.GetNew().ForEach(post => postsSinceAppStart[post.Fullname] = new Reddit.Things.Post(post));
                }
                else 
                {
                    subreddit.Posts.GetNew();
                }

                subreddit.Posts.MonitorNew();
                subreddit.Posts.NewUpdated += C_NewPostsUpdated;
                return true;
            }

            return false;
        }

        public void StopTrackingSubreddit(string subredditName)
        {
            // Get controller for the selected subreddit
            Reddit.Controllers.Subreddit subreddit = _redditClient.Subreddit(subredditName);

            // Stop monitoring subreddit for new posts and remove event handler
            if (subreddit.Posts.NewPostsIsMonitored())
            {
                subreddit.Posts.MonitorNew();
                subreddit.Posts.NewUpdated -= C_NewPostsUpdated;

                // Remove the collected posts for this subreddit
                List<Reddit.Things.Post> postsToRemove = postsSinceAppStart.Values.Where(post => post.Subreddit == subredditName).ToList();
                foreach (var post in postsToRemove)
                {
                    postsSinceAppStart.Remove(post.Name);
                }
            }
        }

        private void C_NewPostsUpdated(object sender, PostsUpdateEventArgs e)
        {
            // Add any new posts to the collection
            foreach (var post in e.Added)
            {
                postsSinceAppStart[post.Fullname] = new Reddit.Things.Post(post);
            }
        }

        public IOrderedEnumerable<Reddit.Things.Post> GetPostsWithMostUpVotes(string subreddit)
        {
            return postsSinceAppStart.Values.Where(post => post.Subreddit == subreddit).ToList().OrderByDescending(x => x.Ups);
        }

        public IOrderedEnumerable<KeyValuePair<string, int>> GetUsersWithMostPosts(string subreddit)
        {
            Dictionary<string, int> usersDictionary = new Dictionary<string, int>();

            foreach (Reddit.Things.Post post in postsSinceAppStart.Values.Where(post => post.Subreddit == subreddit))
            {
                string user = post.Author;
                int postCount = 0;
                usersDictionary.TryGetValue(user, out postCount);
                usersDictionary[user] = postCount + 1;
            }

            // Sort by descending order of the users' post count
            return usersDictionary.OrderByDescending(x => x.Value);
        }

        public void UpdateCollectedPosts()
        {
            List<Reddit.Controllers.Post> updatedPosts = _redditClient.GetPosts(postsSinceAppStart.Keys.ToList());
            foreach (var post in updatedPosts)
            {
                postsSinceAppStart[post.Fullname] = new Reddit.Things.Post(post);
            }
        }
    }
}
