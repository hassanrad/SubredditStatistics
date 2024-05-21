# SubredditStatistics

In order to run this app locally, please first fill out all the Reddit API OAuth values in the project's appsettings.json file as shown below:
  
  "RedditClient": {
    "AppID": "",
    "AppSecret": "",
    "RefreshToken": "",
    "AccessToken": "",
    "PrePopulateWithData": "true"
  },

The default value of 'PrePopulateWithData' property being set to true will, upon app load, show statistics for the 100 newest posts of the subreddit that's hardcoded in SubredditStatisticsController (i.e. "funnysigns"). If the value is changed to false, the initial load of the app won't have any posts listed yet, but the background-service will continue to monitor for any new posts.
