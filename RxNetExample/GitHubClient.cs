using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;

namespace RxNetExample
{
    public class GitHubClient
    {
        private const string GitHubToken = "YOUR GITHUB TOKEN HERE";
        public async Task<ICollection<GitHubUser>> GetUsers(int since = 0)
        {
            var restClient = RestService.For<IGitHubApi>("https://api.github.com");
            var users = await restClient.GetUsers(since, $"token {GitHubToken}");

            return users;            
        }
    }
}