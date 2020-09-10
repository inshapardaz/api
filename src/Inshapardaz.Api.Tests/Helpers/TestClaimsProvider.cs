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

        internal static TestClaimsProvider WithAuthLevel(AuthenticationLevel authenticationLevel)
        {
            var provider = new TestClaimsProvider();

            if (authenticationLevel != AuthenticationLevel.Unauthorized)
            {
                provider.Claims.Add(new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()));
                provider.Claims.Add(new Claim(ClaimTypes.Name, "Library Admin user"));
                provider.Claims.Add(new Claim("permissions", authenticationLevel.ToString().ToLower()));
            }

            return provider;
        }
    }
}
