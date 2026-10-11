using System.IO.Compression;
using System.Text;

namespace Inshapardaz.Api.Tests.Framework.Helpers
{
    // Small StarDict and DSL dictionaries for upload tests.
    public static class DictionaryFileBuilder
    {
        public static byte[] Dsl(string name, params (string Headword, string Definition)[] entries)
        {
            var text = new StringBuilder();
            text.AppendLine($"#NAME \"{name}\"");
            text.AppendLine("#INDEX_LANGUAGE \"Urdu\"");
            text.AppendLine("#CONTENTS_LANGUAGE \"English\"");
            text.AppendLine();
            foreach (var (headword, definition) in entries)
            {
                text.AppendLine(headword);
                text.AppendLine($"\t[m1][trn]{definition}[/trn][/m]");
            }
            return Encoding.UTF8.GetBytes(text.ToString());
        }

        // A zip holding name.ifo / name.idx / name.dict with plain-text ('m') definitions.
        public static byte[] StarDictZip(string name, params (string Headword, string Definition)[] entries)
        {
            var dict = new MemoryStream();
            var idx = new MemoryStream();
            foreach (var (headword, definition) in entries.OrderBy(e => e.Headword, StringComparer.Ordinal))
            {
                var data = Encoding.UTF8.GetBytes(definition);
                var offset = (uint)dict.Length;
                dict.Write(data);

                idx.Write(Encoding.UTF8.GetBytes(headword));
                idx.WriteByte(0);
                idx.Write(BigEndian(offset));
                idx.Write(BigEndian((uint)data.Length));
            }

            var ifo = $"StarDict's dict ifo file\nversion=2.4.2\nbookname={name}\nwordcount={entries.Length}\nidxfilesize={idx.Length}\nsametypesequence=m\n";

            using var zip = new MemoryStream();
            using (var archive = new ZipArchive(zip, ZipArchiveMode.Create, leaveOpen: true))
            {
                Add(archive, "test/test.ifo", Encoding.UTF8.GetBytes(ifo));
                Add(archive, "test/test.idx", idx.ToArray());
                Add(archive, "test/test.dict", dict.ToArray());
            }
            return zip.ToArray();
        }

        public static IReadOnlyList<string> ZipEntries(byte[] zip)
        {
            using var archive = new ZipArchive(new MemoryStream(zip), ZipArchiveMode.Read);
            return archive.Entries.Select(e => e.FullName).ToList();
        }

        public static string ReadZipEntry(byte[] zip, string entryName)
        {
            using var archive = new ZipArchive(new MemoryStream(zip), ZipArchiveMode.Read);
            using var reader = new StreamReader(archive.GetEntry(entryName)!.Open());
            return reader.ReadToEnd();
        }

        private static byte[] BigEndian(uint value) =>
            [(byte)(value >> 24), (byte)(value >> 16), (byte)(value >> 8), (byte)value];

        private static void Add(ZipArchive archive, string name, byte[] content)
        {
            using var stream = archive.CreateEntry(name).Open();
            stream.Write(content);
        }
    }
}
