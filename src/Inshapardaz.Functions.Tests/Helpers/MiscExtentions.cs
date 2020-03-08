using System;
using System.Collections.Generic;
using System.Text;

namespace Inshapardaz.Functions.Tests.Helpers
{
    public static class MiscExtentions
    {
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }
        }
    }
}
