using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace RxNetExample
{
    public static class Extensions
    {
        /// <summary>
        /// Helper extension to write user results to the console.        
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProp"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <param name="name"></param>
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

        /// <summary>
        /// Returns items from random indexes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="count"></param>
        /// <returns></returns>
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