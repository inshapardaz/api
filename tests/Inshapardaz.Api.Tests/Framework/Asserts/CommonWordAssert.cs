using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using System.Net.Http;
using Inshapardaz.Api.Views.Tools;

namespace Inshapardaz.Api.Tests.Framework.Asserts
{
    internal class CommonWordAssert
    {
        private CommonWordView _commonWord;
        public HttpResponseMessage _response;
        public CommonWordView View => _commonWord;

        private readonly ICommonWordsTestRepository _commonWordsTestRepository;

        public CommonWordAssert(ICommonWordsTestRepository commonWordsTestRepository)
        {
            _commonWordsTestRepository = commonWordsTestRepository;
        }


        public CommonWordAssert ForResponse(HttpResponseMessage response)
        {
            _response = response;
            _commonWord = response.GetContent<CommonWordView>().Result;
            return this;
        }

        public CommonWordAssert ForView(CommonWordView view)
        {
            _commonWord = view;
            return this;
        }
        
        internal CommonWordAssert ShouldBeSameAs(CommonWordDto expected)
        {
            _commonWord.Should().NotBeNull();
            _commonWord.Language.Should().Be(expected.Language);
            _commonWord.Word.Should().Be(expected.Word);
            return this;
        }

        internal CommonWordAssert ShouldBeSameAs(CommonWordView expected)
        {
            _commonWord.Should().NotBeNull();
            _commonWord.Language.Should().Be(expected.Language);
            _commonWord.Word.Should().Be(expected.Word);
            return this;
        }

        internal CommonWordAssert ShouldHaveDeletedCommonWord(long wordId)
        {
            var correction = _commonWordsTestRepository.GetCommonWordById(wordId);
            correction.Should().BeNull();
            return this;
        }

        internal CommonWordAssert ShouldNotHaveDeletedWord(long wordId)
        {
            var correction = _commonWordsTestRepository.GetCommonWordById(wordId);
            correction.Should().NotBeNull();
            return this;
        }

        public CommonWordAssert ShouldNotHaveDeleteLink()
        {
            _commonWord.DeleteLink().Should().BeNull();

            return this;
        }
        
        public CommonWordAssert ShouldHaveSelfLink()
        {
            _commonWord.SelfLink().Should().NotBeNull();

            return this;
        }
        
        public CommonWordAssert ShouldNotHaveUpdateLink()
        {
            _commonWord.UpdateLink().Should().BeNull();

            return this;
        }
        
        public CommonWordAssert ShouldHaveUpdateLink()
        {
            _commonWord.UpdateLink()
                .ShouldBePut()
                .EndingWith($"tools/{_commonWord.Language}/words/{_commonWord.Id}");

            return this;
        }
        
        public CommonWordAssert ShouldHaveDeleteLink()
        {
            _commonWord.DeleteLink()
                .ShouldBeDelete()
                .EndingWith($"tools/{_commonWord.Language}/words/{_commonWord.Id}");

            return this;
        }

        public CommonWordAssert ShouldHaveSavedWord()
        {
            var dbWord = _commonWordsTestRepository.GetCommonWordById(_commonWord.Id);
            dbWord.Should().NotBeNull();
            _commonWord.Language.Should().Be(dbWord.Language);
            _commonWord.Word.Should().Be(dbWord.Word);
            return this;
        }

        public CommonWordAssert ShouldMatchSavedWord(CommonWordDto word)
        {
            var dbWord = _commonWordsTestRepository.GetCommonWordById(word.Id);
            dbWord.Should().NotBeNull();
            word.Language.Should().Be(dbWord.Language);
            word.Word.Should().Be(dbWord.Word);
            return this;
        }

        public CommonWordAssert ShouldHaveCorrectWordReturned(CommonWordDto word)
        {
            _commonWord.Language.Should().Be(word.Language);
            _commonWord.Word.Should().Be(word.Word);
            return this;
        }
    }
}
