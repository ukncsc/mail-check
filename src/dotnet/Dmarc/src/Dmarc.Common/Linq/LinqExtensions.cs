using System;
using System.Collections.Generic;
using System.Linq;

namespace Dmarc.Common.Linq
{
    public static class LinqExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> action)
        {
            foreach (T item in sequence) action(item);
        }

        public static void ForEach<T>(this IEnumerable<T> sequence, Action<T, int> action)
        {
            var i = 0;
            foreach (T item in sequence) action(item, i++);
        }

        //Credited to Sergey Berezovskiy : http://stackoverflow.com/questions/13731796/create-batches-in-linq
        public static IEnumerable<IEnumerable<TSource>> Batch<TSource>(
                  this IEnumerable<TSource> source, int size)
        {
            TSource[] bucket = null;
            var count = 0;

            foreach (var item in source)
            {
                if (bucket == null)
                    bucket = new TSource[size];

                bucket[count++] = item;
                if (count != size)
                    continue;

                yield return bucket;

                bucket = null;
                count = 0;
            }

            if (bucket != null && count > 0)
            {
                yield return bucket.Take(count);
            }
        }
    }
}
