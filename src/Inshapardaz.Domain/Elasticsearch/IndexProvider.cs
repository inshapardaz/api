using System;
using System.Collections.Generic;
using System.Text;

namespace Inshapardaz.Domain.Elasticsearch
{
    public interface IProvideIndex
    {
        string GetIndexForDictionary(int dictionaryId);
    }

    public class IndexProvider : IProvideIndex
    {
        public string GetIndexForDictionary(int dictionaryId)
        {
            return $"dictionary-{dictionaryId}";
        }
    }
}
