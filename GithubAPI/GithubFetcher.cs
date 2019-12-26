using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;

namespace GithubAPIGet
{
    public class GithubFetcher
    {
        private readonly HttpClient client = new HttpClient();

        public GithubFetcher()
        {
            // set up request headers
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add
                (new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
        }

        public async Task<GithubUser> ProcessUser(string username)
        {
            // make request
            var stringRes = await client.GetStreamAsync($"https://api.github.com/users/{username}");
            // serialize into object
            var serializer = new DataContractJsonSerializer(typeof(GithubUser));
            return serializer.ReadObject(stringRes) as GithubUser;
        }

        public async Task<List<GithubRepository>> ProcessUserRepositories(string username)
        {
            var stringRes = await client.GetStreamAsync($"https://api.github.com/users/{username}/repos");
            var serializer = new DataContractJsonSerializer(typeof(List<GithubRepository>));
            return serializer.ReadObject(stringRes) as List<GithubRepository>;
        }
    }
}
