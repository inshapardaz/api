using Bogus;

namespace Inshapardaz.Functions.Tests.Helpers
{
    public static class Random
    {

        public static int Number => new Faker().Random.Number();

        public static string Text => new Faker().Random.String();
    }
}
