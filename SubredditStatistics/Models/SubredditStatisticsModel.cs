using Microsoft.AspNetCore.Mvc.RazorPages;
using Reddit.Things;

namespace SubredditStatistics.Models
{
    public class SubredditStatisticsModel : PageModel
    {
        public string SubredditName { get; set; }
        public IOrderedEnumerable<Post> PostsWithMostUpVotes { get; set; }
        public IOrderedEnumerable<KeyValuePair<string, int>> UsersWithMostPosts { get; set; }

        public SubredditStatisticsModel(string subredditName)
        {
            this.SubredditName = subredditName;
        }
    }
}
