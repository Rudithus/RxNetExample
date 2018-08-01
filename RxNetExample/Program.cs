using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace RxNetExample
{    
    public class Program
    {
        private static event EventHandler<int> SuggestEvent;
        private static event EventHandler RefreshEvent;

        private static readonly IObservable<Task<IEnumerable<GitHubUser>>> SuggestionStream;

        private static readonly GitHubClient GitHubClient = new GitHubClient();

        /// <summary>
        /// At startup, the app querries a random list of users and returns 3 randomly picked user from that list.
        /// </summary>
        static Program()
        {            
            var usersStream = Observable
                .FromEventPattern(h => RefreshEvent += h, h => RefreshEvent -= h)
                .StartWith(new EventPattern<object>(null, null))
                .Select(async observer => await GitHubClient.GetUsers(new Random().Next(10) * 500));

            SuggestionStream = Observable
                .FromEventPattern<int>(h => SuggestEvent += h, h => SuggestEvent -= h)
                .StartWith(new EventPattern<int>(null, 3))
                .CombineLatest(usersStream, async (e, o) => (await o).TakeRandom(e.EventArgs));
        }
        /// <summary>
        /// Entry point for the console app.               
        /// 
        /// There are 3 available commands.
        /// If you enter a number, the app returns that many users from its initial users cache.
        /// If you enter "exit", the app quits.
        /// If you enter anything else, the app refreshes its cache and returns 3 random users from the refreshed cache.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            SuggestionStream.Dump(d => $"{d.Id} - {d.Login}", "");

            while (true)
            {
                var command = Console.ReadLine();
                if (command == "exit")
                    Environment.Exit(0);
                if (int.TryParse(command, out var count))
                {
                    SuggestEvent?.Invoke("app", count);
                }
                else
                {
                    RefreshEvent?.Invoke(null, null);
                }
            }
        }
    }  
}
