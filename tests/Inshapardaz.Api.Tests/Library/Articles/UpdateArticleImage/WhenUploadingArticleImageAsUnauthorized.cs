﻿using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Helpers;
using NUnit.Framework;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Articles.UpdateArticleImage
{
    [TestFixture]
    public class WhenUploadingArticleImageAsUnauthorized : TestBase
    {
        private HttpResponseMessage _response;
        private long _articleId;
        private byte[] _oldImage;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var article = ArticleBuilder.WithLibrary(LibraryId).Build();
            _articleId = article.Id;
            var imageUrl = DatabaseConnection.GetArticleImageUrl(_articleId);
            _oldImage = await FileStore.GetFile(imageUrl, CancellationToken.None);

            _response = await Client.PutFile($"/libraries/{LibraryId}/articles/{_articleId}/image", RandomData.Bytes);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            ArticleBuilder.CleanUp();
            Cleanup();
        }

        [Test]
        public void ShouldHaveUnauthorisedResult()
        {
            _response.ShouldBeUnauthorized();
        }

        [Test]
        public void ShouldNotHaveUpdatedArticleImage()
        {
            ArticleAssert.ShouldNotHaveUpdatedArticleImage(_articleId, _oldImage, DatabaseConnection, FileStore);
        }
    }
}
