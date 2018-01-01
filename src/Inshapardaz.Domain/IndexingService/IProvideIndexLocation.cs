using System;
using System.Collections.Generic;
using System.Text;

namespace Inshapardaz.Domain.IndexingService
{
    public interface IProvideIndexLocation
    {
        string GetDictionaryIndexFolder(int dictionaryId);
    }
}
