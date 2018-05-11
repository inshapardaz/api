namespace Inshapardaz.Ports.Elasticsearch
{
    public interface IProvideIndex
    {
        string GetIndexForDictionary(int dictionaryId);
        string GetAllDicionaryIndexes();
    }

    public class IndexProvider : IProvideIndex
    {
        public string GetIndexForDictionary(int dictionaryId)
        {
            return $"dictionary-{dictionaryId}";
        }

        public string GetAllDicionaryIndexes()
        {
            return "dictionary-*";
        }
    }
}
