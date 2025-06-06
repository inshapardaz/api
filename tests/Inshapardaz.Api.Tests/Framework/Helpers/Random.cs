﻿using Bogus;
using Inshapardaz.Domain.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Inshapardaz.Api.Tests.Framework.Helpers
{
    public static class RandomData
    {
        public static int Number => new Faker().Random.Number(1);

        public static string Text => new Faker().Random.Words(3);
        public static string String => new Faker().Random.String2(10);

        public static bool Bool => new Faker().Random.Bool();

        public static string Email => new Faker().Internet.Email();
        public static string Name => new Faker().Name.FullName();
        public static byte[] Bytes => new Faker().Random.Bytes(10);
        public static byte[] ImageBytes
        {
            get
            {
                using (var bmp = new Bitmap(1, 1))
                using (var ms = new MemoryStream())
                {
                    using (var g = Graphics.FromImage(bmp))
                    {
                        g.Clear(Color.AliceBlue);
                    }
                    bmp.Save(ms, ImageFormat.Png);
                    return ms.ToArray();
                }
            }
        }

        public static T PickRandom<T>(this IEnumerable<T> source) =>
            new Faker().PickRandom<T>(source);

        public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count) =>
            new Faker().PickRandom<T>(source, count);
        
        public static T PickRandomExcept<T>(this IEnumerable<T> source, T excluding) =>
            new Faker().PickRandom<T>(source.Where(x => !x.Equals(excluding)).ToList());

        internal static string Locale => PickRandom(Langs);

        internal static string Words(int count) => new Faker().Random.Words(count);

        internal static string MimeType =>
            new Faker().PickRandom(new[] { MimeTypes.Markdown, MimeTypes.Pdf, MimeTypes.Epub, MimeTypes.Html, MimeTypes.Json, MimeTypes.MsWord, MimeTypes.Text });

        internal static IEnumerable<EditingStatus> AssignableEditingStatusList => new[] { EditingStatus.Available, EditingStatus.Typing, EditingStatus.InReview, EditingStatus.Typed };
        internal static IEnumerable<EditingStatus> EditingStatusList => new[] { EditingStatus.Available, EditingStatus.Typing, EditingStatus.InReview, EditingStatus.Typed, EditingStatus.Completed };
        internal static EditingStatus AssignableEditingStatus =>
           new Faker().PickRandom(AssignableEditingStatusList);
        internal static EditingStatus EditingStatus => new Faker().PickRandom(EditingStatusList);
        internal static StatusType StatusType => new Faker().PickRandom<StatusType>();
        internal static int NumberBetween(int v1, int v2) => new Faker().Random.Number(v1, v2);

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

        internal static string FilePath => new Faker().System.FilePath();
        internal static string FileName(string mimetype) => new Faker().System.FileName(mimetype.GetExtentionForMimeType());
    }
}
