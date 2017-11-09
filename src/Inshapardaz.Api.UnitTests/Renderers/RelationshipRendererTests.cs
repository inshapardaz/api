using AutoMapper;
using FizzWare.NBuilder;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.UnitTests.Fakes.Helpers;
using Inshapardaz.Api.UnitTests.Fakes.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;
using EnumHelper = Inshapardaz.Api.Helpers.EnumHelper;
using Shouldly;
using Xunit;

namespace Inshapardaz.Api.UnitTests.Renderers
{
    public class RelationshipRendererTests
    {
        public class WhenRendereingRelationships
        {
            private readonly RelationshipView _result;
            private readonly WordRelation _relationship = Builder<WordRelation>.CreateNew().Build();
            private readonly int _dictionaryId = 432;

            public WhenRendereingRelationships()
            {
                Mapper.Initialize(c => c.AddProfile(new MappingProfile()));

                var fakeLinkRenderer = new FakeLinkRenderer();
                var fakeUserHelper = new FakeUserHelper();
                var renderer = new RelationRenderer(fakeLinkRenderer, new EnumRenderer(), fakeUserHelper);

                _result = renderer.Render(_relationship, _dictionaryId);
            }

            [Fact]
            public void ShouldRenderRelationship()
            {
                _result.ShouldNotBeNull();
            }

            [Fact]
            public void ShouldRenderId()
            {
                _result.Id.ShouldBe(_relationship.Id);
            }

            [Fact]
            public void ShouldRenderRelationType()
            {
                _result.RelationType.ShouldBe(EnumHelper.GetEnumDescription(_relationship.RelationType));
            }

            [Fact]
            public void ShouldRenderRelationshipTypeId()
            {
                _result.RelationTypeId.ShouldBe((int)_relationship.RelationType);
            }

            [Fact]
            public void ShouldRenderSourceWordId()
            {
                _result.SourceWordId.ShouldBe(_relationship.SourceWordId);
            }

            [Fact]
            public void ShouldRenderRelatedWordId()
            {
                _result.RelatedWordId.ShouldBe(_result.RelatedWordId);
            }

            [Fact]
            public void ShouldRenderLinks()
            {
                _result.Links.ShouldNotBeNull();
            }

            [Fact]
            public void ShouldRenderSelfLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.Self);
            }

            [Fact]
            public void ShouldRenderSourceWordLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.SourceWord);
            }

            [Fact]
            public void ShouldRenderRelatedWordLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.RelatedWord);
            }
        }

        public class WhenRendereingForOwner
        {
            private readonly RelationshipView _result;
            private readonly WordRelation _relationship = Builder<WordRelation>.CreateNew().Build();
            private readonly int _dictionaryId = 121;

            public WhenRendereingForOwner()
            {
                Mapper.Initialize(c => c.AddProfile(new MappingProfile()));

                var fakeLinkRenderer = new FakeLinkRenderer();
                var fakeUserHelper = new FakeUserHelper().AsContributor();
                var renderer = new RelationRenderer(fakeLinkRenderer, new EnumRenderer(), fakeUserHelper);

                _result = renderer.Render(_relationship, _dictionaryId);
            }

            [Fact]
            public void ShouldRenderUpdateLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.Update);
            }

            [Fact]
            public void ShouldRenderDeleteLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.Delete);
            }
        }
    }
}
