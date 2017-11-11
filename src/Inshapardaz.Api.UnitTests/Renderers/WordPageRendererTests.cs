using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FizzWare.NBuilder;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.UnitTests.Fakes.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;
using Shouldly;
using Xunit;

namespace Inshapardaz.Api.UnitTests.Renderers
{
    public class WordPageRendererTests
    {

        public class WhenGettingFirstPageWithNoFurtherPages
        {
            private readonly PageView<WordView> _result;

            private readonly IList<Word> _words = Builder<Word>.CreateListOfSize(10).Build();

            public WhenGettingFirstPageWithNoFurtherPages()
            {
                Mapper.Initialize(c => c.AddProfile(new MappingProfile()));

                var rendereArgs = new PageRendererArgs<Word>
                {
                    RouteName = "Words",
                    RouteArguments = new PagedRouteArgs(),
                    Page = new Page<Word> {Data = _words, PageNumber = 1, TotalCount = 10, PageSize = 10 }
                };

                var fakeLinkRenderer = new FakeLinkRenderer();
                var renderer = new WordPageRenderer(fakeLinkRenderer, new FakeWordRenderer());

                _result = renderer.Render(rendereArgs, 1);
            }

            [Fact]
            public void ShouldRenderAPage()
            {
                _result.ShouldNotBeNull();
            }

            [Fact]
            public void ShouldRenderCorrectPageSize()
            {
                _result.PageSize.ShouldBe(10);
            }

            [Fact]
            public void ShouldRenderCorrectPageCount()
            {
                _result.PageCount.ShouldBe(1);
            }

            [Fact]
            public void ShouldRenderCorrectCountIndex()
            {
                _result.CurrentPageIndex.ShouldBe(1);
            }

            [Fact]
            public void ShouldRenderData()
            {
                _result.Data.Count().ShouldBe(_words.Count);
            }

            [Fact]
            public void ShouldRenderSelfLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.Self);
            }

            [Fact]
            public void ShouldNotRenderNextLink()
            {
                _result.Links.ShouldNotContain(l => l.Rel == RelTypes.Next);
            }

