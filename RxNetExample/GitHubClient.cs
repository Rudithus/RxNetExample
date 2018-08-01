namespace RxNetExample
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Refit;

    public class GitHubClient
    {
        public async Task<ClientResult> GetUsers(int since = 0)
        {
            var restClient = RestService.For<IGitHubApi>("https://api.github.com");
            var users = await restClient.GetUsers(since, "token GITHUB_TOKEN");

            var results = users;

            return new ClientResult(this, since, results);
        }
    }

    public class ClientResult : List<GitHubUser>
    {
        private readonly GitHubClient _client;
        private int _since;

        public ClientResult(GitHubClient client, int since, IEnumerable<GitHubUser> items) : base(items)
        {
            _client = client;
            _since = since;
        }

        public async Task<ClientResult> More()
        {
            _since += this.Count;
            return await _client.GetUsers(_since);
        }
    }
}