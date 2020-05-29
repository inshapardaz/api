using Bogus;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models;
using System;
using System.Collections.Generic;

namespace Inshapardaz.Functions.Tests.Helpers
{
    public static class Random
    {
        public static int Number => new Faker().Random.Number(1);

        public static string Text => new Faker().Random.String();

        public static bool Bool => new Faker().Random.Bool();

        public static string Name => new Faker().Name.FullName();
        public static byte[] Bytes => new Faker().Random.Bytes(10);

        public static string BlobUrl => $"{ConfigurationSettings.BlobRoot}{new Faker().Internet.UrlRootedPath()}";

        public static T PickRandom<T>(this IEnumerable<T> source) =>
            new Faker().PickRandom<T>(source);

        public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count) =>
            new Faker().PickRandom<T>(source, count);

        internal static string Locale => PickRandom(new[] { "en", "ur", "hi", "pn", "pr" });

        internal static object Words(int count) => new Faker().Random.Words(count);

        internal static string MimeType =>
            new Faker().PickRandom(new[] { MimeTypes.Csv, MimeTypes.Jpg, MimeTypes.Json, MimeTypes.Markdown, MimeTypes.Pdf });
    }
}