            [Fact]
            public void ShouldNotRenderPreviousLink()
            {
                _result.Links.ShouldNotContain(l => l.Rel == RelTypes.Previous);
            }
        }

        public class WhenGettingFirstPageWithMorePages
        {
            private readonly PageView<WordView> _result;

            private readonly IList<Word> _words = Builder<Word>.CreateListOfSize(11).Build();

            public WhenGettingFirstPageWithMorePages()
            {
                Mapper.Initialize(c => c.AddProfile(new MappingProfile()));

                var rendereArgs = new PageRendererArgs<Word>
                {
                    RouteName = "Words",
                    RouteArguments = new PagedRouteArgs(),
                    Page = new Page<Word> { Data = _words, PageNumber = 1, TotalCount = 11, PageSize = 10 }
                };

                var fakeLinkRenderer = new FakeLinkRenderer();
                var renderer = new WordPageRenderer(fakeLinkRenderer, new FakeWordRenderer());

                _result = renderer.Render(rendereArgs, 1);
            }

            [Fact]
            public void ShouldRenderAPage()
            {
                _result.ShouldNotBeNull();
            }

            [Fact]
            public void ShouldRenderData()
            {
                _result.Data.Count().ShouldBe(_words.Count);
            }

            [Fact]
            public void ShouldRenderCorrectPageSize()
            {
                _result.PageSize.ShouldBe(10);
            }

            [Fact]
            public void ShouldRenderCorrectPageCount()
            {
                _result.PageCount.ShouldBe(2);
            }

            [Fact]
            public void ShouldRenderCorrectCountIndex()
            {
                _result.CurrentPageIndex.ShouldBe(1);
            }

            [Fact]
            public void ShouldRenderSelfLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.Self);
            }

            [Fact]
            public void ShouldRenderNextLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.Next);
            }

            [Fact]
            public void ShouldNotRenderPreviousLink()
            {
                _result.Links.ShouldNotContain(l => l.Rel == RelTypes.Previous);
            }
        }

        public class WhenGettingSecondPageWithMorePages
        {
            private readonly PageView<WordView> _result;

            private readonly IList<Word> _words = Builder<Word>.CreateListOfSize(21).Build();

            public WhenGettingSecondPageWithMorePages()
            {
                Mapper.Initialize(c => c.AddProfile(new MappingProfile()));

                var rendereArgs = new PageRendererArgs<Word>
                {
                    RouteName = "Words",
                    RouteArguments = new PagedRouteArgs(),
                    Page = new Page<Word> { Data = _words, PageNumber = 2, TotalCount = 21, PageSize = 10 }
                };

                var fakeLinkRenderer = new FakeLinkRenderer();
                var renderer = new WordPageRenderer(fakeLinkRenderer, new FakeWordRenderer());

                _result = renderer.Render(rendereArgs, 1);
            }

            [Fact]
            public void ShouldRenderAPage()
            {
                _result.ShouldNotBeNull();
            }

            [Fact]
            public void ShouldRenderData()
            {
                _result.Data.Count().ShouldBe(_words.Count);
            }

            [Fact]
            public void ShouldRenderCorrectPageSize()
            {
                _result.PageSize.ShouldBe(10);
            }

            [Fact]
            public void ShouldRenderCorrectPageCount()
            {
                _result.PageCount.ShouldBe(3);
            }

            [Fact]
            public void ShouldRenderCorrectCountIndex()
            {
                _result.CurrentPageIndex.ShouldBe(2);
            }

            [Fact]
            public void ShouldRenderSelfLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.Self);
            }

            [Fact]
            public void ShouldRenderNextLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.Next);
            }

            [Fact]
            public void ShouldRenderPreviousLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.Previous);
            }
        }

        public class WhenGettingLastPageWithPreviousPages
        {
            private readonly PageView<WordView> _result;

            private readonly IList<Word> _words = Builder<Word>.CreateListOfSize(30).Build();

            public WhenGettingLastPageWithPreviousPages()
            {
                Mapper.Initialize(c => c.AddProfile(new MappingProfile()));

                var rendereArgs = new PageRendererArgs<Word>
                {
                    RouteName = "Words",
                    RouteArguments = new PagedRouteArgs(),
                    Page = new Page<Word> { Data = _words, PageNumber = 3, TotalCount = 30, PageSize = 10 }
                };

                var fakeLinkRenderer = new FakeLinkRenderer();
                var renderer = new WordPageRenderer(fakeLinkRenderer, new FakeWordRenderer());

                _result = renderer.Render(rendereArgs, 1);
            }

            [Fact]
            public void ShouldRenderAPage()
            {
                _result.ShouldNotBeNull();
            }

            [Fact]
            public void ShouldRenderData()
            {
                _result.Data.Count().ShouldBe(_words.Count);
            }

            [Fact]
            public void ShouldRenderCorrectPageSize()
            {
                _result.PageSize.ShouldBe(10);
            }

            [Fact]
            public void ShouldRenderCorrectPageCount()
            {
                _result.PageCount.ShouldBe(3);
            }

            [Fact]
            public void ShouldRenderCorrectCountIndex()
            {
                _result.CurrentPageIndex.ShouldBe(3);
            }

            [Fact]
            public void ShouldRenderSelfLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.Self);
            }

            [Fact]
            public void ShouldNotRenderNextLink()
            {
                _result.Links.ShouldNotContain(l => l.Rel == RelTypes.Next);
            }

            [Fact]
            public void ShouldRenderPreviousLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.Previous);
            }
        }

        public class WhenGettingPageBeyondPageLimit
        {
            private readonly PageView<WordView> _result;

            public WhenGettingPageBeyondPageLimit()
            {
                Mapper.Initialize(c => c.AddProfile(new MappingProfile()));

                var rendereArgs = new PageRendererArgs<Word>
                {
                    RouteName = "Words",
                    RouteArguments = new PagedRouteArgs(),
                    Page = new Page<Word> { Data = new List<Word>(), PageNumber = 3, TotalCount = 5, PageSize = 10 }
                };

                var fakeLinkRenderer = new FakeLinkRenderer();
                var renderer = new WordPageRenderer(fakeLinkRenderer, new FakeWordRenderer());

                _result = renderer.Render(rendereArgs, 1);
            }

            [Fact]
            public void ShouldRenderAPage()
            {
                _result.ShouldNotBeNull();
            }

            [Fact]
            public void ShouldRenderNoData()
            {
                _result.Data.Count().ShouldBe(0);
            }

            [Fact]
            public void ShouldRenderCorrectPageSize()
            {
                _result.PageSize.ShouldBe(10);
            }

            [Fact]
            public void ShouldRenderCorrectPageCount()
            {
                _result.PageCount.ShouldBe(1);
            }

            [Fact]
            public void ShouldRenderCorrectCountIndex()
            {
                _result.CurrentPageIndex.ShouldBe(3);
            }

            [Fact]
            public void ShouldRenderSelfLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.Self);
            }

            [Fact]
            public void ShouldNotRenderNextLink()
            {
                _result.Links.ShouldNotContain(l => l.Rel == RelTypes.Next);
            }

            [Fact]
            public void ShouldNotRenderPreviousLink()
            {
                _result.Links.ShouldNotContain(l => l.Rel == RelTypes.Previous);
            }
        }
    }
}
