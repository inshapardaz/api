using System;
using System.Collections.Generic;
using System.Linq;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Jobs;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Domain.UnitTests.Jobs
{
    [TestFixture]
    public class ExportDictionaryTest : DatabaseTest
    {
        [Test]
        public void WhenExportingDictionary()
        {
            var dictionary = new Dictionary
                {
                    Name = "some random name",
                    Language = Languages.Chinese,
                    IsPublic = true,
                    UserId = Guid.NewGuid()
                };
            var word1 = new Word
            {
                Title = "کونونور",
                TitleWithMovements = "ننصھدصھد",
                Description = "some description",
                Language = Languages.English,
                Attributes = GrammaticalType.Male | GrammaticalType.Harf,
                Pronunciation = "sd£sfr!!",
                Meaning = new List<Meaning>
                {
                    new Meaning {Value = "sdas da s 1", Example = "some exmpere1"},
                    new Meaning {Value = "sdas da s 2 ", Example = "some exmpere2"},
                    new Meaning {Value = "sdas da s 3", Example = "some exmpere3"},
                },
                Translation = new List<Translation>
                {
                    new Translation {Language = Languages.Arabic, Value = "رونر"},
                    new Translation {Language = Languages.English, Value = "sfsdf"},
                }
            };

            var word2 = new Word
            {
                Title = "sasadad",
                TitleWithMovements = "asdad",
                Description = "some description 2",
                Language = Languages.Bangali,
                Attributes = GrammaticalType.Female | GrammaticalType.Ism,
                Pronunciation = "sd£sfr!!adad",
                Meaning = new List<Meaning>
                {
                    new Meaning {Value = "sdas da s 1", Example = "some exmpere1"},
                    new Meaning {Value = "sdas da s 2 ", Example = "some exmpere2"},
                },
                Translation = new List<Translation>
                {
                    new Translation {Language = Languages.Arabic, Value = "رونر"},
                }
            };

            var word3 = new Word
            {
                Title = "asdas  asd sasasadad",
                TitleWithMovements = "sad asd asd asdad",
                Description = "some description 3",
                Language = Languages.Avestan,
                Attributes = GrammaticalType.FealMurakkab,
                Pronunciation = "!!adad",
            };

            word2.WordRelationSourceWord.Add(new WordRelation { RelatedWord = word3, RelationType = RelationType.Halat });
            dictionary.Word.Add(word1);
            dictionary.Word.Add(word2);
            dictionary.Word.Add(word3);

            DbContext.Dictionary.Add(dictionary);
            DbContext.SaveChanges();

            var logger = new Mock<ILogger<SqliteExport>>();

            var job = new SqliteExport(logger.Object, DbContext);
            job.ExportDictionary(dictionary.Id);

            DbContext.DictionaryDownload.Count().ShouldBe(1);
            DbContext.File.Count().ShouldBe(1);
        }
    }
}
