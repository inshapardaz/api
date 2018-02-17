namespace Inshapardaz.Domain.IndexingService
{
    public interface IProvideIndexLocation
    {
        string GetDictionaryIndexFolder(int dictionaryId);
    }
}
