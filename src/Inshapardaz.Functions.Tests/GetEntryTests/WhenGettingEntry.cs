using System;
using Inshapardaz.Functions;
using Inshapardaz.Functions.Tests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Tests
{
    [TestFixture]
    public class WhenGettingEntry : FunctionTest
    {
        [SetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();
            var handler = Container.GetService<GetEntry>();
            var response = await handler.Run(request, NullLogger.Instance);
            Assert.IsNotNull(response);
        }

        [Test]
        public async Task ShouldHaveSelfLink()
        {
            throw new NotImplementedException();
        }
    }
}