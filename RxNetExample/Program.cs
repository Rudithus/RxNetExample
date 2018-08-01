namespace RxNetExample
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Threading.Tasks;

    public class Program
    {
        private static event EventHandler<int> SuggestEvent;
        private static event EventHandler RefreshEvent;

        private static readonly IObservable<Task<IEnumerable<GitHubUser>>> SuggestionStream;

        private static readonly GitHubClient GitHubClient = new GitHubClient();
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

    public static class Extensions
    {
        public static void Dump<T>(this IObservable<T> source, string name)
        {
            source.Subscribe(
                value => Console.WriteLine($"{name} --> {value}"),
                ex => Console.WriteLine($"{name} failed --> {ex.Message}"),
                () => Console.WriteLine($"{name} completed"));
        }

        public static void Dump<T, TProp>(this IObservable<IEnumerable<T>> source, Func<T, TProp> selector, string name)
        {
            source
                .Select(r => r.Select(selector))
                .Subscribe(
                    value =>
                    {
                        foreach (var prop in value)
                        {
                            Console.WriteLine($"{prop} --> {name}");
                        }
                    },
                    ex => Console.WriteLine($"{name} failed --> {ex.Message}"),
                    () => Console.WriteLine($"{name} completed"));
        }

        public static void Dump<T, TProp>(this IObservable<Task<IEnumerable<T>>> source, Func<T, TProp> selector, string name)
        {
            source
                .Select(async r => (await r).Select(selector))
                .Subscribe(
                   async value =>
                   {
                       foreach (var prop in await value)
                       {
                           Console.WriteLine($"{prop} --> {name}");
                       }
                   },
                    ex => Console.WriteLine($"{name} failed --> {ex.Message}"),
                    () => Console.WriteLine($"{name} completed"));
        }


        public static IEnumerable<T> TakeRandom<T>(this ICollection<T> collection, int count)
        {
            if (count > collection.Count - 1)
            {
                throw new ArgumentException($"{nameof(count)} cannot be higher than collection size");
            }

            var hashSet = new HashSet<int>(count);
            while (hashSet.Count != count)
            {
                var index = new Random().Next(collection.Count);
                if (hashSet.Add(index))
                {
                    yield return collection.ElementAt(index);
                }
            }
        }
    }
}
