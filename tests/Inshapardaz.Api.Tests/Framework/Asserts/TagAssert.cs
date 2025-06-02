using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using System.Net.Http;

namespace Inshapardaz.Api.Tests.Framework.Asserts
{
    public class TagAssert
    {
        private HttpResponseMessage _response;
        private TagView _tag;
        private int _libraryId;

        private readonly ITagTestRepository _tagRepository;

        public TagAssert(ITagTestRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public TagAssert ForView(TagView view)
        {
            _tag = view;
            return this;
        }

        public TagAssert ForResponse(HttpResponseMessage response)
        {
            _response = response;
            _tag = response.GetContent<TagView>().Result;
            return this;
        }

        public TagAssert ForLibrary(int libraryId)
        {
            _libraryId = libraryId;
            return this;
        }

        public TagAssert ShouldNotHaveDeletedTag(int tagId)
        {
            var tag = _tagRepository.GetTagById(_libraryId, tagId);
            tag.Should().NotBeNull();
            return this;
        }

        public TagAssert ShouldHaveDeletedTag(int tagId)
        {
            var tag = _tagRepository.GetTagById(_libraryId, tagId);
            tag.Should().BeNull();
            return this;
        }

        public TagAssert ShouldHaveCreatedTag()
        {
            var tag = _tagRepository.GetTagById(_libraryId, _tag.Id);
            tag.Should().NotBeNull();
            return this;
        }

        public TagAssert ShouldBeSameAs(TagDto dto)
        {
            _tag.Id.Should().Be(dto.Id);
            _tag.Name.Should().Be(dto.Name);
            return this;
        }

        public TagAssert ShouldBeSameAs(TagView expected, bool matchLinks = false)
        {
            _tag.Id.Should().Be(expected.Id);
            _tag.Name.Should().Be(expected.Name);
            return this;
        }
    }
}
