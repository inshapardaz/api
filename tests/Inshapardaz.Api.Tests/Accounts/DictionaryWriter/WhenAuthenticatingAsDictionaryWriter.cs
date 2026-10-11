using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.DataBuilders;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Accounts;
using Inshapardaz.Domain.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Accounts.DictionaryWriter
{
    [TestFixture]
    public class WhenAuthenticatingAsDictionaryWriter : TestBase
    {
        private AuthenticateResponse _authenticateResponse;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var password = RandomData.String;
            var account = AccountBuilder.WithPassword(password).AsDictionaryWriter().Verified().Build();

            var response = await Client.PostObject("/accounts/authenticate", new AuthenticateRequest { Email = account.Email, Password = password });
            _authenticateResponse = await response.GetContent<AuthenticateResponse>();
        }

        [Test]
        public void ShouldReturnDictionaryWriterFlag() => _authenticateResponse.IsDictionaryWriter.Should().BeTrue();

        [Test]
        public void ShouldIssueDictionaryWriterClaim() =>
            new JwtSecurityTokenHandler().ReadJwtToken(_authenticateResponse.AccessToken)
                .Claims.Should().Contain(c => c.Type == "isDictionaryWriter" && c.Value == bool.TrueString);
    }
}
