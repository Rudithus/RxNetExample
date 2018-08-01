namespace RxNetExample
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Refit;

    [Headers("User-Agent: rudithus")]
    public interface IGitHubApi
    {
        [Get("/users/{user}")]
        Task<GitHubUser> GetUser(string user, [Header("Authorization")] string authorization);

        [Get("/users?since={since}")]
        Task<ICollection<GitHubUser>> GetUsers(int since, [Header("Authorization")] string authorization);
    }
}