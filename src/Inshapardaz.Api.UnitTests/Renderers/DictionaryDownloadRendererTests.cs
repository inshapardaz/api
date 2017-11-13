using AutoMapper;
using FizzWare.NBuilder;
using Inshapardaz.Api.Model;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
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
            private int wordCount = 23;

            private readonly DownloadJobModel _downloadJob = Builder<DownloadJobModel>.CreateNew().Build();

            public WhenRendereingAnonymously()
            {
                Mapper.Initialize(c => c.AddProfile(new MappingProfile()));
                
                var renderer = new DictionaryDownloadRenderer();

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