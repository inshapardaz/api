﻿using Inshapardaz.Api.Views.Accounts;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Accounts.ForgotPassword
{
    [TestFixture]
    public class WhenRequestingPasswordReset : TestBase
    {
        private HttpResponseMessage _response;
        private AccountDto _account;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _account = AccountBuilder.Verified().Build();

            _response = await Client.PostObject("/accounts/forgot-password", new ForgotPasswordRequest() { Email = _account.Email });
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldCreateResetTokenForUser()
        {
            AccountAssert.AssertAccountHasResetToken(_account);
        }
    }
}