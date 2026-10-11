using Inshapardaz.Domain.Models.Dictionaries;

namespace Inshapardaz.Domain.Adapters;

// Reads an uploaded dictionary (a .zip / .tar.gz archive, or a single StarDict/DSL file) into a
// dictionary and its articles. Throws BadRequestException when the format is not recognised.
public interface IReadDictionaryFiles
{
    Task<ParsedDictionaryModel> ReadAsync(Stream file, string fileName, CancellationToken cancellationToken);
}

// Writes a dictionary out as a downloadable zip in one of the export formats.
public interface IWriteDictionaryFiles
{
    Task<ExportedFileModel> WriteAsync(DictionaryModel dictionary, IReadOnlyList<DictionaryArticleModel> articles, DictionaryExportFormat format, CancellationToken cancellationToken);
}
