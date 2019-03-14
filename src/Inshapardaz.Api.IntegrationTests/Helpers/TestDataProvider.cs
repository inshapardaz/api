using System;
using System.Collections.Generic;
using System.Text;
using Bogus;

namespace Inshapardaz.Api.IntegrationTests.Helpers
{
    static class TestDataProvider
    {
        private static readonly Faker _faker;

        static TestDataProvider()
        {
            _faker = new Faker();
        }

        public static int RandomInteger => _faker.Random.Int(-999999999, -666666666);
        public static string RandomAlphaNumeric(int length = 10) => _faker.Random.AlphaNumeric(length);
        public static double RandomPercentage => _faker.Random.Double(0, 100);

        public static string RandomString => _faker.Random.String2(10);
    }
}
