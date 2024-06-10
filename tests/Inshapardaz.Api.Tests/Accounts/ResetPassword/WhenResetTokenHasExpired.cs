﻿using Inshapardaz.Api.Views.Accounts;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using NUnit.Framework;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Accounts.ResetPassword
{
    [TestFixture]
    public class WhenResetTokenHasExpired : TestBase
    {
        private HttpResponseMessage _response;
        private string _password;
        private AccountDto _account;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _password = RandomData.String;
            _account = AccountBuilder.Verified()
                .WithResetToken(RandomData.String)
                .WithResetTokenExpiry(DateTime.UtcNow.AddDays(-3))
                .Build();

            _response = await Client.PostObject("/accounts/reset-password",
                new ResetPasswordRequest()
                {
                    Token = _account.ResetToken,
                    Password = _password
                });
        }

        [Test]
        public void ShouldReturnBadRequest()
        {
            _response.ShouldBeBadRequest();
        }

        [Test]
        public async Task ShouldNotBeAbleToAuthenticateWithNewToken()
        {
            var response = await Client.PostObject("/accounts/authenticate", new AuthenticateRequest { Email = _account.Email, Password = _password });
            response.ShouldBeUnauthorized();
        }
    }
}