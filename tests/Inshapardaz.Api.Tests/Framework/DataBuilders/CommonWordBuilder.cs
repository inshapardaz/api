using AutoFixture;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using System.Collections.Generic;
using System.Linq;
using Inshapardaz.Api.Views.Tools;

namespace Inshapardaz.Api.Tests.Framework.DataBuilders
{
    public class CommonWordBuilder
    {
        private List<CommonWordDto> _commonWords = new();

        private readonly ICommonWordsTestRepository _commonWordsRepository;
        private string _language;
        private string _pattern = "Word_";

        public CommonWordBuilder(ICommonWordsTestRepository commonWordsRepository)
        {
            _commonWordsRepository = commonWordsRepository;
        }

        public IEnumerable<CommonWordDto> Words => _commonWords;

        public CommonWordBuilder WithLanguage(string language)
        {
            _language = language;
            return this;
        }
        
        public CommonWordBuilder WithPattern(string pattern)
        {
            _pattern = pattern;
            return this;
        }
        
        public CommonWordDto Build() => Build(1).First();

        public IEnumerable<CommonWordDto> Build(int count)
        {
            var fixture = new Fixture();
            var words = fixture
                .Build<CommonWordDto>()
                .With(a => a.Word, () => fixture.Create(_pattern))
                .With(x => x.Language, () => _language ?? RandomData.Locale)
                .CreateMany(count);

            _commonWords.AddRange(words);
            _commonWordsRepository.AddCommonWords(words);

            return words;
        }


        public CommonWordView BuildCommonWordView()
        {
            var fixture = new Fixture();
            return fixture
                .Build<CommonWordView>()
                .With(a => a.Word, () => fixture.Create(_pattern))
                .With(x => x.Language, () => _language ?? RandomData.Locale)
                .Create();
        }

        public void Cleanup()
        {
            _commonWordsRepository.DeleteCommonWords(_commonWords);
        }

    }
}
