using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Inshapardaz.Api.IntegrationTests.Helpers
{
    public class TestAuthentication
    {
        //public const string TestingCookieAuthentication = "TestCookieAuthentication";
        public const string TestingHeader = "X-Integration-Testing";
        public const string TestingHeaderValue = "inshapardaz-integraion-test";
    }
}
