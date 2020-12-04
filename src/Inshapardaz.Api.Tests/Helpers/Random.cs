using Bogus;
using Inshapardaz.Domain.Models;
using System;
using System.Collections.Generic;

namespace Inshapardaz.Api.Tests.Helpers
{
    public static class Random
    {
        public static int Number => new Faker().Random.Number(1);

        public static string Text => new Faker().Random.Words(3);

        public static bool Bool => new Faker().Random.Bool();

        public static string Name => new Faker().Name.FullName();
        public static byte[] Bytes => new Faker().Random.Bytes(10);

        public static string BlobUrl => new Faker().Internet.Url();

        public static T PickRandom<T>(this IEnumerable<T> source) =>
            new Faker().PickRandom<T>(source);

        public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count) =>
            new Faker().PickRandom<T>(source, count);

        internal static string Locale => PickRandom(Langs);

        internal static string Words(int count) => new Faker().Random.Words(count);

        internal static string MimeType =>
            new Faker().PickRandom(new[] { MimeTypes.Markdown, MimeTypes.Pdf, MimeTypes.Epub, MimeTypes.Html, MimeTypes.Json, MimeTypes.MsWord, MimeTypes.Text });

        private static string[] Langs = new[] { "en", "ur", "hi", "pn", "pr", "fr", "ar", "pr", "tr" };

        private static int LangPos = 0;

        internal static string NextLocale()
        {
            if (LangPos > Langs.Length - 1)
            {
                LangPos = 0;
            }

            return Langs[LangPos++];
        }

        internal static DateTime Date => new Faker().Date.Past();
    }
}
