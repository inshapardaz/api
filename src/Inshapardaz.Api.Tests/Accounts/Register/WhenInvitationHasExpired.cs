﻿using Inshapardaz.Api.Models.Accounts;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using NUnit.Framework;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Accounts.Register
{
    [TestFixture]
    public class WhenInvitationHasExpired : TestBase
    {
        private LibraryDto _library;
        private AccountDto _account;
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var name = RandomData.String;
            var password = RandomData.String;
            _library = LibraryBuilder.Build();
            _account = AccountBuilder.InLibrary(_library.Id)
                .AsInvitation().ExpiringInvitation(DateTime.UtcNow.AddDays(-1)).Build();

            _response = await Client.PostObject($"/api/accounts/register/{_account.InvitationCode}",
                new RegisterRequest
                {
                    Name = name,
                    Password = password,
                    AcceptTerms = true
                });
        }

        [Test]
        public void ShouldReturnBadRequest()
        {
            _response.ShouldBeBadRequest();
        }
    }
}
