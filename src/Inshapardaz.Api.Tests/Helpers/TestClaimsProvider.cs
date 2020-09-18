using Inshapardaz.Domain.Adapters;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Inshapardaz.Api.Tests.Helpers
{
    // See https://gunnarpeipman.com/aspnet-core-integration-tests-users-roles/
    public class TestClaimsProvider
    {
        public IList<Claim> Claims { get; }

        public TestClaimsProvider(IList<Claim> claims)
        {
            Claims = claims;
        }

        public TestClaimsProvider()
        {
            Claims = new List<Claim>();
        }

        internal static TestClaimsProvider WithAuthLevel(Permission Permission, Guid userId)
        {
            var provider = new TestClaimsProvider();

            if (userId == Guid.Empty)
            {
                userId = Guid.NewGuid();
            }

            if (Permission != Permission.Unauthorised)
            {
                provider.Claims.Add(new Claim(ClaimTypes.NameIdentifier, userId.ToString()));
                provider.Claims.Add(new Claim(ClaimTypes.Name, "Library Admin user"));
                provider.Claims.Add(new Claim("permissions", Permission.ToString().ToLower()));
            }

            return provider;
        }
    }
}
