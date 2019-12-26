using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace GithubAPIGet
{
    class Program
    {
        private static string bars = "----------------------------------------------------";
        static async Task Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: GithubAPI.exe [username]");
                return;
            }
            var fetcher = new GithubFetcher();
            try
            {
                var userinfo = await fetcher.ProcessUser(args[0]);
                Console.WriteLine(bars);
                Console.WriteLine(userinfo.ToString());
                Console.WriteLine(bars);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e.Message);
            }
            try
            {
                var repoinfo = await fetcher.ProcessUserRepositories(args[0]);
                Console.WriteLine(bars);
                Console.WriteLine(String.Join('\n'+bars+'\n', repoinfo));
                Console.WriteLine(bars);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
