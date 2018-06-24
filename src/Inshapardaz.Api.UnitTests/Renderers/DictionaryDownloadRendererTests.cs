using AutoMapper;
using FizzWare.NBuilder;
using Inshapardaz.Api.Model;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.Renderers.Dictionary;
using Inshapardaz.Api.UnitTests.Fakes.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Dictionary;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.UnitTests.Renderers
{
    public class DictionaryDownloadRendererTests
    {
        [TestFixture]
        public class WhenRendereingAnonymously
        {
            private readonly DownloadDictionaryView _result;
            private readonly int _wordCount = 23;

            private readonly DownloadJobModel _downloadJob = Builder<DownloadJobModel>.CreateNew().Build();

            public WhenRendereingAnonymously()
            {
                var renderer = new DictionaryDownloadRenderer(new FakeLinkRenderer());

                _result = renderer.Render(_downloadJob);
            }

            [Test]
            public void ShouldRenderDictionary()
            {
                _result.ShouldNotBeNull();
            }
        }
    }
}