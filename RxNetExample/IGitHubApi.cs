using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;

namespace RxNetExample
{
    [Headers("User-Agent: rudithus")]
    public interface IGitHubApi
    {
        [Get("/users?since={since}")]
        Task<ICollection<GitHubUser>> GetUsers(int since, [Header("Authorization")] string authorization);
    }
}