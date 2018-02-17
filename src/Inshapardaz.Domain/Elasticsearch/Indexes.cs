using Nest;

namespace Inshapardaz.Domain.Elasticsearch
{
    public static class Indexes
    {
        public const string Dictionaries = "dictionaries";

        public static IndexName Dictionary = "dictionary-*";
    }
}
