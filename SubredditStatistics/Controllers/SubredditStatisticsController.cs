using SubredditStatistics.Models;
using SubredditStatistics.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace SubredditStatistics.Controllers
{
    [Route("[controller]")]
    public class SubredditStatisticsController : Controller
    {

        private readonly ILogger<SubredditStatisticsController> _logger;
        private readonly IRedditClientService _redditClientService;

        public SubredditStatisticsController(ILogger<SubredditStatisticsController> logger, IRedditClientService redditClientService)
        {
            _logger = logger;
            _redditClientService = redditClientService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            string subredditName = "funnysigns";
            SubredditStatisticsModel model = new SubredditStatisticsModel(subredditName);
            if (!_redditClientService.StartTrackingSubreddit(subredditName))
            {
                // If the tracking was already started, instead update the collected posts
                _redditClientService.UpdateCollectedPosts();
            }
            model.PostsWithMostUpVotes = _redditClientService.GetPostsWithMostUpVotes(subredditName);
            model.UsersWithMostPosts = _redditClientService.GetUsersWithMostPosts(subredditName);

            return View(model);
        }

        [Route("/error-development")]
        public IActionResult HandleErrorDevelopment(
        [FromServices] IHostEnvironment hostEnvironment)
        {
            if (!hostEnvironment.IsDevelopment())
            {
                return NotFound();
            }

            var exceptionHandlerFeature =
                HttpContext.Features.Get<IExceptionHandlerFeature>()!;

            return Problem(
                detail: exceptionHandlerFeature.Error.StackTrace,
                title: exceptionHandlerFeature.Error.Message);
        }

        [Route("/error")]
        public IActionResult HandleError() =>
            Problem();
    }
}
