using Bogus;
using System.Collections.Generic;

namespace Inshapardaz.Functions.Tests.Helpers
{
    public static class Random
    {
        public static int Number => new Faker().Random.Number();

        public static string Text => new Faker().Random.String();

        public static T PickRandom<T>(this IEnumerable<T> source) =>
            new Faker().PickRandom<T>(source);
    }
}
