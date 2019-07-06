using Bogus;

namespace Inshapardaz.Functions.Tests.Helpers
{
    public static class Random
    {
        private  static readonly Faker Fake = new Faker();

        public static int Number => Fake.Random.Number();

        public static string Text => Fake.Random.String();
    }
}
