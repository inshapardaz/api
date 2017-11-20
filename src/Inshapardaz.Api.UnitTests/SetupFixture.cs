using AutoMapper;
using NUnit.Framework;

namespace Inshapardaz.Api.UnitTests
{
    [SetUpFixture]
    public class GlobalSetup
    {
        [OneTimeSetUp]
        public void InitialzeMapper()
        {
            Mapper.Initialize(c => c.AddProfile(new MappingProfile()));
        }
    }
}